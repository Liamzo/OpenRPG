using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterHandler))]

public class Player : BaseBrain
{
    public static Player instance; // Can be only 1 player

    private bool _pointerOverUI;

    [Header("Interactions")]
    public float interactionDistance = 2f;
    public InteractionHandler interactingWith;
    BaseInteraction interactionDoing;
    InteractionHandler interactionHighlighted;

    [Header("Controls")]
    private bool wasAttacking = false;


    [Header("Dodging")]
    Vector3 dodgeMovement;
    public bool wasDodging = false;
    public float dodgeTimer = 0.0f;
    public float dodgeDuration;
    public float dodgeSpeedMulti;
    float dodgeCDTimer = 0.0f;
    float dodgeCDDuration = 1.0f;
    public float dodgeStaminaCost;


    public static Player GetInstance() 
    {
        return instance;
    }

    protected override void Awake() {
        base.Awake();

        instance = this;
    }

    protected override void Start()
    {
        base.Start();

        character.OnTakeDamage += InteruptInteractionDamage;
    }

    // Update is called once per frame
    void Update()
    {
        _pointerOverUI = EventSystem.current.IsPointerOverGameObject();

        SetTargetLookingDirection(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));

        if (dodgeCDTimer > 0.0f) {
            dodgeCDTimer -= Time.deltaTime;
        }

        // Controls
        if (character.objectStatusHandler.HasControls())
            DodgeControls();

        if (character.objectStatusHandler.HasMovementControls())
            MovementControls();
        
        if (character.objectStatusHandler.HasControls()) {
            WeaponControls();
            FindClosestInteractable();
            InteractionControls();
        }

        if (dodgeTimer > 0.0f || wasDodging == true)
            DodgeUpdate();

        if (InputManager.GetInstance().GetTabPressed())
            OnTab();

