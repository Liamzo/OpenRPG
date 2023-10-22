using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectHandler))]
[RequireComponent(typeof(ItemHandler))]
public abstract class BaseWeaponHandler : MonoBehaviour
{
    public Animator animator;
    public Transform _handle;
    public GameObject strategies;

    public ItemHandler item {get; private set;}

    public BaseWeaponStats baseWeaponStats;
    public Dictionary<WeaponStatNames, Stat> statsWeapon = new Dictionary<WeaponStatNames, Stat>();

    bool holestered;


    // For smooth motion
    Vector3 prevPosition;
    float prevAngle;
    float smoothTimeDist = 0.05f;
    Vector3 distVelocity = Vector3.zero;
    float smoothTimeAngle = 0.2f;
    float angleVelocity = 0.0f;

    protected virtual void Awake() {
        item = GetComponent<ItemHandler>();

        foreach (WeaponStatValue sv in baseWeaponStats.stats) {
            statsWeapon.Add(sv.statName, new Stat(sv.value));
        }
    }

    protected virtual void Update() {

    }

    protected virtual void LateUpdate() {

    }

    public void Holster() {
        transform.localPosition = new Vector3(0,0,0);
        transform.localRotation = Quaternion.identity;
        prevPosition = transform.TransformPoint(Vector3.zero);
        prevAngle = 0.0f;

        item.objectHandler.spriteRenderer.enabled = false;
    }
    public void Unholster() {
        transform.localPosition = new Vector3(0,0,0);
        transform.localRotation = Quaternion.identity;
        prevPosition = transform.TransformPoint(Vector3.zero);
        prevAngle = 0.0f;

        item.objectHandler.spriteRenderer.enabled = true;
    }

    public abstract float AttackHoldCost();
    public abstract float AttackHold();
    public abstract float AttackReleaseCost();
    public abstract float AttackRelease();
    public abstract void AttackCancel();

    public abstract void AttackAnticipation();

    protected void IdleFollow() {
        Vector3 movePos = Vector3.SmoothDamp(prevPosition, transform.TransformPoint(Vector3.zero), ref distVelocity, smoothTimeDist);

        _handle.position = movePos;

        // // Angle

        // Check if movement
        float targetRotation = 0f;

        if (Vector3.Distance(prevPosition, _handle.position) > 0.01f) {
            // Direction of movement
            Vector3 idealDir = -(movePos - prevPosition).normalized;

            Vector3 spriteRotation = new Vector3(0f, 0f, item.objectHandler.spriteRenderer.transform.localEulerAngles.z - 45f); // -45 due to sprite being at an angle rather than straight in source image

            float idealExtraRotation = Vector3.Angle(idealDir, Quaternion.Euler(_handle.eulerAngles + spriteRotation) * Vector3.up);
            float side = -Mathf.Sign(Vector3.Cross(idealDir, Quaternion.Euler(_handle.eulerAngles + spriteRotation) * Vector3.up).z);

            targetRotation = Mathf.Clamp(idealExtraRotation * side, -25f, 25f);
        }

        float newRotation = Mathf.SmoothDamp(prevAngle, targetRotation, ref angleVelocity, smoothTimeAngle);

        _handle.localRotation = Quaternion.Euler(new Vector3(0f,0f,newRotation));

        prevPosition = _handle.position;
        prevAngle = newRotation;

    }
}


[System.Serializable]
public enum WeaponStatNames {
    PenetrationValue,
    DamageRollCount,
    DamageRollValue,
    AttackTimer,
    Stagger,
    KnockBack,
    Range,
    Accuracy,
    ClipSize,
    ReloadDuration,
    StaminaCostEnd,
    StaminaCostHold,
    StaminaCostAim,
    SelfKnockForce
}

[System.Serializable]
public struct WeaponStatValue {
    public WeaponStatNames statName;
    public float value;
}