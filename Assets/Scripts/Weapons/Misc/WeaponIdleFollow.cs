using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponIdleFollow : MonoBehaviour
{
    WeaponHandler weaponHandler;

    Vector3 prevPosition;
    float prevAngle;
    float smoothTimeDist = 0.05f;
    Vector3 distVelocity = Vector3.zero;
    float smoothTimeAngle = 0.2f;
    float angleVelocity = 0.0f;

    private void Start() {
        weaponHandler = GetComponent<WeaponHandler>();
    }

    void LateUpdate() {
        IdleFollow();
    }

    protected void IdleFollow() {
        Vector3 movePos = Vector3.SmoothDamp(prevPosition, transform.TransformPoint(Vector3.zero), ref distVelocity, smoothTimeDist);

        weaponHandler._handle.position = movePos;

        // // Angle

        // Check if movement
        float targetRotation = 0f;

        if (Vector3.Distance(prevPosition, weaponHandler._handle.position) > 0.01f) {
            // Direction of movement
            Vector3 idealDir = -(movePos - prevPosition).normalized;

            Vector3 spriteRotation = new Vector3(0f, 0f, weaponHandler.item.objectHandler.spriteRenderer.transform.localEulerAngles.z - 45f); // -45 due to sprite being at an angle rather than straight in source image

            float idealExtraRotation = Vector3.Angle(idealDir, Quaternion.Euler(weaponHandler._handle.eulerAngles + spriteRotation) * Vector3.up);
            float side = -Mathf.Sign(Vector3.Cross(idealDir, Quaternion.Euler(weaponHandler._handle.eulerAngles + spriteRotation) * Vector3.up).z);

            targetRotation = Mathf.Clamp(idealExtraRotation * side, -25f, 25f);
        }

        float newRotation = Mathf.SmoothDamp(prevAngle, targetRotation, ref angleVelocity, smoothTimeAngle);

        weaponHandler._handle.localRotation = Quaternion.Euler(new Vector3(0f,0f,newRotation));

        prevPosition = weaponHandler._handle.position;
        prevAngle = newRotation;

    }
}
