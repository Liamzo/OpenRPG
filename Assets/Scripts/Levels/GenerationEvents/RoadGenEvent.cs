using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class RoadGenEvent : BaseGenEvent
{
    public RuleTile roadTile;
    public GameObject tilemapPrefab;

    public int minAmount;
    public int maxAmount;
    public int minWidth;
    public int maxWidth;
    
    public override void Generate(LevelData level)
    {
        GameObject newTileMap = GameObject.Instantiate(tilemapPrefab, GameObject.FindWithTag("TileMap").transform);
        Tilemap tilemap = newTileMap.GetComponent<Tilemap>();
        tilemap.GetComponent<TilemapRenderer>().sortingOrder = GameObject.FindWithTag("TileMap").transform.childCount;

        int amount = Random.Range(minAmount, maxAmount+1);
        for (int i = 0; i < amount; i++)
        {
            int width = Random.Range(minWidth, maxWidth+1);

            bool horizontal = Random.Range(0,2) > 0.5f;
            
            int startingPos = Random.Range(0, 100-width);

            if (horizontal) {
                for (int y = startingPos; y < startingPos + width; y++)
                {
                    for (int x = -10; x < 110; x++)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), roadTile);
                    }
                }
            } else {
                for (int x = startingPos; x < startingPos + width; x++)
                {
                    for (int y = -10; y < 110; y++)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), roadTile);
                    }
                }
            }
        }
    }
}
