using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterHandler))]

public class Player : BaseBrain
{
    private static Player instance; // Can be only 1 player

    EquipmentHandler equipmentHandler;

    bool _pointerOverUI;

    [Header("Interactions")]
    InteractionHandler interactingWith;
    InteractionHandler interactionHighlighted;

    [Header("Controls")]
    private bool leftMousePressedBefore = false;
    private bool rightMousePressedBefore = false;


    [Header("Dodging")]
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
        
        equipmentHandler = GetComponent<EquipmentHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        _pointerOverUI = EventSystem.current.IsPointerOverGameObject();


        // Dodging
        if (dodgeCDTimer > 0.0f) {
            dodgeCDTimer -= Time.deltaTime;
        }

        if (InputManager.GetInstance().GetDashPressed() && dodgeTimer <= 0.0f && dodgeCDTimer <= 0.0f && character.currentStamina >= dodgeStaminaCost) {
            // Start dodge
            wasDodging = true;

            MovementControls();

            if (movement == Vector3.zero) {
                movement = ((Vector2)Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - (Vector2)transform.position).normalized * character.statsCharacter[CharacterStatNames.MovementSpeed].GetValue();
            }

            movement *= dodgeSpeedMulti;

            _animator.SetBool("Rolling", true);
            _animator.SetTrigger("Roll");

            character.objectStatusHandler.hasMovementControls = false;
            character.objectStatusHandler.isDodging = true;
            character.ChangeStamina(-dodgeStaminaCost);
            character.objectStatusHandler.BlockRegainStamina(0.5f);

            footSteps.Emit(25);
        } 
        
        if (dodgeTimer > 0.0f || wasDodging == true) {
            // Do: Lerp between cur and max speed and back again
            // Too hard to make feel better than simple option for now
            // if (dodgeTimer < dodgeDuration / 2.0f) {
            //     movement = dodgeStartingMovement * Mathf.Lerp(1f, dodgeSpeedMulti, dodgeTimer*2);
            // } else {
            //     movement = dodgeStartingMovement * Mathf.Lerp(dodgeSpeedMulti, 1f, (dodgeTimer - (dodgeDuration / 2.0f)) * 2);
            // }

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
                character.objectStatusHandler.hasMovementControls = true;
                character.objectStatusHandler.isDodging = false;

                dodgeCDTimer = dodgeCDDuration;

                footSteps.Emit(25);
            }
        }


        // Interactions
        float closest = 3f;
        InteractionHandler closestObject = null;
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, 2f)) {
            InteractionHandler interactionHandler = col.GetComponentInParent<InteractionHandler>();

            if (interactionHandler == null) continue;

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


        // Controls
        if (character.objectStatusHandler.HasMovementControls())
            MovementControls();

        if (character.objectStatusHandler.HasControls()) {
            InteractionControls();
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


    void InteractionControls() {
        if (_pointerOverUI == false) {
            // Attacking
            if (InputManager.GetInstance().GetRightMousePressed()) {
                // Ranged
                equipmentHandler.ToggleMeleeRanged(false);

                if (transform.position.x > Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()).x) {
                    character.spriteRenderer.flipX = true;
                } else {
                    character.spriteRenderer.flipX = false;
                }

                BaseWeaponHandler weapon = character.GetComponent<EquipmentHandler>().rightMeleeSpot.weapon;

                if (weapon == null) return;

                float cost = weapon.statsWeapon[WeaponStatNames.StaminaCostAim].GetValue() * Time.deltaTime;
                character.ChangeStamina(-cost);
                character.objectStatusHandler.BlockRegainStamina(0f);
                movement *= 0.5f; // TODO: Replace with weapon stat

                if (InputManager.GetInstance().GetLeftMousePressed()) {
                    rightMousePressedBefore = true;
                    // Attack with ranged weapon
                    if (character.currentStamina >= weapon.AttackHoldCost()) {
                        cost = weapon.AttackHold();
                        if (cost != 0f) {
                            character.ChangeStamina(-cost);
                            character.objectStatusHandler.BlockRegainStamina(0.2f);
                        }
                    } else if (rightMousePressedBefore) {
                        weapon.AttackCancel();
                        leftMousePressedBefore = false;
                    }
                } else {
                    if (rightMousePressedBefore) {
                        if (character.currentStamina >= weapon.AttackReleaseCost()) {
                            cost = weapon.AttackRelease();
                            if (cost != 0f) {
                                character.ChangeStamina(-cost);
                                character.objectStatusHandler.BlockRegainStamina(0.2f);
                            }
                        } else {
                            weapon.AttackCancel();
                        }
                    }

                    rightMousePressedBefore = false;
                }
            } else {
                // Melee
                equipmentHandler.ToggleMeleeRanged(true);

                BaseWeaponHandler weapon = character.GetComponent<EquipmentHandler>().rightMeleeSpot.weapon;

                if (weapon == null) return;

                if (InputManager.GetInstance().GetLeftMousePressed()) {
                    if (character.currentStamina >= weapon.AttackHoldCost()) {
                        leftMousePressedBefore = true;

                        float cost = weapon.AttackHold();
                        if (cost != 0f) {
                            character.ChangeStamina(-cost);
                            character.objectStatusHandler.BlockRegainStamina(0.2f);
                        }
                    } else if (leftMousePressedBefore) {
                        weapon.AttackCancel();
                        leftMousePressedBefore = false;
                    }
                } else {
                    if (leftMousePressedBefore == true) {
                        if (character.currentStamina >= weapon.AttackReleaseCost()) {
                            float cost = weapon.AttackRelease();
                            if (cost != 0f) {
                                character.ChangeStamina(-cost);
                                character.objectStatusHandler.BlockRegainStamina(0.2f);
                            }

                            if (transform.position.x > Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()).x) {
                                character.spriteRenderer.flipX = true;
                            } else {
                                character.spriteRenderer.flipX = false;
                            }
                        } else {
                            weapon.AttackCancel();
                        }
                    }

                    leftMousePressedBefore = false;
                }
            }
        }

        
        if (InputManager.GetInstance().GetInteractPressed()) {
            OnInteract();
        }

        if (InputManager.GetInstance().GetTabPressed()) {
            OnTab();
        }
    }


    private void FixedUpdate() {
        if (!character.objectStatusHandler.HasMovement()) {
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

    void OnInteract() {
        interactingWith = interactionHighlighted;
        interactionHighlighted?.Use(character);
    }

    void OnTab() {
        // Pause or close down menus
        InventoryHandlerUI.instance.CloseInventory();
        ContainerHandler.instance.CloseContainer();
        DialogueHandler.GetInstance().ExitDialogue();
        TradingManager.GetInstance().ExitTrade();
    }

}
