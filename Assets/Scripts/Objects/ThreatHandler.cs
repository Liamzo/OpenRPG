using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterHandler))]
[RequireComponent(typeof(FactionHandler))]
public class ThreatHandler : MonoBehaviour
{
    private CharacterHandler characterHandler;
    private FactionHandler factionHandler;


    public ObjectHandler Target {get; private set;}
    public Vector3? TargetLastSeen {get; private set;}
    float outLineOfSightTimer = 0.0f;
    [SerializeField] float outLineOfSightDuration = 5f;
    public LineOfSightInfo LineOfSightToTarget {get; private set;}


    // Start is called before the first frame update
    void Start()
    {
        characterHandler = GetComponent<CharacterHandler>();
        factionHandler = GetComponent<FactionHandler>();

        characterHandler.OnTakeDamage += OnTakeDamage;
    }

    // Update is called once per frame
    void Update()
    {
        if (Target == null) {
            Target = FindTargetInRange();
        }

        if (Target == null) return;


        LineOfSightToTarget = CheckLineOfSightToTarget(Target);


        if (LineOfSightToTarget.TargetInLineOfSight) {
            TargetLastSeen = LineOfSightToTarget.hit.collider.bounds.center;
            outLineOfSightTimer = 0.0f;
        } 
        else if (!LineOfSightToTarget.TargetInLineOfSight) 
        {
            outLineOfSightTimer += Time.deltaTime;

            if (outLineOfSightTimer >= outLineOfSightDuration) {
                // Target out of sight for too long, find a new one if possible
                TargetLastSeen = null;
                outLineOfSightTimer = 0.0f;

                Target = FindTargetInRange();
                if (Target != null) LineOfSightToTarget = CheckLineOfSightToTarget(Target);
            }
        }
    }


    public LineOfSightInfo CheckLineOfSightToTarget(ObjectHandler target) {
        bool targetInRange = Vector2.Distance(transform.position, target.transform.position) < characterHandler.statsCharacter[CharacterStatNames.Sight].GetValue();

        Vector3 targetDir = (target.transform.position - transform.position).normalized;

        Vector3 leftPos = characterHandler.Collider.bounds.center + (Quaternion.AngleAxis(90f, Vector3.forward) * targetDir);
        Vector3 leftDir = (target.Collider.bounds.center - leftPos).normalized;
        Vector3 rightPos = characterHandler.Collider.bounds.center + (Quaternion.AngleAxis(-90f, Vector3.forward) * targetDir).normalized;
        Vector3 rightDir = (target.Collider.bounds.center - rightPos).normalized;

        LayerMask mask = LayerMask.GetMask("Default");

        RaycastHit2D hit = Physics2D.Raycast(characterHandler.Collider.bounds.center, targetDir, characterHandler.statsCharacter[CharacterStatNames.Sight].GetValue(), mask);
        RaycastHit2D hitLeft = Physics2D.Raycast(leftPos, leftDir, characterHandler.statsCharacter[CharacterStatNames.Sight].GetValue()+1f, mask);
        RaycastHit2D hitRight = Physics2D.Raycast(rightPos, rightDir, characterHandler.statsCharacter[CharacterStatNames.Sight].GetValue()+1f, mask);


        if (hit.collider == null) {
            return new LineOfSightInfo(false, targetInRange, null, hit);
        } else if (hit.collider.gameObject != target.gameObject) {
            return new LineOfSightInfo(false, targetInRange, hit.collider.gameObject, hit);
        }

        if (hitLeft.collider != null && hitLeft.collider.gameObject == target.gameObject && hitRight.collider != null && hitRight.collider.gameObject == target.gameObject) {
            return new LineOfSightInfo(true, targetInRange, null, hit);
        }

        if (hitLeft.collider != null && hitLeft.collider.gameObject != target.gameObject) {
            return new LineOfSightInfo(false, targetInRange, hitLeft.collider.gameObject, hitLeft);
        } else if (hitRight.collider != null && hitRight.collider.gameObject != target.gameObject) {
            return new LineOfSightInfo(false, targetInRange, hitRight.collider.gameObject, hitRight);
        } else {
            return new LineOfSightInfo(false, targetInRange, null, hitLeft);
        }
    }

