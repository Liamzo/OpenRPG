using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(Player))]
public class ThreatHandlerPlayer : MonoBehaviour
{
    private CharacterHandler characterHandler;
    private FactionHandler factionHandler;

    CinemachineVirtualCamera virtualCamera;
    CinemachineFramingTransposer transposer;

    //float CHECK_ENEMIES_CD = 0.5f;
    //float checkEnemiesTimer = 0.0f;
    LayerMask enemiesMask;
    LayerMask visionMask;
    [SerializeField] List<EnemyRangeInfo> enemiesInRange = new();
    private float sizeVelocity = 0.0f;
    private float smoothTimeSize = 0.5f;


    public bool inCombat = false;
    bool leavingCombat = false;
    float leavingTimer = 0f;
    float leavingDuration = 3f;


    // Start is called before the first frame update
    void Start()
    {
        characterHandler = GetComponent<CharacterHandler>();
        factionHandler = GetComponent<FactionHandler>();

        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        enemiesMask = LayerMask.GetMask("Object");
        visionMask = LayerMask.GetMask("Default") | LayerMask.GetMask("Object");
    }

    // Update is called once per frame
    void Update()
    {
        // if (checkEnemiesTimer > 0f) {
        //     checkEnemiesTimer -= Time.deltaTime;
        // } else {
        //     CheckEnemiesInRange();
        //     checkEnemiesTimer = CHECK_ENEMIES_CD;
        // }

        CheckEnemiesInRange();

        if (leavingCombat) {
            if (leavingTimer > 0f) {
                leavingTimer -= Time.deltaTime;
            } else {
                leavingCombat = false;
                inCombat = false;
            }
        }
    }

    private void CheckEnemiesInRange()
    {
        enemiesInRange.Clear();

        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, characterHandler.statsCharacter[CharacterStatNames.Sight].GetValue())) {
            CharacterHandler otherCharacter = col.GetComponent<CharacterHandler>();
            if (otherCharacter == null || otherCharacter == characterHandler) continue;

            FactionHandler hitFactionHandler = otherCharacter.GetComponent<FactionHandler>();
            if (hitFactionHandler == null) continue;


            float reputation = factionHandler.FindReputation(hitFactionHandler);

            if (reputation > -100f) {
                break;
            }

            // Raycast to the target within Sight range and see if clear path
            Vector3 targetDir = (otherCharacter.transform.position - transform.position).normalized;

            RaycastHit2D hit = Physics2D.Raycast(characterHandler.Collider.bounds.center, targetDir, characterHandler.statsCharacter[CharacterStatNames.Sight].GetValue(), visionMask);

            if (hit.collider != null && hit.collider.gameObject == otherCharacter.gameObject) {
                // Enemy in sight
                enemiesInRange.Add(new EnemyRangeInfo(otherCharacter, true));
            } else {
                enemiesInRange.Add(new EnemyRangeInfo(otherCharacter, false));
            }
        }

        if (enemiesInRange.Count == 0) {
            transposer.m_TrackedObjectOffset = new Vector3();
            virtualCamera.m_Lens.OrthographicSize = Mathf.SmoothDamp(virtualCamera.m_Lens.OrthographicSize, 14f, ref sizeVelocity, smoothTimeSize * 2f);

            if (!leavingCombat) {
                leavingCombat = true;
                leavingTimer = leavingDuration;
            }

            //transposer.m_LookaheadTime = 0.5f;

            return;
        }

        inCombat = true;
        leavingCombat = false;

        // Calculate centre point of the enemies in line of sight
        Vector2 centre = new();

        enemiesInRange.ForEach(x => {centre += (Vector2)(x.objectHandler.transform.position - transform.position);}); // Try to make work: if (x.inLineOfSight)
        centre /= enemiesInRange.Count;
        centre /= 2; // Halfway between enemies and player

        transposer.m_TrackedObjectOffset = centre;
        //transposer.m_LookaheadTime = 0;

        Vector2 minPoint = transform.position;
        Vector2 maxPoint = transform.position;
        for (int i = 0; i < enemiesInRange.Count; i++) {
            //if (!enemiesInRange[i].inLineOfSight) continue;

            minPoint.x = Mathf.Min(minPoint.x, enemiesInRange[i].objectHandler.transform.position.x);
            minPoint.y = Mathf.Min(minPoint.y, enemiesInRange[i].objectHandler.transform.position.y);
            maxPoint.x = Mathf.Max(maxPoint.x, enemiesInRange[i].objectHandler.transform.position.x);
            maxPoint.y = Mathf.Max(maxPoint.y, enemiesInRange[i].objectHandler.transform.position.y);
        }
        float targetSpread = Vector2.Distance(minPoint, maxPoint);
        targetSpread = Mathf.Clamp(targetSpread / 1.4f, 6f, 14f);
        targetSpread = Mathf.Min(14f, targetSpread);
        virtualCamera.m_Lens.OrthographicSize = Mathf.SmoothDamp(virtualCamera.m_Lens.OrthographicSize, targetSpread, ref sizeVelocity, smoothTimeSize);
    }
}

public struct EnemyRangeInfo {
    public ObjectHandler objectHandler;
    public bool inLineOfSight;

    public EnemyRangeInfo (ObjectHandler objectHandler, bool inLineOfSight) {
        this.objectHandler = objectHandler;
        this.inLineOfSight = inLineOfSight;
    }
} 