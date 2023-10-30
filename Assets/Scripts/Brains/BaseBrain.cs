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

    protected virtual void OnTakeDamage(float damage, WeaponHandler weapon, CharacterHandler damageDealer) {
        _animator.SetTrigger("Hit");
        GameObject bloodEffect = ObjectPoolManager.instance.GetPooledObject(PoolIdentifiers.BloodEffect);
        bloodEffect.transform.position = transform.position;
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

        Vector2 diff = target - transform.position;

        lookingDirection = diff.normalized;
    }
}
