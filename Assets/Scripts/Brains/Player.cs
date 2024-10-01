using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterHandler))]
[RequireComponent(typeof(ThreatHandlerPlayer))]

public class Player : BaseBrain
{
    public static Player instance; // Can be only 1 player

    private bool _pointerOverUI;

    public ThreatHandlerPlayer threatHandler {get; private set;}

    LayerMask visionMask;


    [Header("Interactions")]
    public float interactionDistance = 2f;
    public InteractionHandler interactingWith;
    BaseInteraction interactionDoing;
    InteractionHandler interactionHighlighted;

    [Header("Controls")]
    private List<bool> wasAttacking = new List<bool> {false, false};
    private bool wasSprinting = false;


    [Header("Dodging")]
    Vector3 dodgeMovement;
    public bool wasDodging = false;
    public float dodgeTimer = 0.0f;
    public float dodgeDuration;
    public float dodgeSpeedMulti;
    [SerializeField] float dodgeDurationReleaseModifier;
    float dodgeCDTimer = 0.0f;
    [SerializeField] float dodgeCDDuration = 0.2f;
    public float dodgeStaminaCost;


    public static Player GetInstance() 
    {
        return instance;
    }

    protected override void Awake() {
        base.Awake();

        instance = this;

        threatHandler = GetComponent<ThreatHandlerPlayer>();

        DontDestroyOnLoad(gameObject);
    }

    protected override void Start()
    {
        base.Start();

        character.OnTakeDamage += InteruptInteractionDamage;

        visionMask = LayerMask.GetMask("Default");
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        
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

        if (InputManager.GetInstance().GetHolsterPressed()) {
            if (equipmentHandler.rightMeleeSpot.weapon != null) {
                if (equipmentHandler.rightMeleeSpot.weapon.Holstered) {
                    equipmentHandler.rightMeleeSpot.weapon.Unholster();
                } else {
                    equipmentHandler.rightMeleeSpot.weapon.Holster();
                }
            }
        }


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

        if (InputManager.GetInstance().GetSprintPressed() && character.currentStamina > 0f && movement != Vector3.zero) {
            movement *= character.statsCharacter[CharacterStatNames.SprintMultiplier].GetValue();

            character.ChangeStamina(-5 * Time.deltaTime); // 5 stamina per second
            character.objectStatusHandler.BlockRegainStamina(0f);
            wasSprinting = true;
        } else if (wasSprinting) {
            wasSprinting = false;
        }

        // Look At Mouse
        SetLookingDirection(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));

