using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterHandler))]
public class BaseBrain : MonoBehaviour
{
    public CharacterHandler character {get; private set;}
    public EquipmentHandler equipmentHandler {get; private set;}

    public Animator _animator {get; protected set;}
    

    public ParticleSystem footSteps;
    protected ParticleSystem.EmissionModule footEmission;

    public Vector3 movement;
    public Vector3 lookingDirection;
    public Vector3 targetLookingDirection;

    float waitClock;

    [Header("Dodging")]
    public Vector3 dodgeMovement;
    public bool wasDodging = false;
    public float dodgeTimer = 0.0f;
    public float dodgeDuration;
    public float dodgeSpeedMulti;
    public float dodgeStaminaCost;
    

    protected virtual void Awake() {
        _animator = GetComponentInChildren<Animator>();
        character = GetComponent<CharacterHandler>();

        footEmission = footSteps.emission;

        GetComponent<ObjectHandler>().OnDeath += OnDeath;
        GetComponent<ObjectHandler>().OnTakeDamage += OnTakeDamage;
    }

    protected virtual void Start() {
        equipmentHandler = GetComponent<EquipmentHandler>();
    }

    protected virtual void Update() {
        if (waitClock >= 0f) {
            waitClock -= Time.deltaTime;

            if (waitClock <= 0f) {
                _animator.SetTrigger("Recover");
            }
        }

        if (dodgeTimer > 0.0f || wasDodging == true)
            DodgeUpdate();
    }

    protected virtual void DodgeUpdate() {
        // Do: Lerp between cur and max speed and back again
        // Too hard to make feel better than simple option for now
        // if (dodgeTimer < dodgeDuration / 2.0f) {
        //     movement = dodgeStartingMovement * Mathf.Lerp(1f, dodgeSpeedMulti, dodgeTimer*2);
        // } else {
        //     movement = dodgeStartingMovement * Mathf.Lerp(dodgeSpeedMulti, 1f, (dodgeTimer - (dodgeDuration / 2.0f)) * 2);
        // }

        Vector3 newMove = dodgeMovement * Time.deltaTime;
        character.movement += newMove;

        dodgeTimer += Time.deltaTime;
        
        if (dodgeTimer >= dodgeDuration) {
            wasDodging = false;
            dodgeTimer = 0.0f;
            _animator.SetBool("Rolling", false);
            character.objectStatusHandler.UnblockMovementControls();
            character.objectStatusHandler.isDodging = false;

            footSteps.Emit(25);
        }
    }

    protected virtual void OnTakeDamage(float damage, WeaponHandler weapon, CharacterHandler damageDealer, BasicBullet projectile) {
        _animator.SetTrigger("Hit");

        float waitTime = weapon.GetStatValue(WeaponStatNames.Stagger) - 0.1f;

        if (waitTime <= 0f && waitClock <= 0f) {
            // If we're currently stunned, don't cancel just because of a new weak attack
            _animator.SetTrigger("Recover");
        } else {
            waitClock = waitTime;
        }

        GameObject bloodGO = ObjectPoolManager.Instance.GetPooledObject(PoolIdentifiers.BloodEffect);
        ParticleSystem bloodEffect = bloodGO.GetComponent<ParticleSystem>();
        bloodEffect.transform.position = equipmentHandler.orbitPoint.position;

        Vector3 bloodDirection;
        if (projectile == null) {
            bloodDirection = (transform.position - weapon.item.owner.transform.position).normalized;
            var shape = bloodEffect.shape;
            shape.arc = 120f;
            shape.rotation = new Vector3(0, 0, 30f);
        } else {
            bloodDirection = projectile.direction;
            var shape = bloodEffect.shape;
            shape.arc = 20f;
            shape.rotation = new Vector3(0, 0, 80f);
        }

        bloodEffect.transform.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector3.up, bloodDirection));


        bloodGO.SetActive(true);
    }

    protected virtual void OnDeath(ObjectHandler obj) {
        _animator.SetTrigger("Death");
        GetComponent<CharacterHandler>().enabled = false;
        this.enabled = false;
        GetComponent<Collider2D>().enabled = false;

        LevelManager.instance.currentLevel.characters.Remove(character);
    }

    public void SetLookingDirection(Vector3 target) { 
        GetComponent<EquipmentHandler>().SpotLook(target);

        lookingDirection = -GetComponent<EquipmentHandler>().rightMeleeSpot.transform.up;
        targetLookingDirection = lookingDirection;
    }
    public void SetTargetLookingDirection(Vector3 target) { 
        Vector2 diff = target - GetComponent<EquipmentHandler>().orbitPoint.position;// (transform.position + new Vector3(0,0.7f,0));

        targetLookingDirection = diff.normalized * -1;
    }

}
