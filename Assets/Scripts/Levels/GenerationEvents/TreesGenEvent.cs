using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TreesGenEvent : BaseGenEvent
{
    public List<GameObject> treePrefabs;
    public float treeCoverage;
    public override void Generate(LevelData level)
    {
        int attempts = 0;
        for (int i = 0; i < treeCoverage; i++)
        {
            Vector2 newPosition;
            bool validPosition = false;
            do
            {
                newPosition = new Vector2(
                    Random.Range(0f, 100f),
                    Random.Range(0f, 100f));
                
                validPosition = IsPositionValid(newPosition, 5f);

                attempts++;
                if (attempts > 1000) // Prevent infinite loop in case of too many attempts
                {
                    Debug.LogWarning("Unable to place all objects with given constraints.");
                    return;
                }

            } while (!validPosition);

            GameObject treePrefab = treePrefabs[Random.Range(0,treePrefabs.Count)];
            GameObject go = GameObject.Instantiate(treePrefab);

            go.transform.position = newPosition;
            
            LevelManager.instance.currentLevel.things.Add(go);
        }
    }
}
