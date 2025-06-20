using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[System.Serializable]
public class GroundPatchGenEvent : BaseGenEvent
{
    public RuleTile patchTile;
    public GameObject tilemapPrefab;
    public bool exclusionZone;

    public int minWidth;
    public int maxWidth;

    public int numberOfPatchs = 1;
    public int minPatchSize;
    public int maxPatchSize;
    public float holeChance = 0.0f;
    public float continueChance = 0.6f;
    public bool spawnEdgeThings;
    public List<ThingSpawnChance> edgePrefabs;
    public float spawnEdgeChance;
    

    public override void Generate(LevelData level)
    {
        GameObject newTileMap = GameObject.Instantiate(tilemapPrefab, GameObject.FindWithTag("TileMap").transform);
        Tilemap tilemap = newTileMap.GetComponent<Tilemap>();
        tilemap.GetComponent<TilemapRenderer>().sortingOrder = GameObject.FindWithTag("TileMap").transform.childCount;

        List<Vector3Int> patchTiles = new();

        for (int i = 0; i < numberOfPatchs; i++)
        {
            int amount = Random.Range(minPatchSize, maxPatchSize + 1);

            Queue<Vector3Int> toCheck = new();
            List<Vector3Int> added = new();


            Vector3Int start = new Vector3Int(Random.Range(minWidth, maxWidth), Random.Range(minWidth, maxWidth), 0);

            toCheck.Enqueue(start);
            added.Add(start);


            while (toCheck.Count > 0 && added.Count < amount)
            {
                Vector3Int current = toCheck.Dequeue();

                foreach (Vector3Int direction in directions)
                {
                    Vector3Int neighbour = current + direction;

                    if (!added.Contains(neighbour) && !toCheck.Contains(neighbour) && added.Count < amount && Random.value <= continueChance)
                    {
                        toCheck.Enqueue(neighbour);
                        added.Add(neighbour);
                    }
                }
            }

            foreach (Vector3Int position in added)
            {
                if (Random.value > holeChance)
                {
                    tilemap.SetTile(position, patchTile);
                    patchTiles.Add(position);
                }
            }
        }

        if (spawnEdgeThings)
        {
            foreach (Vector3Int position in patchTiles)
            {
                foreach (Vector3Int direction in directions)
                {
                    Vector3Int neighbour = position + direction;

                    if (!patchTiles.Contains(neighbour))
                    {
                        if (Random.value < spawnEdgeChance)
                        {
                            Vector2 newPosition = new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                            GameObject thingPrefab = BaseGenEvent.PickRandomThingSpawn(edgePrefabs);
                            GameObject go = GameObject.Instantiate(thingPrefab);

                            go.transform.position = (Vector3)neighbour + (Vector3)newPosition;

                            LevelManager.instance.currentLevel.things.Add(go);
                        }
                    }
                }
            }
        }


        if (exclusionZone)
            LevelManager.instance.currentLevel.exclusionZones.Add(new TileMapExclusionZone(tilemap));
    }

    Vector3Int[] directions = new Vector3Int[] {
        Vector3Int.up,Vector3Int.down,
        Vector3Int.left,Vector3Int.right
    };
}
