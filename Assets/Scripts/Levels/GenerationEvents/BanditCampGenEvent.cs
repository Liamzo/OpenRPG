using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BanditCampGenEvent : BaseGenEvent
{
    public List<SpawnChance> campPrefabs;
    public List<SpawnChance> enemyPrefabs;
    public int enemyAmount;
    
    public override void Generate(LevelData level) 
    {
        int attempts = 0;
        
        // Place camp
        Vector2 campPosition;
        bool validPosition = false;
        do
        {
            campPosition = new Vector2(
                Random.Range(0f, 100f),
                Random.Range(0f, 100f));
            
            validPosition = IsPositionValid(campPosition, 10f);

            attempts++;
            if (attempts > 1000) // Prevent infinite loop in case of too many attempts
            {
                Debug.LogWarning("Unable to place all objects with given constraints.");
                return;
            }

        } while (!validPosition);

        GameObject campPrefab = BaseGenEvent.PickRandomSpawn(campPrefabs);
        GameObject go = GameObject.Instantiate(campPrefab);

        go.transform.position = campPosition;
        
        LevelManager.instance.currentLevel.things.Add(go);


        // Spawn Enemies
        for (int i = 0; i < enemyAmount; i++)
        {
            Vector2 banditPosition = new Vector2(
                    Random.Range(-5f, 5f),
                    Random.Range(-5f, 5f));

            GameObject enemyPrefab = BaseGenEvent.PickRandomSpawn(enemyPrefabs);
            ObjectHandler character = GameObject.Instantiate(enemyPrefab).GetComponent<ObjectHandler>();

            character.transform.position = campPosition + banditPosition;

            LevelManager.instance.currentLevel.characters.Add(character);
            character.CreateBaseObject();
        }
    }
}
