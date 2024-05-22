using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : BaseThought
{

    Vector3 homePoint;
    public float radiusMax;
    Vector3 prevDir;

    public float pauseDuration;
    float _pauseTimer = 0f;

    public float minPauseWait;
    public float maxPauseWait;
    float _pauseWaitTimer = 0f;


    protected override void Start() {
        base.Start();

        homePoint = transform.position;

        float randomAngle = Random.Range(0f, 2f * Mathf.PI);
        float x = Mathf.Cos(randomAngle);
        float y = Mathf.Sin(randomAngle);

        prevDir = new Vector3(x, y, 0f).normalized;
    }

    public override float Evaluate()
    {
        if (!brain.character.objectStatusHandler.HasMovementControls()) {
            return 0f;
        }

        if (brain.threatHandler.TargetLastSeen == null) {
            return 100f;
        } else {
            return 0f;
        }
    }

    public override void Execute()
    {
        if (_pauseTimer > 0f) {
            // Pause
            _pauseTimer -= Time.deltaTime;
            return;
        }

        if (_pauseWaitTimer <= 0) {
            // Have a pause
            _pauseTimer = pauseDuration;
            _pauseWaitTimer = pauseDuration + Random.Range(minPauseWait, maxPauseWait);
            return;
        } else {
            _pauseWaitTimer -= Time.deltaTime;
        }



        // Calculate maxium angle
        float maxAngle = 180f;

        float distToHome = Vector3.Distance(homePoint, transform.position);
        if (distToHome >= radiusMax - 3f) {
            maxAngle = Mathf.Lerp(180f, 60f, (distToHome - (radiusMax - 3f)) / 3f);
        }

        Vector3 dirToHome = (homePoint - transform.position).normalized;

        float angleBetween = Vector3.Angle(prevDir, dirToHome);
        float side = Mathf.Sign(Vector3.Cross(prevDir, dirToHome).z); // Right is negative, Left is positive

        float noise = (Mathf.PerlinNoise(Time.time * 0.15f , 0.0f) - 0.5f) * 2f;

        float maxChange = maxAngle - angleBetween;

        if (maxChange < 1f) {
            noise -= (maxChange - 1f) * side;
        }

        Vector3 moveDir = Quaternion.AngleAxis(noise, Vector3.forward) * prevDir;

        brain.movement += moveDir.normalized * brain.character.statsCharacter[CharacterStatNames.MovementSpeed].GetValue() * 0.3f; // 30% move speed

        prevDir = moveDir;


        brain.equipmentHandler.rightMeleeSpot.weapon.Holster(); // Temp, do better        
    }
}
