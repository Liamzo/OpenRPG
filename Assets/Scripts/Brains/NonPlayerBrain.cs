using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ThreatHandler))]
public class NonPlayerBrain : BaseBrain
{
    protected BaseThought[] thoughts;

    public BaseThought thoughtLocked = null;

    public ThreatHandler threatHandler {get; private set;}

    protected NavMeshAgent agent;
    protected NavMeshPath pathToTarget;

    public float distToTarget {get; private set;}
    public float attackCoolDown;
    public float attackTimer = 0f;


    protected override void Awake()
    {
        base.Awake();

        thoughts = GetComponentsInChildren<BaseThought>();

        threatHandler = GetComponent<ThreatHandler>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.enabled = false;
    }

    protected override void Update() {
        base.Update();
        
        if (attackTimer > 0f) {
            attackTimer -= Time.deltaTime;
        }

        // Check if in combat maybe

        distToTarget = 0f;
        if (threatHandler.targetLastSeen != null) {
            pathToTarget = new NavMeshPath();
            if (NavMesh.CalculatePath(transform.position, threatHandler.targetLastSeen.Value, NavMesh.AllAreas, pathToTarget)) {
                for(int i = 0; i < pathToTarget.corners.Length-1; i++) {
                    distToTarget += Vector3.Distance(pathToTarget.corners[i], pathToTarget.corners[i+1]);
                }   
            }
        }


        // Find the best thought and execute it
        // I think order in the Editor does matter. Higher == Greater Priority
        BaseThought bestThought = null;

        if (thoughtLocked == null) {
            float bestScore = 0f;

            foreach (BaseThought thought in thoughts) {
                float score = thought.Evaluate();

                if (score > bestScore) {
                    bestThought = thought;
                    bestScore = score;
                }
            }
        } else {
            bestThought = thoughtLocked;
        }

        movement = Vector3.zero; // Reset movement

        if (bestThought != null) {
            bestThought.Execute();
        }
    }

    private void FixedUpdate() {
        if (!character.objectStatusHandler.HasMovement() || !character.objectStatusHandler.HasMovementControls()) {
            _animator.SetFloat("Movement", 0f);
            footEmission.rateOverTime = 0f;
            return;
        }

        // Just for now

        if (movement.x < 0) {
            character.spriteRenderer.flipX = true;
        } else if (movement.x > 0) {
            character.spriteRenderer.flipX = false;
        }

        Vector3 newMove = movement * Time.fixedDeltaTime;
        character.movement += newMove;
        _animator.SetFloat("Movement", newMove.magnitude);

        if (movement == Vector3.zero || character.objectStatusHandler.isDodging) {
            footEmission.rateOverTime = 0f;
        } else {
            footEmission.rateOverTime = 20f;
        }
    }

    public Vector3 GetDirectionFromPath() {
        Vector3 dir = (pathToTarget.corners[1] - transform.position).normalized;

        return dir;
    }
    public Vector3 GetDirectionFromPath(NavMeshPath path) {
        Vector3 dir = (path.corners[1] - transform.position).normalized;

        return dir;
    }
}
