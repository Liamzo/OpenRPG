using System.Collections;
using System.Collections.Generic;
using Ink.Parsed;
using UnityEngine;

public class WeaponIdleFollow : MonoBehaviour
{
    WeaponHandler weapon;

    Vector3 prevPosition;
    float prevAngle;
    float smoothTimeDist = 0.05f;
    Vector3 distVelocity = Vector3.zero;
    float smoothTimeAngle = 0.2f;
    float angleVelocity = 0.0f;

    bool doFollow = true;

    private void OnEnable() {
        if (weapon == null) return;

        weapon._handle.localPosition = new Vector3(0,0,0);
        weapon._handle.localRotation = Quaternion.identity;
        prevPosition = transform.TransformPoint(Vector3.zero);
        prevAngle = 0f;
    }

    private void Start() {
        weapon = GetComponent<WeaponHandler>();

        foreach (TriggerHolder triggerHolder in weapon.triggerHolders) {
            triggerHolder.OnTrigger += PauseFollow;
        }
    }

    private void Update() {
        if (doFollow == false && weapon.animator.GetCurrentAnimatorStateInfo(0).IsName("Combo_Idle")) {
            prevPosition = transform.TransformPoint(Vector3.zero);
            doFollow = true;
        }
    }

    void LateUpdate() {
        IdleFollow();
    }

    protected void IdleFollow() {
        if (doFollow == false) { return; }
        
        Vector3 movePos = Vector3.SmoothDamp(prevPosition, transform.TransformPoint(Vector3.zero), ref distVelocity, smoothTimeDist);

        weapon._handle.position = movePos;

        // // Angle

        // Check if movement
        float targetRotation = 0f;

        if (Vector3.Distance(prevPosition, weapon._handle.position) > 0.01f) {
            // Direction of movement
            Vector3 idealDir = -(movePos - prevPosition).normalized;

            Vector3 spriteRotation = new Vector3(0f, 0f, weapon.item.objectHandler.spriteRenderer.transform.localEulerAngles.z - 45f); // -45 due to sprite being at an angle rather than straight in source image

            float idealExtraRotation = Vector3.Angle(idealDir, Quaternion.Euler(weapon._handle.eulerAngles + spriteRotation) * Vector3.up);
            float side = -Mathf.Sign(Vector3.Cross(idealDir, Quaternion.Euler(weapon._handle.eulerAngles + spriteRotation) * Vector3.up).z);

            targetRotation = Mathf.Clamp(idealExtraRotation * side, -25f, 25f);
        }

        float newRotation = Mathf.SmoothDamp(prevAngle, targetRotation, ref angleVelocity, smoothTimeAngle);

        weapon._handle.localRotation = Quaternion.Euler(new Vector3(0f,0f,newRotation));

        prevPosition = weapon._handle.position;
        prevAngle = newRotation;

    }

    void PauseFollow(float charge) {
        doFollow = false;
        prevAngle = 0f;
        weapon._handle.localPosition = Vector3.zero;
        weapon._handle.localRotation = Quaternion.Euler(new Vector3(0f,0f,0f));
    }
}
