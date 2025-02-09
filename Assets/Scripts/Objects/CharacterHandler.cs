using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseBrain))]

public class CharacterHandler : ObjectHandler
{
    public BaseBrain brain {get; private set;}
    private BaseCharacterStats baseCharacterStats;
    public AttributeHandler Attributes { get; private set; }

    public Dictionary<CharacterStatNames, Stat> statsCharacter = new Dictionary<CharacterStatNames, Stat>();
    
    public float currentStamina;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start () 
    {
        base.Start();

        LevelManager.instance.LoadLevelPre += LevelLoaded;
    }

    protected override void Update() {
        base.Update();

        if (currentStamina < statsCharacter[CharacterStatNames.Stamina].GetValue() && objectStatusHandler.CanRegainStamina()) {
            ChangeStamina(statsCharacter[CharacterStatNames.StaminaRegen].GetValue() * Time.deltaTime);
        }
    }


    public override HitOutcome GetHit(WeaponHandler weapon, CharacterHandler damageDealer, GameObject projectile) 
    {
        if (objectStatusHandler.isBlocking) {
            // Vector2 hitPosition = projectile == null ? damageDealer.rigidBody.position : projectile.transform.position;
            // Vector3 attackerDir = (hitPosition - rigidBody.position).normalized;

            // float angleBetween = Vector3.Angle(GetComponent<BaseBrain>().lookingDirection, attackerDir);

            Vector2 hitPosition = projectile == null ? damageDealer.hitboxCollider.bounds.center : projectile.transform.position;
            Vector3 attackerDir = (hitPosition - (Vector2)hitboxCollider.bounds.center).normalized;
            float angleBetween = Vector3.Angle(GetComponent<BaseBrain>().lookingDirection, attackerDir);

            // Debug.Log($"-");
            // Debug.Log($"{hitboxCollider.bounds.center}");
            // Debug.Log($"{hitPosition.x},{hitPosition.y}");
            // Debug.Log($"{attackerDir.x},{attackerDir.y}");
            // Debug.Log($"{angleBetween}");

            if (angleBetween <= objectStatusHandler.blockAngle)
            {
                // Did block, check if a Parry
                if (objectStatusHandler.blockTimer <= 0.3f) {
                    if (objectStatusHandler.timeSinceLastBlock > 0.1f) {
                        // Did a Parry
                        GameManager.instance.ChangeTimeScale(0.5f, 0.4f);

                        GameObject sparksEffect = ObjectPoolManager.Instance.GetPooledObject(PoolIdentifiers.EffectSparks);
                        Vector2 sparksPosition = ((hitPosition - (Vector2)hitboxCollider.bounds.center) / 2f) + (Vector2)hitboxCollider.bounds.center;
                        sparksEffect.transform.position = sparksPosition;
                        sparksEffect.SetActive(true);

                        return HitOutcome.Parry;
                    }
                }

                GameManager.instance.ChangeTimeScale(0.25f, 0.1f);
                return HitOutcome.Block;
            }
        }

        return base.GetHit(weapon, damageDealer, projectile);
    }


    public virtual void ChangeStamina(float changeAmount) {
        currentStamina = Mathf.Clamp(currentStamina + changeAmount, 0f, statsCharacter[CharacterStatNames.Stamina].GetValue());

        if (currentStamina <= 0f) {
            conditionHandler.AddCondition(new OutOfBreathCondition(this, 2f, 5f, 75f));
        }
    }

    public float GetStatValue(CharacterStatNames statName) {
        return statsCharacter[statName].GetValue();
    }

    void OnDamageFlash(float damage, WeaponHandler weapon, CharacterHandler damageDealer) {
        StartCoroutine("DoFlash");
    }

    public void LevelLoaded() {
        currentStamina = statsCharacter[CharacterStatNames.Stamina].GetValue();

        ChangeStamina(0); // Updates UI
    }

    protected override void Setup(BaseStats baseStats) {
        base.Setup(baseStats);

        brain = GetComponent<BaseBrain>();

        Dictionary<CharacterStatNames, AttributeValue> characterAttributeMappings = new Dictionary<CharacterStatNames, AttributeValue>
        {
            {CharacterStatNames.Stamina, new AttributeValue(AttributeNames.Agility, 5f)},
            {CharacterStatNames.StaminaRegen, new AttributeValue(AttributeNames.Agility, 2f)},
            {CharacterStatNames.MovementSpeed, new AttributeValue(AttributeNames.Agility, 0.2f)},
        };
        Dictionary<ObjectStatNames, AttributeValue> objectAttributeMappings = new Dictionary<ObjectStatNames, AttributeValue>
        {
            {ObjectStatNames.Health, new AttributeValue(AttributeNames.Toughness, 5f)},
        };


        baseCharacterStats = baseStats.GetStatBlock<BaseCharacterStats>();
        
        foreach (CharacterStatValue sv in baseCharacterStats.stats) {
            Stat stat = new Stat(sv.value);
            statsCharacter.Add(sv.statName, stat);
            if (characterAttributeMappings.ContainsKey(sv.statName))
                stat.AddAttribute(characterAttributeMappings[sv.statName], this);
        }

        foreach (KeyValuePair<ObjectStatNames, Stat> objectStat in statsObject)
        {
            if (objectAttributeMappings.ContainsKey(objectStat.Key))
                objectStat.Value.AddAttribute(objectAttributeMappings[objectStat.Key], this);
        }

        Attributes = new AttributeHandler(baseCharacterStats.attributes);

        currentStamina = statsCharacter[CharacterStatNames.Stamina].GetValue();

        OnTakeDamage += OnDamageFlash;
    }

    IEnumerator DoFlash() {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        spriteRenderer.color = Color.white;
    }
}