        _animator.SetFloat("Movement", movement.magnitude / character.statsCharacter[CharacterStatNames.MovementSpeed].BaseValue);
        footEmission.rateOverTime = 7f * (movement.magnitude / character.statsCharacter[CharacterStatNames.MovementSpeed].BaseValue);
    }

    void DodgeControls() {
        if (InputManager.GetInstance().GetDashPressed() && dodgeTimer <= 0.0f && dodgeCDTimer <= 0.0f && character.currentStamina > 0f) {
            // Start dodge
            wasDodging = true;

            dodgeMovement = new Vector3(InputManager.GetInstance().GetMoveDirection().x, InputManager.GetInstance().GetMoveDirection().y, 0).normalized;

            if (dodgeMovement == Vector3.zero) 
                dodgeMovement = ((Vector2)Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - (Vector2)transform.position).normalized;  
            

            if (dodgeMovement.x < 0) {
                character.spriteRenderer.flipX = true;
            } else if (dodgeMovement.x > 0) {
                character.spriteRenderer.flipX = false;
            }

            dodgeMovement *= character.statsCharacter[CharacterStatNames.MovementSpeed].BaseValue * dodgeSpeedMulti;

            _animator.SetBool("Rolling", true);
            _animator.SetTrigger("Roll");

            character.objectStatusHandler.BlockMovementControls();
            character.objectStatusHandler.isDodging = true;
            character.ChangeStamina(-dodgeStaminaCost);
            character.objectStatusHandler.BlockRegainStamina(0.5f);

            footSteps.Emit(25);

            AudioManager.instance.PlayClipRandom(AudioID.Roll, character.audioSource);
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

        Vector3 newMove = dodgeMovement * Time.deltaTime;
        character.movement += newMove;

        if (wasDodging) {
            if (InputManager.GetInstance().GetDashPressed()) {
                dodgeTimer += Time.deltaTime * dodgeDurationReleaseModifier;
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

            InputManager.GetInstance().RegisterSubmitPressed();

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

        // Swithcing Weapons Melee or Ranged
        if (InputManager.GetInstance().GetToggleRangedPressed() && character.currentStamina > 0) {
            // Ranged
            // If switching equipment and previously attacking, cancel that attack
            if (equipmentHandler.meleeDrawn == true && (wasAttacking[0] || wasAttacking[1])) {
                for (int i = 0; i < wasAttacking.Count; i++)
                {
                    if (wasAttacking[i]) {
                        wasAttacking[i] = false;
                        weapon.AttackCancel(i);
                    }
                }
            }
            equipmentHandler.ToggleMeleeRanged(false);

            SpriteLookAtMouse();

            weapon = equipmentHandler.rightMeleeSpot.weapon;

            if (weapon == null) return;

            float cost = weapon.statsWeapon[WeaponStatNames.StaminaCostAim].GetValue() * Time.deltaTime;
            character.ChangeStamina(-cost);
            character.objectStatusHandler.BlockRegainStamina(0f);
            movement *= 0.5f; // TODO: Replace with weapon stat
        } 
        else 
        {
            // Melee
            if (equipmentHandler.meleeDrawn == false && (wasAttacking[0] || wasAttacking[1])) {
                for (int i = 0; i < wasAttacking.Count; i++)
                {
                    if (wasAttacking[i]) {
                        wasAttacking[i] = false;
                        weapon.AttackCancel(i);
                    }
                }
            }
            equipmentHandler.ToggleMeleeRanged(true);

            weapon = equipmentHandler.rightMeleeSpot.weapon;

            if (weapon == null) return;
        }


        CheckAttackInput(weapon, 0, InputManager.GetInstance().GetLeftMousePressed());
        CheckAttackInput(weapon, 1, InputManager.GetInstance().GetRightMousePressed());
    }

    void CheckAttackInput(WeaponHandler weapon, int triggerSlot, bool pressed) {
        if (pressed) {
            equipmentHandler.rightMeleeSpot.weapon.Unholster();

            float weaponHoldCost = weapon.AttackHoldCost(triggerSlot);
            
            if (character.currentStamina > 0 && weapon.CanAttackHold(triggerSlot)) 
            {
                wasAttacking[triggerSlot] = true;

                weapon.AttackHold(triggerSlot);

                if (weaponHoldCost != 0f) {
                    character.ChangeStamina(-weaponHoldCost);
                    character.objectStatusHandler.BlockRegainStamina(0.2f);
                }

                SpriteLookAtMouse();
            } 
            else if (wasAttacking[triggerSlot] && character.currentStamina <= 0f) 
            {
                weapon.AttackCancel(triggerSlot);
                wasAttacking[triggerSlot] = false;
            }
        } 
        else if (wasAttacking[triggerSlot]) 
        {
            float weaponReleaseCost = weapon.AttackReleaseCost(triggerSlot);

            if (character.currentStamina > 0 && weapon.CanAttackRelease(triggerSlot)) 
            {
                weapon.AttackRelease(triggerSlot);

                if (weaponReleaseCost != 0f) {
                    character.ChangeStamina(-weaponReleaseCost);
                    character.objectStatusHandler.BlockRegainStamina(0.2f);
                }

                SpriteLookAtMouse();    
            } 
            else 
            {
                weapon.AttackCancel(triggerSlot);
            }

            wasAttacking[triggerSlot] = false;
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
            Vector3 targetDir = (interactionHandler.transform.position - transform.position).normalized;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDir, distance, visionMask);

            if (hit.collider != null && hit.collider.gameObject != interactionHandler.gameObject) {
                continue;
            }
            
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
        InventoryHandlerUI.Instance.CloseInventory();
        CancelInteraction();
    }


    private void OnCollisionEnter2D(Collision2D other) {
        //Debug.Log("collision");
    }

}