        // Check if too far away for object interaction
        if (interactingWith != null) {
            float distance = Vector3.Distance(interactingWith.transform.position, transform.position);

            if (distance > interactionDistance + 1f) {
                CancelInteraction();
            }
        }
        
    }

    void MovementControls() {
        movement = new Vector3(InputManager.GetInstance().GetMoveDirection().x, InputManager.GetInstance().GetMoveDirection().y, 0);

        if (movement.magnitude > 1) {
            movement = movement.normalized;
        }


        if (movement.x < 0) {
            character.spriteRenderer.flipX = true;
        } else if (movement.x > 0) {
            character.spriteRenderer.flipX = false;
        }

        movement *= character.statsCharacter[CharacterStatNames.MovementSpeed].GetValue();

        // Look At Mouse
        SetLookingDirection(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
    }

    void DodgeControls() {
        if (InputManager.GetInstance().GetDashPressed() && dodgeTimer <= 0.0f && dodgeCDTimer <= 0.0f && character.currentStamina >= dodgeStaminaCost) {
            // Start dodge
            wasDodging = true;

            MovementControls();

            dodgeMovement = movement;

            if (dodgeMovement == Vector3.zero) {
                dodgeMovement = ((Vector2)Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - (Vector2)transform.position).normalized * character.statsCharacter[CharacterStatNames.MovementSpeed].GetValue();
                
                if (dodgeMovement.x < 0) {
                    character.spriteRenderer.flipX = true;
                } else if (dodgeMovement.x > 0) {
                    character.spriteRenderer.flipX = false;
                }
            }

            dodgeMovement *= dodgeSpeedMulti;

            _animator.SetBool("Rolling", true);
            _animator.SetTrigger("Roll");

            character.objectStatusHandler.BlockMovementControls();
            character.objectStatusHandler.isDodging = true;
            character.ChangeStamina(-dodgeStaminaCost);
            character.objectStatusHandler.BlockRegainStamina(0.5f);

            footSteps.Emit(25);
        } 
    }
    void DodgeUpdate() {
        // Do: Lerp between cur and max speed and back again
        // Too hard to make feel better than simple option for now
        // if (dodgeTimer < dodgeDuration / 2.0f) {
        //     movement = dodgeStartingMovement * Mathf.Lerp(1f, dodgeSpeedMulti, dodgeTimer*2);
        // } else {
        //     movement = dodgeStartingMovement * Mathf.Lerp(dodgeSpeedMulti, 1f, (dodgeTimer - (dodgeDuration / 2.0f)) * 2);
        // }

        Vector3 newMove = dodgeMovement * Time.fixedDeltaTime;
        character.movement += newMove;

        if (wasDodging) {
            if (InputManager.GetInstance().GetDashPressed()) {
                dodgeTimer += Time.deltaTime * 0.5f;
            } else {
                wasDodging = false;
                dodgeTimer += Time.deltaTime;
            }
        } else {
            dodgeTimer += Time.deltaTime;
        }

        if (dodgeTimer >= dodgeDuration) {
            wasDodging = false;
            dodgeTimer = 0.0f;
            _animator.SetBool("Rolling", false);
            character.objectStatusHandler.UnblockMovementControls();
            character.objectStatusHandler.isDodging = false;

            dodgeCDTimer = dodgeCDDuration;

            footSteps.Emit(25);
        }
    }

    void InteractionControls() {
        if (InputManager.GetInstance().GetInteractPressed()) {
            OnInteract();
        }
    }

    void WeaponControls() {
        if (_pointerOverUI == true) return;

        WeaponHandler weapon = equipmentHandler.rightMeleeSpot.weapon;

        if (InputManager.GetInstance().GetRightMousePressed()) {
            // Ranged
            // If switching equipment and previously attacking, cancel that attack
            if (equipmentHandler.wasMeleeDrawn && wasAttacking) {
                weapon.AttackCancel();
            }
            equipmentHandler.ToggleMeleeRanged(false);

            SpriteLookAtMouse();

            weapon = equipmentHandler.rightMeleeSpot.weapon;

            if (weapon == null) return;

            float cost = weapon.statsWeapon[WeaponStatNames.StaminaCostAim].GetValue() * Time.deltaTime;
            character.ChangeStamina(-cost);
            character.objectStatusHandler.BlockRegainStamina(0f);
            movement *= 0.5f; // TODO: Replace with weapon stat

        } else {
            // Melee
            if (equipmentHandler.wasMeleeDrawn == false && wasAttacking) {
                weapon.AttackCancel();
            }
            equipmentHandler.ToggleMeleeRanged(true);

            weapon = equipmentHandler.rightMeleeSpot.weapon;

            if (weapon == null) return;
        }

        if (InputManager.GetInstance().GetLeftMousePressed()) {
            if (character.currentStamina >= weapon.AttackHoldCost()) {
                wasAttacking = true;

                float cost = weapon.AttackHold();
                if (cost != 0f) {
                    character.ChangeStamina(-cost);
                    character.objectStatusHandler.BlockRegainStamina(0.2f);
                }

                SpriteLookAtMouse();

            } else if (wasAttacking) {
                weapon.AttackCancel();
                wasAttacking = false;
            }
        } else {
            if (wasAttacking) {
                if (character.currentStamina >= weapon.AttackReleaseCost()) {
                    float cost = weapon.AttackRelease();
                    if (cost != 0f) {
                        character.ChangeStamina(-cost);
                        character.objectStatusHandler.BlockRegainStamina(0.2f);
                    }

                    SpriteLookAtMouse();

                } else {
                    weapon.AttackCancel();
                }
            }

            wasAttacking = false;
        }
    }

    void SpriteLookAtMouse() {
        if (!character.objectStatusHandler.HasMovementControls()) { return; }

        if (transform.position.x > Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()).x) {
            character.spriteRenderer.flipX = true;
        } else {
            character.spriteRenderer.flipX = false;
        }
    }


    private void FixedUpdate() {
        if (!character.objectStatusHandler.HasMovement() || !character.objectStatusHandler.HasMovementControls()) {
            movement = Vector3.zero;
            _animator.SetFloat("Movement", 0f);
            footEmission.rateOverTime = 0f;
            return;
        }

        Vector3 newMove = movement * Time.fixedDeltaTime;
        character.movement += newMove;
        _animator.SetFloat("Movement", newMove.magnitude);

        if (movement == Vector3.zero || character.objectStatusHandler.isDodging) {
            footEmission.rateOverTime = 0f;
        } else {
            footEmission.rateOverTime = 20f;
        }
    }

    void FindClosestInteractable() {
        float closest = 3f;
        InteractionHandler closestObject = null;
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, interactionDistance)) {
            InteractionHandler interactionHandler = col.GetComponentInParent<InteractionHandler>();

            if (interactionHandler == null || interactionHandler == interactingWith) continue;

            ItemHandler item = col.GetComponentInParent<ItemHandler>();
            if (item?.owner != null) continue;
            

            float distance = Vector3.Distance(interactionHandler.transform.position, transform.position);

            if (distance < closest) {
                closest = distance;
                closestObject = interactionHandler;
            }
        }

        if (closestObject != interactionHighlighted) {
            interactionHighlighted?.Unhighlight();
            closestObject?.Highlight();

            interactionHighlighted = closestObject;
        }
    }

    void OnInteract() {
        if (interactionHighlighted == null) { return; }

        BaseInteraction interaction = interactionHighlighted.GetInteraction(character);

        if (interaction.Continuous) {
            CancelInteraction(); // Cancel current interaction
            interactingWith = interactionHighlighted;
            interactionDoing = interaction;
        }

        if (interaction.Blocking) {
            character.objectStatusHandler.BlockControls();
            character.objectStatusHandler.BlockMovementControls();

        }

        interaction.Interact(character);
    }

    public void CancelInteraction() {
        if (interactionDoing == null) { return; }

        interactionDoing.Cancel();

        if (interactionDoing.Blocking) {
            character.objectStatusHandler.UnblockControls();
            character.objectStatusHandler.UnblockMovementControls();

        }

        interactingWith = null;
        interactionDoing = null;
    }

    void InteruptInteractionDamage(float damage, WeaponHandler weapon, CharacterHandler attacker) {
        CancelInteraction();
    }

    void OnTab() {
        // Close down menus
        InventoryHandlerUI.instance.CloseInventory();
        CancelInteraction();
    }

}
