using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseBrain))]

public class CharacterHandler : ObjectHandler
{
    public BaseBrain brain {get; private set;}
    [SerializeField] private BaseCharacterStats baseCharacterStats;
    [SerializeField] private BaseAttributes baseAttributes;
    public AttributeHandler Attributes { get; private set; }

    public Dictionary<CharacterStatNames, Stat> statsCharacter = new Dictionary<CharacterStatNames, Stat>();
    
    public float currentStamina;

    protected override void Awake()
    {
        base.Awake();

        brain = GetComponent<BaseBrain>();

        Dictionary<CharacterStatNames, AttributeValue> characterAttributeMappings = new Dictionary<CharacterStatNames, AttributeValue>
        {
            {CharacterStatNames.Stamina, new AttributeValue()},
            {CharacterStatNames.StaminaRegen, new AttributeValue()},
            {CharacterStatNames.AttackSpeed, new AttributeValue()},
            {CharacterStatNames.MovementSpeed, new AttributeValue()},
        };
        Dictionary<ObjectStatNames, AttributeValue> objectAttributeMappings = new Dictionary<ObjectStatNames, AttributeValue>
        {
            {ObjectStatNames.Health, new AttributeValue()},
        };
        
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


        Attributes = new AttributeHandler(baseAttributes);

        currentStamina = statsCharacter[CharacterStatNames.Stamina].GetValue();

        OnTakeDamage += OnDamageFlash;
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
            Vector2 hitPosition = projectile == null ? damageDealer.rigidBody.position : projectile.transform.position;
            Vector3 attackerDir = hitPosition - rigidBody.position;

            Debug.Log(hitPosition);

            float angleBetween = Vector3.Angle(GetComponent<BaseBrain>().lookingDirection, attackerDir);

            if (angleBetween <= objectStatusHandler.blockAngle)
            {
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

    IEnumerator 
    DoFlash() {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        spriteRenderer.color = Color.white;
    }
}