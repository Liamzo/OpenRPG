using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : NonPlayerBrain
{
    // protected override void Start() {
    //     base.Start();

    //     homePoint = transform.position;
    //     float randomAngle = Random.Range(0f, 2f * Mathf.PI);
    //     float x = Mathf.Cos(randomAngle);
    //     float y = Mathf.Sin(randomAngle);

    //     dir = new Vector3(x, y, 0f).normalized;
        
    // }
    // // Update is called once per frame
    // protected override void Update()
    // {
    //     if (target == null) {
    //         Idle();
    //         return;
    //     }

    // }

    // Vector3 homePoint;
    // public float radiusMin;
    // public float radiusMax;
    // Vector3 dir;

    // public float pauseDuration;
    // float _pauseTimer = 0f;

    // public float minPauseWait;
    // public float maxPauseWait;
    // float _pauseWaitTimer = 1f;

    // void Idle() {
    //     // Wander around Home Point
    //     if (_pauseTimer > 0f) {
    //         // Pause
    //         _pauseTimer -= Time.deltaTime;
    //         return;
    //     }

    //     if (_pauseWaitTimer <= 0) {
    //         // Have a pause
    //         _pauseTimer = pauseDuration;
    //         _pauseWaitTimer = pauseDuration + Random.Range(minPauseWait, maxPauseWait);
    //         return;
    //     } else {
    //         _pauseWaitTimer -= Time.deltaTime;
    //     }


    //     float noise = (Mathf.PerlinNoise(Time.time * 0.1f , 0.0f) - 0.5f);

    //     Vector3 moveDir = Quaternion.AngleAxis(noise, Vector3.forward) * dir;

    //     transform.position += moveDir * Time.deltaTime;

    //     dir = moveDir;
    // }
}
