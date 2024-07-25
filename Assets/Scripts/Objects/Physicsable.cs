using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectHandler))]
public class Physicsable : MonoBehaviour
{
    ObjectHandler objectHandler;

    public float baseKnockDuration = 0.25f;
    float knockDuration;
    float knockTimer = 99f;

    Vector3 knockForce;
    Vector3 _prevValue;

    void Awake() {
        objectHandler = GetComponent<ObjectHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update() {
        if (knockTimer <= knockDuration) {
            knockTimer += Time.deltaTime;

            float t = EaseOut(knockTimer / knockDuration);
            Vector3 currentValue = Vector3.Lerp(Vector3.zero, knockForce, t);
            
            objectHandler.movement += currentValue - _prevValue;

            _prevValue = currentValue;
        }
    }

    public void KnockBack(Vector3 hitPosition, float knockback) {
        Vector3 dir = (objectHandler.hitboxCollider.bounds.center - hitPosition).normalized;

        // Calculate KnockBack Force
        float weight = objectHandler.statsObject[ObjectStatNames.Weight].GetValue();

        float force = knockback / weight;

        knockForce = dir * force;

        knockTimer = 0f;
        _prevValue = Vector3.zero;
        knockDuration = baseKnockDuration;
    }

    public void Knock(Vector3 dir, float knock) {
        float weight = objectHandler.statsObject[ObjectStatNames.Weight].GetValue();

        float force = knock / weight;

        knockForce = dir * force;

        knockTimer = 0f;
        _prevValue = Vector3.zero;
        knockDuration = baseKnockDuration;
    }


    float Flip(float x)
    {
        return 1 - x;
    }

    public float EaseIn(float t)
    {
        return t * t;
    }

    float EaseOut(float t)
    {
        return Flip(Flip(t) * Flip(t));
    }

    public float EaseInOut(float t)
    {
        return Mathf.Lerp(EaseIn(t), EaseOut(t), t);
    }
}
