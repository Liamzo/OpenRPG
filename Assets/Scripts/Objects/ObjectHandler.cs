using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ObjectStatusHandler))]
public class ObjectHandler : MonoBehaviour
{
    public SpriteRenderer spriteRenderer{get; private set;}

    public bool leavesCorpse;

    public BaseStats baseStats;

    public string objectName;
    public ObjectType objectType;

    public Dictionary<ObjectStatNames, Stat> statsObject = new Dictionary<ObjectStatNames, Stat>();

    
    public Vector3 movement;


    public float currentHealth;

    public ObjectStatusHandler objectStatusHandler;


    // Events
    public event System.Action<BaseWeaponHandler, CharacterHandler> OnGetHit = delegate { };
    public event System.Action<float, BaseWeaponHandler, CharacterHandler> OnTakeDamage = delegate { };
    public event System.Action<ObjectHandler> OnDeath = delegate { };


    protected virtual void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        objectName = baseStats.objectName;
        objectType = baseStats.type;

        foreach (ObjectStatValue sv in baseStats.stats) {
            statsObject.Add(sv.statName, new Stat(sv.value));
        }

        currentHealth = statsObject[ObjectStatNames.Health].GetValue();
    }

    protected virtual void Start() {
        QuestManager.GetInstance().RegisterOnDeath(this);

        objectStatusHandler = GetComponent<ObjectStatusHandler>();
    }

    private void FixedUpdate() {
        if (movement == Vector3.zero || !objectStatusHandler.HasMovement())
            return;

        GetComponent<Rigidbody2D>().MovePosition(GetComponent<Rigidbody2D>().position + (Vector2)movement);

        movement = Vector3.zero;
    }

    public virtual void Heal(float amount) {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, statsObject[ObjectStatNames.Health].GetValue());
    }

    public virtual void TakeDamge (float damage, BaseWeaponHandler weapon, CharacterHandler damageDealer) {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, statsObject[ObjectStatNames.Health].GetValue());

        GameObject go = ObjectPoolManager.instance.GetPooledObject(PoolIdentifiers.DamageNumber);
        go.transform.parent = gameObject.transform;
        go.transform.localPosition = Vector3.zero;
        go.GetComponentInChildren<TextMeshPro>().SetText(damage.ToString());
        go.SetActive(true);

        if (currentHealth <= 0) {
            // Give the character that killed this EXP if there is one
            if (damageDealer != null) {
                ExpHandler expHandler;
                if (damageDealer.TryGetComponent<ExpHandler>(out expHandler)) {
                    expHandler.GainExp(baseStats.expValue);
                }
            }

            Die();
            return;
        }

        OnTakeDamage(damage, weapon, damageDealer);
    }

    void Die() {
        // Become corpse
        // Drop items, etc.
        OnDeath(this);
        StartCoroutine("CompleteDeath");
    }
    IEnumerator CompleteDeath () {
        yield return new WaitForSeconds(1f);

        if (leavesCorpse == true) {
            GameObject go = ObjectPoolManager.instance.GetPooledObject(PoolIdentifiers.Corpse);
            go.transform.position = transform.position;
            go.GetComponent<Corpse>().SetVars(GetComponent<ObjectHandler>().spriteRenderer.sprite, GetComponent<InventoryHandler>().inventory);
            go.SetActive(true);
        }
        
        Destroy(gameObject);
    }

    public void GetHit(BaseWeaponHandler weapon, CharacterHandler damageDealer) {
        // // Check for Dodge
        // Stat dodgeChance;
        // if (statsObject.TryGetValue(StatNames.DodgeValue, out dodgeChance)) {
        //     int hitChance = Random.Range(0, 100);

        //     if (dodgeChance.GetValue() > hitChance) {
        //         // Dodge
        //         Debug.Log("Attack Misses");
        //         return;
        //     }
        // }
        if (objectStatusHandler.isDodging) {
            // Dodge
            Debug.Log("Attack Misses");
            return;
        }

        // On-hit effects
        OnGetHit(weapon, damageDealer);

        // // Check for Penetration
        // float penValue = weapon.statsWeapon[WeaponStatNames.PenetrationValue].GetValue();
        // float armourValue = statsObject[ObjectStatNames.ArmourValue].GetValue();

        // float penChance = Random.Range(1, 11) - 2 + penValue;

        // if (armourValue >= penChance) {
        //     // Didn't Penetrate
        //     Debug.Log("Attack Doesn't Penetrate");
        //     return;
        // }

        GameManager.instance.ShakeCamera(5.0f, 0.15f);
        GameManager.instance.HitStop(0.1f);

        // Roll for Damage
        float damage = 0.0f;

        for (int i = 0; i < weapon.statsWeapon[WeaponStatNames.DamageRollCount].GetValue(); i++) {
            damage += Random.Range(1, (int)weapon.statsWeapon[WeaponStatNames.DamageRollValue].GetValue() + 1);
        }

        TakeDamge(damage, weapon, damageDealer);
    }
}



[System.Serializable]
public enum ObjectStatNames {
    Health,
    ArmourValue,
    DodgeValue,
    Weight
}

[System.Serializable]
public struct ObjectStatValue {
    public ObjectStatNames statName;
    public float value;
}

// Very temporary, replace with Faction System
[System.Serializable]
public enum ObjectType {
    None,
    Human,
    Orc,
    Lizard
}