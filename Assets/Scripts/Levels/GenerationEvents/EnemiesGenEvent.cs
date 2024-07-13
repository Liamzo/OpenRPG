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

        for (int i = 0; i < amount; i++)
        {
            Vector2 newPosition = PickRandomLocation(1f, true);

            GameObject enemyPrefab = BaseGenEvent.PickRandomSpawn(enemyPrefabs);

            ObjectHandler character = GameObject.Instantiate(enemyPrefab).GetComponent<ObjectHandler>();
            character.transform.position = newPosition;
            LevelManager.instance.currentLevel.characters.Add(character);
            character.CreateBaseObject();
        }
    }
}
