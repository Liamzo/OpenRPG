using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ThreatHandler))]
public class NonPlayerBrain : BaseBrain
{
    public ThreatHandler threatHandler {get; private set;}

    protected NavMeshAgent agent;
    protected NavMeshPath pathToTarget;
    public float distToTarget {get; private set;}

    protected BaseThought[] thoughts;
    public BaseThought thoughtLocked = null;

    [Header("Combat")]
    public float attackCoolDown;
    public float attackCoolDownRange;
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
        
        ResetAttackCoolDown();
    }

    protected override void Update() {
        base.Update();
        
        if (threatHandler.Target != null && attackTimer > 0f) {
            attackTimer -= Time.deltaTime;
        }

        // Check if in combat maybe

        distToTarget = 0f;
        if (threatHandler.TargetLastSeen != null) {
            pathToTarget = new NavMeshPath();
            if (NavMesh.CalculatePath(transform.position, threatHandler.TargetLastSeen.Value, NavMesh.AllAreas, pathToTarget)) {
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

        _animator.SetFloat("Movement", movement.magnitude / character.statsCharacter[CharacterStatNames.MovementSpeed].baseValue);
        footEmission.rateOverTime = 7f * (movement.magnitude / character.statsCharacter[CharacterStatNames.MovementSpeed].baseValue);
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
    }

    public Vector3 GetDirectionFromPath() {
        Vector3 dir = (pathToTarget.corners[1] - transform.position).normalized;

        return dir;
    }
    public Vector3 GetDirectionFromPath(NavMeshPath path) {
        Vector3 dir = (path.corners[1] - transform.position).normalized;

        return dir;
    }

    public Vector3 FindPossibleDirectionFromIdeal(Vector3 idealDir) {
        float score = 10f;

        Debug.DrawLine(transform.position, transform.position + (idealDir * 5), Color.yellow, 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, idealDir, 5f);
        if (hit.collider != null) {
            score += hit.distance * 2;
            Debug.Log(hit.transform.name);
        } else {
            score += 10;
        }

        Vector3 bestDir = idealDir;
        float bestScore = score;

        for(int i = 10; i <= 180; i += 10) {
            // Used to check either side of the Character
            for (int j = -1; j <= 1; j += 2) {
                if (j == 1 && (i == 0 || i == 180)) {
                    continue;
                }

                // Maybe want something that favours when i is low, that way it will prefer to run away

                Vector3 tryDir = Quaternion.AngleAxis(i * j, Vector3.forward) * idealDir;
                score = 10 * Vector3.Dot(idealDir, tryDir);

                hit = Physics2D.Raycast(transform.position, tryDir, 5f);
                if (hit.collider != null) {
                    Debug.DrawLine(transform.position, transform.position + (tryDir * 5), Color.red, 0.1f);
                    score += hit.distance * 2;
                } else {
                    Debug.DrawLine(transform.position, transform.position + (tryDir * 5), Color.white, 0.1f);
                    score += 10;
                }

                if (score > bestScore) {
                    bestScore = score;
                    bestDir = tryDir;
                }
            }
        }

        Debug.DrawLine(transform.position, transform.position + (bestDir * 5), Color.blue, 0.1f);
        return bestDir.normalized;
    }


    public void ResetAttackCoolDown() {
        float randRange = Random.Range(-attackCoolDownRange, attackCoolDownRange);

        attackTimer = attackCoolDown + randRange;
    }
}
