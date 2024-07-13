using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemiesGenEvent : BaseGenEvent
{
    public List<SpawnChance> enemyPrefabs;
    public int minAmount;
    public int maxAmount;

    public override void Generate(LevelData level)
    {
        int amount = Random.Range(minAmount, maxAmount+1);

        int attempts = 0;
        for (int i = 0; i < amount; i++)
        {
            Vector2 newPosition;
            bool validPosition = false;
            do
            {
                newPosition = new Vector2(
                    Random.Range(5f, 95f),
                    Random.Range(5f, 95f));
                
                validPosition = IsPositionValid(newPosition, 5f);

                attempts++;
                if (attempts > 1000) // Prevent infinite loop in case of too many attempts
                {
                    Debug.LogWarning("Unable to place all objects with given constraints.");
                    return;
                }

            } while (!validPosition);

            GameObject enemyPrefab = BaseGenEvent.PickRandomSpawn(enemyPrefabs);

            ObjectHandler character = GameObject.Instantiate(enemyPrefab).GetComponent<ObjectHandler>();
            character.transform.position = newPosition;
            LevelManager.instance.currentLevel.characters.Add(character);
            character.CreateBaseObject();
        }
    }
}
