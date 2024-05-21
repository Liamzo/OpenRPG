using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterHandler))]
[RequireComponent(typeof(FactionHandler))]
public class ThreatHandler : MonoBehaviour
{
    private CharacterHandler characterHandler;
    private FactionHandler factionHandler;


    public GameObject target;
    public Vector3? targetLastSeen;
    float lastSeenTimer = 0.0f;
    [SerializeField] float lastSeenDuration = 5f;


    // Start is called before the first frame update
    void Start()
    {
        characterHandler = GetComponent<CharacterHandler>();
        factionHandler = GetComponent<FactionHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, characterHandler.statsCharacter[CharacterStatNames.Sight].GetValue())) {
            CharacterHandler otherCharacter = col.GetComponent<CharacterHandler>();

            if (otherCharacter == null) continue;

            // Raycast to the target within Sight range and see if clear path
            Vector3 targetDir = (otherCharacter.transform.position - transform.position).normalized;

            LayerMask mask = LayerMask.GetMask("Default");

            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0,0.6f,0), targetDir, characterHandler.statsCharacter[CharacterStatNames.Sight].GetValue(), mask);

            if (hit.collider != null && hit.collider.gameObject == otherCharacter.gameObject) {
                // Found the target

                FactionHandler hitFactionHandler = otherCharacter.GetComponent<FactionHandler>();

                if (hitFactionHandler == null) { continue; }

                float reputation = factionHandler.FindReputation(hitFactionHandler);

                if (reputation <= -100f) {
                    target = otherCharacter.gameObject;
                    targetLastSeen = hit.collider.bounds.center; // Collider is offset, this way the aim for the centre of the target
                    return;
                }

                float distance = Vector3.Distance(characterHandler.transform.position, transform.position); // TODO: Targetting, at least based on Distance
            }
        }

        // Not in range or sight was blocked
        target = null;

        if (targetLastSeen != null) {
            lastSeenTimer += Time.deltaTime;

            if (lastSeenTimer >= lastSeenDuration) {
                targetLastSeen = null;
                lastSeenTimer = 0.0f;
            }
        }
    }
}
