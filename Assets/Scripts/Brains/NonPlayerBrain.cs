using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ThreatHandlerNPC))]
public class NonPlayerBrain : BaseBrain
{
    public ThreatHandlerNPC threatHandler {get; private set;}

    protected NavMeshAgent agent;
    protected NavMeshPath pathToTarget;
    public float distToTarget {get; private set;}

    protected BaseThought[] activeThoughts;
    protected BaseReaction[] reactionThoughts;
    public BaseThought thoughtLocked = null;
    public BaseThought previousThought = null;

    [Header("Combat")]
    public float attackCoolDown;
    public float attackCoolDownRange;
    public float attackTimer = 0f;


    protected override void Awake()
    {
        base.Awake();

        BaseThought[] allThoughts = GetComponentsInChildren<BaseThought>();

        activeThoughts = allThoughts.Where(thought => thought is not BaseReaction).ToArray();
        reactionThoughts = allThoughts.Where(thought => thought is BaseReaction).Cast<BaseReaction>().ToArray();

        threatHandler = GetComponent<ThreatHandlerNPC>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.enabled = false;
        
        ResetAttackCoolDown();
    }

    protected override void Start() {
        base.Start();
        
        character.OnTakeDamage += InteruptThought;
        threatHandler.OnReact += AttemptReaction;
    }

    protected void InteruptThought(float damage, WeaponHandler weapon, CharacterHandler damageDealer, BasicBullet projectile) {
        if (thoughtLocked != null) {
            thoughtLocked.Cancel();
        }

        thoughtLocked = null;
    }

    void AttemptReaction(WeaponHandler weapon, GameObject projectile) {
        if (previousThought?.canReact == false) return;

        if (thoughtLocked == null) {
            BaseThought bestThought = null;
            float bestScore = 0f;

            foreach (BaseReaction thought in reactionThoughts) {
                float score = thought.Evaluate(weapon, projectile);

                if (score > bestScore) {
                    bestThought = thought;
                    bestScore = score;
                }
            }

            thoughtLocked = bestThought;
        }
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

            foreach (BaseThought thought in activeThoughts) {
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

        previousThought = bestThought;

        _animator.SetFloat("Movement", movement.magnitude / character.statsCharacter[CharacterStatNames.MovementSpeed].BaseValue);
        footEmission.rateOverTime = 7f * (movement.magnitude / character.statsCharacter[CharacterStatNames.MovementSpeed].BaseValue);
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
        int tryAngle = 15;
        bool idealClear = true;
        List<bool> rightSideClear = new List<bool>();
        List<bool> leftSideClear = new List<bool>();

        LayerMask mask = LayerMask.GetMask("Default") | LayerMask.GetMask("Object");

        Debug.DrawLine(character.Collider.bounds.center, character.Collider.bounds.center + (idealDir.normalized * 2f), Color.yellow);
        RaycastHit2D hit = Physics2D.Raycast(character.Collider.bounds.center, idealDir, 2f, mask);
        if (hit.collider != null) {
            idealClear = false;
        }

        for(int i = tryAngle; i <= 180; i += tryAngle) {
            // Used to check either side of the Character
            for (int j = -1; j <= 1; j += 2) {
                if (j == 1 && i == 180) {
                    continue;
                }

                Vector3 tryDir = Quaternion.AngleAxis(i * j, Vector3.forward) * idealDir;

                hit = Physics2D.Raycast(character.Collider.bounds.center, tryDir, 2f, mask);
                if (hit.collider != null) {
                    //Debug.DrawLine(collider.bounds.center, collider.bounds.center + (tryDir * 5f), Color.red, 0.1f);
                    if (j == 1) {
                        rightSideClear.Add(false);
                    } else {
                        leftSideClear.Add(false);
                    }
                } else {
                    //Debug.DrawLine(collider.bounds.center, collider.bounds.center + (tryDir * 5f), Color.white, 0.1f);
                    if (j == 1) {
                        rightSideClear.Add(true);
                    } else {
                        leftSideClear.Add(true);
                    }
                }

            }
        }

        

        if (idealClear && rightSideClear[0] && leftSideClear[0]) {
            Debug.DrawLine(character.Collider.bounds.center, character.Collider.bounds.center + (idealDir.normalized * 5f), Color.green);
            return idealDir.normalized;
        }

        int rightSideCount = idealClear ? 1 : 0;
        int leftSideCount = idealClear ? 1 : 0;

        for(int i = 0; i < rightSideClear.Count; i++) {
            if (rightSideClear[i]) {
                rightSideCount++;
            } else {
                rightSideCount = 0;
            }

            if (rightSideCount == 3) {
                // Find a direction clear on both sides
                Vector3 bestDir = Quaternion.AngleAxis(i*tryAngle, Vector3.forward) * idealDir;
                Debug.DrawLine(character.Collider.bounds.center, character.Collider.bounds.center + (bestDir * 5f), Color.blue);
                return bestDir.normalized;
            }
        }
        for(int i = 0; i < leftSideClear.Count; i++) {
            if (leftSideClear[i]) {
                leftSideCount++;
            } else {
                leftSideCount = 0;
            }

            if (leftSideCount == 3) {
                // Find a direction clear on both sides
                Vector3 bestDir = Quaternion.AngleAxis(i*-tryAngle, Vector3.forward) * idealDir;
                Debug.DrawLine(character.Collider.bounds.center, character.Collider.bounds.center + (bestDir * 5f), Color.gray);
                return bestDir.normalized;
            }
        }


        Debug.LogWarning("Couldn't find a clear direction");
        Debug.DrawLine(character.Collider.bounds.center, character.Collider.bounds.center + (idealDir.normalized * 5f), Color.blue);
        return idealDir.normalized;
    }


    public void ResetAttackCoolDown() {
        float randRange = Random.Range(-attackCoolDownRange, attackCoolDownRange);

        attackTimer = attackCoolDown + randRange;
    }
}
