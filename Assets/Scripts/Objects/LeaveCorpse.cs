using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectHandler))]
public class LeaveCorpse : MonoBehaviour
{
    ObjectHandler objectHandler;

    public Sprite corpseSprite;

    void Awake() {
        objectHandler = GetComponent<ObjectHandler>();
    }
    // Start is called before the first frame update
    void Start()
    {
        objectHandler.OnDeath += OnDeath;
    }
    

    protected virtual void OnDeath(ObjectHandler obj) {
        StartCoroutine("SpawnCorpse");
    }

    IEnumerator SpawnCorpse () {
        yield return new WaitForSeconds(1f);

        GameObject go = ObjectPoolManager.instance.GetPooledObject(PoolIdentifiers.Corpse);
        go.transform.position = transform.position;
        go.GetComponent<Corpse>().SetVars(objectHandler.spriteRenderer.sprite, GetComponent<InventoryHandler>().inventory);
        go.SetActive(true);
    }
}
