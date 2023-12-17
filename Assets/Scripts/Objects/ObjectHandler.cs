using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using SimpleJSON;
using Ink.Runtime;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ObjectStatusHandler))]
public class ObjectHandler : MonoBehaviour
{
    public string prefabId;
    static int idIncrementor = 0;
    public int objectHandlerId; // Unique per instance ideally

    public SpriteRenderer spriteRenderer{get; private set;}
    public Rigidbody2D rigidBody{get; private set;}

    public BaseStats baseStats;

    public string objectName;
    public ObjectType objectType;

    public Dictionary<ObjectStatNames, Stat> statsObject = new Dictionary<ObjectStatNames, Stat>();

    
    public Vector3 movement;


    public float currentHealth;

    public ObjectStatusHandler objectStatusHandler;


    // Events
    public event System.Action<float, WeaponHandler, CharacterHandler> OnTakeDamage = delegate { };
    public event System.Action<ObjectHandler> OnDeath = delegate { };


    protected virtual void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rigidBody = GetComponentInChildren<Rigidbody2D>();
        
        objectName = baseStats.objectName;
        objectType = baseStats.type;

        foreach (ObjectStatValue sv in baseStats.stats) {
            statsObject.Add(sv.statName, new Stat(sv.value));
        }
    }

    protected virtual void Start() {
        QuestManager.GetInstance().RegisterOnDeath(this);

        objectStatusHandler = GetComponent<ObjectStatusHandler>();
    }

    private void FixedUpdate() {
        rigidBody.velocity = Vector2.zero;
        
        if (movement == Vector3.zero || !objectStatusHandler.HasMovement())
            return;

        rigidBody.MovePosition(rigidBody.position + (Vector2)movement);

        movement = Vector3.zero;
    }

    public virtual void Heal(float amount) {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, statsObject[ObjectStatNames.Health].GetValue());
    }

    public virtual void TakeDamge (float damage, WeaponHandler weapon, CharacterHandler damageDealer) {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, statsObject[ObjectStatNames.Health].GetValue());

        GameObject go = ObjectPoolManager.instance.GetPooledObject(PoolIdentifiers.DamageNumber);
        go.transform.parent = gameObject.transform;
        go.transform.localPosition = Vector3.up;
        go.GetComponentInChildren<TextMeshPro>().SetText(damage.ToString());
        go.SetActive(true);

        OnTakeDamage(damage, weapon, damageDealer);

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
    }

    void Die() {
        // Become corpse
        // Drop items, etc.
        OnDeath(this);
        StartCoroutine("CompleteDeath");
    }
    IEnumerator CompleteDeath () {
        yield return new WaitForSeconds(1f);
        
        Destroy(gameObject);
    }

    public bool GetHit(float damage, WeaponHandler weapon, CharacterHandler damageDealer) {
        // Check for Dodge
        if (objectStatusHandler.isDodging) {
            // Dodge
            return false;
        }

        // On-hit effects
        weapon.CallOnHitTarget(this);

        // // Check for Penetration
        // float penValue = weapon.statsWeapon[WeaponStatNames.PenetrationValue].GetValue();
        // float armourValue = statsObject[ObjectStatNames.ArmourValue].GetValue();

        // float penChance = Random.Range(1, 11) - 2 + penValue;

        // if (armourValue >= penChance) {
        //     // Didn't Penetrate
        //     Debug.Log("Attack Doesn't Penetrate");
        //     return;
        // }

        TakeDamge(damage, weapon, damageDealer);

        return true;
    }


    public JSONNode SaveObject() {
        Destroy(gameObject);

        string json = $"{{prefabId: {prefabId}, objectHandlerId: {objectHandlerId}, x: {transform.position.x}, y: {transform.position.y}, currentHealth: {currentHealth}";

        foreach (ISaveable saveable in GetComponents<ISaveable>()) {
            json += ", " + saveable.SaveComponent();
        }

        return json + "}";
    }

    public void LoadObject(JSONNode data) {
        transform.position = new Vector3(data["x"], data["y"], 0f);
        objectHandlerId = data["objectHandlerId"];
        currentHealth = data["currentHealth"];
    }

    public void CreateBaseObject() {
        objectHandlerId = idIncrementor++;
        currentHealth = statsObject[ObjectStatNames.Health].GetValue();
        Heal(0f); // Temp fix for ui
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