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

    public SpriteRenderer spriteRenderer { get; private set; }
    public Rigidbody2D rigidBody { get; private set; }
    public Collider2D Collider { get; private set; }
    public Collider2D hitboxCollider;
    public AudioSource audioSource { get; private set; }

    public BaseStats baseStats  {get; private set; }

    public string objectName;
    public ObjectType objectType;

    public Dictionary<ObjectStatNames, Stat> statsObject;

    
    public Vector3 movement;
    private Vector3 prevMovement = Vector3.zero;


    public float currentHealth;

    public ObjectStatusHandler objectStatusHandler;


    public ConditionHandler conditionHandler { get; protected set; }


    // Events
    public event System.Action<float, WeaponHandler, CharacterHandler, BasicBullet> OnTakeDamage = delegate { };
    public event System.Action<ObjectHandler> OnDeath = delegate { };


    protected virtual void Awake()
    {
        
    }

    protected virtual void Start() {
        QuestManager.GetInstance().RegisterOnDeath(this);

        objectStatusHandler = GetComponent<ObjectStatusHandler>();
    }


    protected virtual void Update() {
        if (baseStats == null) return;

        conditionHandler.Tick();
    }


    private void FixedUpdate() {
        if (baseStats == null) return;

        rigidBody.velocity = Vector2.zero;
        
        if (!objectStatusHandler.HasMovement())
            return;

        movement = Vector3.Lerp(prevMovement, movement, 0.2f); // TODO: Proper smooth movement

        prevMovement = movement;

        if (movement == Vector3.zero) 
            return;
        

        rigidBody.MovePosition(rigidBody.position + (Vector2)movement);

        movement = Vector3.zero;
    }

    public virtual Renderer GetRenderer() {
        return spriteRenderer;
    }

    public virtual void Heal(float amount) {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, statsObject[ObjectStatNames.Health].GetValue());

        // Edge cases where healing might cause damage
        if (currentHealth <= 0) 
            Die();
    }

    public virtual void TakeDamge (float damage, WeaponHandler weapon, CharacterHandler damageDealer, BasicBullet projectile) {
        damage -= statsObject[ObjectStatNames.ArmourValue].GetValue();

        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, statsObject[ObjectStatNames.Health].GetValue());

        GameObject go = ObjectPoolManager.Instance.GetPooledObject(PoolIdentifiers.DamageNumber);
        //go.transform.parent = gameObject.transform;
        go.transform.position = transform.position;
        go.transform.position += Vector3.up*2;
        go.GetComponentInChildren<TextMeshPro>().SetText(damage.ToString());
        go.SetActive(true);

        OnTakeDamage(damage, weapon, damageDealer, projectile);

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
        hitboxCollider.enabled = false;
        OnDeath(this);
        StartCoroutine("CompleteDeath");
    }
    IEnumerator CompleteDeath () {
        yield return new WaitForSeconds(1f);
        
        Destroy(gameObject);
    }

    public virtual HitOutcome GetHit(WeaponHandler weapon, CharacterHandler damageDealer, BasicBullet projectile) {
        // Check for Dodge
        if (objectStatusHandler.isDodging) {
            // Dodge
            GameManager.instance.ChangeTimeScale(0.25f, 0.1f);

            return HitOutcome.Dodge;
        }

        // // Check for Penetration
        // float penValue = weapon.statsWeapon[WeaponStatNames.PenetrationValue].GetValue();
        // float armourValue = statsObject[ObjectStatNames.ArmourValue].GetValue();

        // float penChance = Random.Range(1, 11) - 2 + penValue;

        // if (armourValue >= penChance) {
        //     // Didn't Penetrate
        //     Debug.Log("Attack Doesn't Penetrate");
        //     return;
        // }

        return HitOutcome.Hit;
    }


    public JSONNode SaveObject() {
        Destroy(gameObject);

        string json = $"{{prefabId: {prefabId}, objectHandlerId: {objectHandlerId}, x: {transform.position.x}, y: {transform.position.y}, currentHealth: {currentHealth}";

        foreach (ISaveable saveable in GetComponents<ISaveable>()) {
            json += ", " + saveable.SaveComponent();
        }

        return json + "}";
    }

    public void LoadObject(BaseStats baseStats, JSONNode data) {
        Setup(baseStats);
        
        transform.position = new Vector3(data["x"], data["y"], 0f);
        objectHandlerId = data["objectHandlerId"];
        currentHealth = data["currentHealth"];

        foreach (ISaveable saveable in GetComponents<ISaveable>()) {
            saveable.LoadComponent(data);
        }
    }

    public void CreateBaseObject(BaseStats baseStats) {
        Setup(baseStats);

        objectHandlerId = idIncrementor++;
        currentHealth = statsObject[ObjectStatNames.Health].GetValue();
        Heal(0f); // Temp fix for ui

        foreach (ISaveable saveable in GetComponents<ISaveable>()) {
            saveable.CreateBase();
        }
    }

    protected virtual void Setup(BaseStats startingStats) {
        if (startingStats == null) {
            if (prefabId == "")
                Debug.LogError("No prefab ID assigned");
            startingStats = PrefabManager.Instance.FindBaseStatsById(prefabId);
        }

        baseStats = startingStats;
        prefabId = baseStats.prefabId;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rigidBody = GetComponentInChildren<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
        audioSource = GetComponentInChildren<AudioSource>();
        
        objectName = baseStats.objectName;
        spriteRenderer.sprite = baseStats.sprite;
        objectType = baseStats.type;

        statsObject = new ();
        foreach (ObjectStatValue sv in baseStats.stats) {
            statsObject.Add(sv.statName, new Stat(sv.value));
        }

        conditionHandler = new ConditionHandler();
    }

}



// Very temporary, replace with Faction System
[System.Serializable]
public enum ObjectType {
    None,
    Human,
    Orc,
    Lizard
}