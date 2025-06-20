using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[System.Serializable]
public class NoiseGroundGenEvent : BaseGenEvent
{
    public RuleTile tile;
    public GameObject tilemapPrefab;
    public bool exclusionZone;

    [Range(0f, 1f)] public float coverage;
    public float noiseScale;


    public override void Generate(LevelData level)
    {
        GameObject newTileMap = GameObject.Instantiate(tilemapPrefab, GameObject.FindWithTag("TileMap").transform);
        Tilemap tilemap = newTileMap.GetComponent<Tilemap>();
        tilemap.GetComponent<TilemapRenderer>().sortingOrder = GameObject.FindWithTag("TileMap").transform.childCount;

        float offsetX = Random.Range(0, 1000f);
        float offsetY = Random.Range(0, 1000f);

        for (int x = -10; x < 110; x++)
        {
            for (int y = -10; y < 110; y++)
            {
                float noiseX = x * noiseScale;
                float noiseY = y * noiseScale;
                float noise = Mathf.PerlinNoise(noiseX + offsetX, noiseY + offsetY);

                if (noise < coverage)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        }


        if (exclusionZone)
            LevelManager.instance.currentLevel.exclusionZones.Add(new TileMapExclusionZone(tilemap));
    }
}
