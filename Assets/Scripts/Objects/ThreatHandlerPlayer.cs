using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(Player))]
public class ThreatHandlerPlayer : ThreatHandler
{
    CinemachineVirtualCamera virtualCamera;
    CinemachineFramingTransposer transposer;

    //float CHECK_ENEMIES_CD = 0.5f;
    //float checkEnemiesTimer = 0.0f;
    LayerMask enemiesMask;
    LayerMask visionMask;
    public List<CharacterRangeInfo> charactersInRange { get; private set; } = new();
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

        enemiesMask = LayerMask.GetMask("Hitbox");
        visionMask = LayerMask.GetMask("Default") | LayerMask.GetMask("Hitbox");
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

        FindEnemiesInRange();

        if (leavingCombat) {
            if (leavingTimer > 0f) {
                leavingTimer -= Time.deltaTime;
            } else {
                leavingCombat = false;
                inCombat = false;
            }
        }
    }

    private void FindEnemiesInRange()
    {
        charactersInRange.Clear();

        foreach (Collider2D col in Physics2D.OverlapCircleAll(transform.position, characterHandler.statsCharacter[CharacterStatNames.Sight].GetValue())) {
            CharacterHandler otherCharacter = col.GetComponent<CharacterHandler>();
            if (otherCharacter == null || otherCharacter == characterHandler) continue;

            FactionHandler hitFactionHandler = otherCharacter.GetComponent<FactionHandler>();
            if (hitFactionHandler == null) continue;


            float reputation = factionHandler.FindReputation(hitFactionHandler);

            // Raycast to the target within Sight range and see if clear path
            Vector3 targetDir = (otherCharacter.transform.position - transform.position).normalized;

            RaycastHit2D hit = Physics2D.Raycast(characterHandler.hitboxCollider.bounds.center, targetDir, characterHandler.statsCharacter[CharacterStatNames.Sight].GetValue(), visionMask);

            if (hit.collider != null && hit.collider.transform.root.gameObject == otherCharacter.gameObject) {
                // Enemy in sight
                charactersInRange.Add(new CharacterRangeInfo(otherCharacter, true, reputation));
            } else {
                charactersInRange.Add(new CharacterRangeInfo(otherCharacter, false, reputation));
            }
        }

        if (!EnemyInRange()) {
            transposer.m_TrackedObjectOffset = new Vector3();
            virtualCamera.m_Lens.OrthographicSize = Mathf.SmoothDamp(virtualCamera.m_Lens.OrthographicSize, 14f, ref sizeVelocity, smoothTimeSize * 2f);

            if (!leavingCombat && inCombat) {
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
        Vector2 minPoint = transform.position;
        Vector2 maxPoint = transform.position;
        for (int i = 0; i < charactersInRange.Count; i++) {
            //if (!enemiesInRange[i].inLineOfSight) continue;
            if (charactersInRange[i].reputation > -100f) continue;

            centre += (Vector2)(charactersInRange[i].objectHandler.transform.position - transform.position);

            minPoint.x = Mathf.Min(minPoint.x, charactersInRange[i].objectHandler.transform.position.x);
            minPoint.y = Mathf.Min(minPoint.y, charactersInRange[i].objectHandler.transform.position.y);
            maxPoint.x = Mathf.Max(maxPoint.x, charactersInRange[i].objectHandler.transform.position.x);
            maxPoint.y = Mathf.Max(maxPoint.y, charactersInRange[i].objectHandler.transform.position.y);
        }

        //charactersInRange.ForEach(x => {centre += (Vector2)(x.objectHandler.transform.position - transform.position);}); // Try to make work: if (x.inLineOfSight)
        centre /= charactersInRange.Count;
        centre /= 2; // Halfway between enemies and player
        transposer.m_TrackedObjectOffset = centre;
        //transposer.m_LookaheadTime = 0;

        float targetSize = Vector2.Distance(minPoint, maxPoint);
        targetSize = Mathf.Clamp(targetSize / 1.35f, 6f, 14f);
        //targetSpread = Mathf.Min(14f, targetSpread);
        virtualCamera.m_Lens.OrthographicSize = Mathf.SmoothDamp(virtualCamera.m_Lens.OrthographicSize, targetSize, ref sizeVelocity, smoothTimeSize);

    }

    public bool CheckInLineOfSight(ObjectHandler objectHandler) {
        foreach (CharacterRangeInfo enemyRangeInfo in charactersInRange) {
            if (enemyRangeInfo.objectHandler == objectHandler) {
                if (enemyRangeInfo.inLineOfSight) {
                    return true;
                }
            }
        }

        return false;
    }

    public bool EnemyInRange() {
        foreach (CharacterRangeInfo enemy in charactersInRange) {
            if (enemy.reputation <= -100f) return true;
        }

        return false;
    }

}

public struct CharacterRangeInfo {
    public ObjectHandler objectHandler;
    public bool inLineOfSight;
    public float reputation;

    public CharacterRangeInfo (ObjectHandler objectHandler, bool inLineOfSight, float reputation) {
        this.objectHandler = objectHandler;
        this.inLineOfSight = inLineOfSight;
        this.reputation = reputation;
    }
} 