    public LineOfSightInfo CheckLineOfSightFromPosition(ObjectHandler target, Vector3 position) {
        bool targetInRange = Vector2.Distance(position, target.transform.position) < characterHandler.statsCharacter[CharacterStatNames.Sight].GetValue();

        Vector3 startingPos = position + new Vector3(0,characterHandler.Collider.bounds.extents.y / 2f,0);
        Vector3 targetDir = (target.transform.position - position).normalized;

        Vector3 leftPos = startingPos + (Quaternion.AngleAxis(90f, Vector3.forward) * targetDir);
        Vector3 leftDir = (target.Collider.bounds.center - leftPos).normalized;
        Vector3 rightPos = startingPos + (Quaternion.AngleAxis(-90f, Vector3.forward) * targetDir).normalized;
        Vector3 rightDir = (target.Collider.bounds.center - rightPos).normalized;

        LayerMask mask = LayerMask.GetMask("Default");

        RaycastHit2D hit = Physics2D.Raycast(startingPos, targetDir, characterHandler.statsCharacter[CharacterStatNames.Sight].GetValue(), mask);
        RaycastHit2D hitLeft = Physics2D.Raycast(leftPos, leftDir, characterHandler.statsCharacter[CharacterStatNames.Sight].GetValue()+1f, mask);
        RaycastHit2D hitRight = Physics2D.Raycast(rightPos, rightDir, characterHandler.statsCharacter[CharacterStatNames.Sight].GetValue()+1f, mask);

        if (hit.collider == null) {
            return new LineOfSightInfo(false, targetInRange, null, hit);
        } else if (hit.collider.gameObject != target.gameObject) {
            return new LineOfSightInfo(false, targetInRange, hit.collider.gameObject, hit);
        }

        if (hitLeft.collider != null && hitLeft.collider.gameObject == target.gameObject && hitRight.collider != null && hitRight.collider.gameObject == target.gameObject) {
            return new LineOfSightInfo(true, targetInRange, null, hit);
        }

        if (hitLeft.collider != null && hitLeft.collider.gameObject != target.gameObject) {
            return new LineOfSightInfo(false, targetInRange, hitLeft.collider.gameObject, hitLeft);
        } else if (hitRight.collider != null && hitRight.collider.gameObject != target.gameObject) {
            return new LineOfSightInfo(false, targetInRange, hitRight.collider.gameObject, hitRight);
        } else {
            return new LineOfSightInfo(false, targetInRange, null, hitLeft);
        }
    }

    ObjectHandler FindTargetInRange() {
        float bestDistance = characterHandler.statsCharacter[CharacterStatNames.Sight].GetValue() + 1f;
        ObjectHandler bestTarget = null;

        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, characterHandler.statsCharacter[CharacterStatNames.Sight].GetValue())) {
            CharacterHandler otherCharacter = col.GetComponent<CharacterHandler>();

            if (otherCharacter == null) continue;

            // Raycast to the target within Sight range and see if clear path
            Vector3 targetDir = (otherCharacter.transform.position - transform.position).normalized;

            LayerMask mask = LayerMask.GetMask("Default");

            RaycastHit2D hit = Physics2D.Raycast(characterHandler.Collider.bounds.center, targetDir, characterHandler.statsCharacter[CharacterStatNames.Sight].GetValue(), mask);

            if (hit.collider != null && hit.collider.gameObject == otherCharacter.gameObject) {
                // Can see the character, evaluate the threat
                // For now, just picks the closest target
                // TODO: Evalute threat of target based on level, equipment, reputation
                float distance = Vector3.Distance(characterHandler.transform.position, transform.position);

                FactionHandler hitFactionHandler = otherCharacter.GetComponent<FactionHandler>();

                if (hitFactionHandler == null) continue;

                float reputation = factionHandler.FindReputation(hitFactionHandler);

                if (reputation <= -100f && distance < bestDistance) {
                    bestTarget = otherCharacter;
                    bestDistance = distance;
                }

            }
        }

        return bestTarget;
    }

    void OnTakeDamage(float damage, WeaponHandler weapon, CharacterHandler attacker) {
        FactionHandler hitFactionHandler = attacker.GetComponent<FactionHandler>();

        if (hitFactionHandler == null) return;

        float reputation = factionHandler.FindReputation(hitFactionHandler);

        if (reputation <= -100f) {
            Target = attacker;
        }
    }
}


public struct LineOfSightInfo {
    public bool TargetInLineOfSight {get; private set;}
    public bool TargetInRange {get; private set;}
    public GameObject LineOfSightBlockedBy {get; private set;}
    public RaycastHit2D hit;

    public LineOfSightInfo (bool TargetInLineOfSight, bool TargetInRange, GameObject LineOfSightBlockedBy, RaycastHit2D hit) {
        this.TargetInLineOfSight = TargetInLineOfSight;
        this.TargetInRange = TargetInRange;
        this.LineOfSightBlockedBy = LineOfSightBlockedBy;
        this.hit = hit;
    }
}