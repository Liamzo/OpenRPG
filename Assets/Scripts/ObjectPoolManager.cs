using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;

    public List<PoolObject> poolObjects; // The prefab to pool
    //public int poolSize = 10; // The initial size of the object pool

    private Dictionary<PoolIdentifiers, List<GameObject>> pooledObjects = new Dictionary<PoolIdentifiers, List<GameObject>>();

    private void Start()
    {
        instance = this;

        LevelManager.instance.LoadLevelPre += LevelLoaded;
        
        InitializePool();
    }

    private void InitializePool()
    {
        foreach (PoolObject poolObject in poolObjects) {
            List<GameObject> pool = new List<GameObject>();
            for (int i = 0; i < poolObject.poolSize; i++)
            {
                GameObject obj = Instantiate(poolObject.prefab, transform);
                obj.SetActive(false);
                pool.Add(obj);
            }
            pooledObjects.Add(poolObject.identifier, pool);
        }
    }

    public GameObject GetPooledObject(PoolIdentifiers identifier)
    {
        List<GameObject> pool;
        if (pooledObjects.TryGetValue(identifier, out pool)) {
            for (int i = 0; i < pool.Count; i++)
            {
                if (pool[i].activeInHierarchy == false)
                {
                    return pool[i];
                }
            }


            // If all objects are currently in use, create a new one
            foreach (PoolObject poolObject in poolObjects) {
                if (identifier != poolObject.identifier) {
                    continue;
                }

                GameObject newObj = Instantiate(poolObject.prefab, transform);
                newObj.SetActive(false);
                pool.Add(newObj);
                return newObj;
            }
        }

        Debug.LogError("Identifier not found");
        return null;
    }


    public void LevelLoaded() {
        foreach (PoolObject poolObject in poolObjects) {
            if (poolObject.resetOnNewLevel == false) continue;

            foreach (GameObject pooledObject in pooledObjects[poolObject.identifier]) {
                pooledObject.SetActive(false);
            }
        }
    }
}

[System.Serializable]
public struct PoolObject {
    public PoolIdentifiers identifier;
    public GameObject prefab;
    public int poolSize;
    public bool resetOnNewLevel;
}

public enum PoolIdentifiers {
    DamageNumber,
    BasicBullet,
    Corpse,
    ItemSlotUI,
    WeaponSwing,
    BloodEffect,
    QuestStepUI
}