using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CircleTarget : BaseThought
{
    public float circleDistance;
    public float circleDistanceRange;
    public float minCircleDistance; // Will move faster when in this distance to get away, not pause

    [Header("Pause")]
    public float minPauseDuration;
    public float maxPauseDuration;
    float _pauseTimer = 0f;

    public float minPauseWait;
    public float maxPauseWait;
    float _pauseWaitTimer = 1f;
    float pausedDistance;

    
    [Header("Direction")]
    public int circleDirection = 1;
    public float minDirectionDuration;
    public float maxDirectionDuration;
    float _directionTimer = 0f;

    protected override void Start() {
        base.Start();
    }

    public override float Evaluate()
    {
        if (brain.threatHandler.target == null || !brain.character.objectStatusHandler.HasMovementControls()) {
            return 0f;
        }

        float value = 0f;

        if (brain.distToTarget < circleDistance * 2) {
            value += 75f;
        }

        if (brain.attackTimer <= 0f) {
            // Can attack so slightly less this
            value -= 20f;
        }

        return value;
    }

    public override void Execute()
    {
        brain.SetLookingDirection(brain.threatHandler.targetLastSeen.Value);

        float dist = Vector3.Distance(transform.position, brain.threatHandler.target.transform.position);

        // Move Speed
        float moveSpeed = brain.character.GetStatValue(CharacterStatNames.MovementSpeed);

        if (dist < minCircleDistance) {
            moveSpeed *= 1.5f;
        } else {
            moveSpeed *= 0.7f;
        }

        // Target
        Vector3 dir = (transform.position - brain.threatHandler.target.transform.position).normalized;
        float vertDist = (circleDistance - circleDistanceRange) + (((circleDistance + circleDistanceRange) - (circleDistance - circleDistanceRange)) * Mathf.PerlinNoise(Time.time * 0.15f , 0.0f));
        Vector3 vertPos = dir * vertDist;

        Vector3 horzPos = Quaternion.AngleAxis(20 * circleDirection, Vector3.forward) * vertPos;
        
        Vector3 horzTarget = brain.threatHandler.target.transform.position + horzPos;

        if (dist > minCircleDistance) {
            // Pause
            if (_pauseTimer > 0f) {
                // Pause
                _pauseTimer -= Time.deltaTime;

                // Maintain distance
                Vector3 vertTarget = brain.threatHandler.target.transform.position + (dir * pausedDistance);
                
                if (Vector3.Distance(transform.position, vertTarget) < 0.1f) {
                    return;
                }

                NavMeshPath vertPath = new NavMeshPath();
                if (NavMesh.CalculatePath(transform.position, vertTarget, NavMesh.AllAreas, vertPath) && vertPath.corners.Length > 1) {
                    brain.movement += brain.GetDirectionFromPath(vertPath) * moveSpeed; // 70% move speed
                }
                
                return;
            }

            if (_pauseWaitTimer <= 0) {
                // Have a pause
                _pauseTimer = Random.Range(minPauseDuration, maxPauseDuration);
                _pauseWaitTimer = _pauseTimer + Random.Range(minPauseWait, maxPauseWait);
                pausedDistance = dist;
                return;
            } else {
                _pauseWaitTimer -= Time.deltaTime;
            }
        } else {
            _pauseTimer = 0f;
            _pauseWaitTimer = _pauseTimer + Random.Range(minPauseWait, maxPauseWait);
        }


        // Direction
        if (_directionTimer > 0f) {
            _directionTimer -= Time.deltaTime;
        } else if (_pauseWaitTimer <= 0) {
            // Switch Direction
            circleDirection *= -1;
            _directionTimer = Random.Range(minDirectionDuration, maxDirectionDuration);
        }

        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(transform.position, horzTarget, NavMesh.AllAreas, path) && path.corners.Length > 1) {
            brain.movement += brain.GetDirectionFromPath(path) * moveSpeed; // 70% move speed
        } else {
            // Bug where Orc gets stuck on wall, thinks there's a path when none exists so keeps moving into wall. Path length returns 2, need to verify points
            circleDirection *= -1;
            horzPos = Quaternion.AngleAxis(20 * circleDirection, Vector3.forward) * vertPos;

            horzTarget = brain.threatHandler.target.transform.position + horzPos;
            if (NavMesh.CalculatePath(transform.position, horzTarget, NavMesh.AllAreas, path)) {
                brain.movement += brain.GetDirectionFromPath(path) * moveSpeed; // 70% move speed
            }
        }
    }
}
