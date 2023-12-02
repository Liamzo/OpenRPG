using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterHandler))]
public class BaseBrain : MonoBehaviour
{
    public CharacterHandler character {get; private set;}
    public EquipmentHandler equipmentHandler {get; private set;}

    protected Animator _animator;
    

    public ParticleSystem footSteps;
    protected ParticleSystem.EmissionModule footEmission;

    public Vector3 movement;
    public Vector3 lookingDirection;
    public Vector3 targetLookingDirection;

    float waitClock;
    

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
        if (waitClock > 0f) {
            waitClock -= Time.deltaTime;

            if (waitClock <= 0f) {
                _animator.SetTrigger("Recover");
            }
        }
    }

    protected virtual void OnTakeDamage(float damage, WeaponHandler weapon, CharacterHandler damageDealer) {
        _animator.SetTrigger("Hit");

        float waitTime = weapon.GetStatValue(WeaponStatNames.Stagger) - 0.1f;

        if (waitTime < 0f && waitClock <= 0f) {
            // If we're currently stunned, don't cancel just because of a new weak attack
            _animator.SetTrigger("Recover");
        } else {
            waitClock = waitTime;
        }

        GameObject bloodEffect = ObjectPoolManager.instance.GetPooledObject(PoolIdentifiers.BloodEffect);
        bloodEffect.transform.position = equipmentHandler.orbitPoint.position;
        // bloodEffect.transform.parent = equipmentHandler.orbitPoint;
        // bloodEffect.transform.localPosition = Vector3.zero;
        bloodEffect.SetActive(true);
    }

    protected virtual void OnDeath(ObjectHandler obj) {
        _animator.SetTrigger("Death");
        GetComponent<CharacterHandler>().enabled = false;
        this.enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }

    public void SetLookingDirection(Vector3 target) { 
        GetComponent<EquipmentHandler>().SpotLook(target);

        lookingDirection = GetComponent<EquipmentHandler>().rightMeleeSpot.transform.up;
        targetLookingDirection = lookingDirection;
    }
    public void SetTargetLookingDirection(Vector3 target) { 
        Vector2 diff = target - GetComponent<EquipmentHandler>().orbitPoint.position;// (transform.position + new Vector3(0,0.7f,0));

        targetLookingDirection = diff.normalized * -1;
    }
}
