using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterHandler))]
public class ThreatHandler : MonoBehaviour
{
    private CharacterHandler characterHandler;
    public GameObject target;
    public Vector3? targetLastSeen;

    // Later: Replace with Faction System and actually determine threat
    // Overlap circle and evaluation each Character found for Faction etc
    [SerializeField] private string targetToFind;

    // Start is called before the first frame update
    void Start()
    {
        characterHandler = GetComponent<CharacterHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetToFind == "") return;
        
        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, characterHandler.statsCharacter[CharacterStatNames.Sight].GetValue())) {
            CharacterHandler characterHandler = col.GetComponent<CharacterHandler>();

            if (characterHandler == null) continue;

            // Raycast to the target within Sight range and see if clear path
            Vector3 targetDir = (characterHandler.transform.position - transform.position).normalized;

            LayerMask mask = LayerMask.GetMask("Default");

            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0,0.6f,0), targetDir, characterHandler.statsCharacter[CharacterStatNames.Sight].GetValue(), mask);

            if (hit.collider != null && hit.collider.gameObject == characterHandler.gameObject) {
                if (characterHandler.tag == targetToFind) {
                    // Found the target
                    target = characterHandler.gameObject;
                    targetLastSeen = hit.collider.bounds.center; // Collider is offset, this way the aim for the centre of the target
                    return;
                }

                float distance = Vector3.Distance(characterHandler.transform.position, transform.position);
            }
        }

        // Not in range or sight was blocked
        target = null;
    }
}
