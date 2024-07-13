using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class BanditCampGenEvent : BaseGenEvent
{
    public List<SpawnChance> campPrefabs;
    public List<SpawnChance> enemyPrefabs;
    public int enemyAmount;
    
    public override void Generate(LevelData level) 
    {
        // Place camp
        Vector2 campPosition = PickRandomLocation(10f, true);

        GameObject campPrefab = BaseGenEvent.PickRandomSpawn(campPrefabs);
        GameObject go = GameObject.Instantiate(campPrefab);

        go.transform.position = campPosition;
        
        LevelManager.instance.currentLevel.things.Add(go);

        go.GetComponentsInChildren<Building>().ToList().ForEach(x => x.Generate());


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
