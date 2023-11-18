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
    [SerializeField] private GameObject targetToFind;

    // Start is called before the first frame update
    void Start()
    {
        characterHandler = GetComponent<CharacterHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetToFind == null) {
            return;
        }

        // Raycast to the target within Sight range and see if clear path
        Vector3 targetDir = ((targetToFind.transform.position + new Vector3(0,0.6f,0)) - (transform.position + new Vector3(0,0.6f,0))).normalized;

        LayerMask mask = LayerMask.GetMask("Default");

        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0,0.6f,0), targetDir, characterHandler.statsCharacter[CharacterStatNames.Sight].GetValue(), mask);
        Debug.DrawLine(transform.position + new Vector3(0,0.6f,0), transform.position + new Vector3(0,0.6f,0) + (targetDir*7));

        if (hit.collider != null) {
            Debug.Log(hit.transform.name);
            if (hit.collider.gameObject == targetToFind) {
                // Found the target
                target = targetToFind;
                targetLastSeen = target.transform.position;
                return;
            }
        }

        // Not in range or sight was blocked
        target = null;
    }
}
