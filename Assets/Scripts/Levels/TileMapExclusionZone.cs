using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapExclusionZone : ExclusionZone
{
    public Tilemap tilemap;

    public TileMapExclusionZone(Tilemap tilemap) {
        this.tilemap = tilemap;
    }

    public override bool CheckInZone(Vector2 position, float distance = 0) {
        // Should be good enough for now just to test in 4 directions based on distance

        if (tilemap.HasTile(tilemap.WorldToCell(position))) return true;

        if (distance == 0) return false;

        for (int i = -Mathf.RoundToInt(distance); i <= distance; i++) {
            Vector3 testPostion = new Vector3(position.x + i, position.y, 0f);
            if (tilemap.HasTile(tilemap.WorldToCell(testPostion))) return true;
        }

        for (int i = -Mathf.RoundToInt(distance); i <= distance; i++) {
            Vector3 testPostion = new Vector3(position.x, position.y + i, 0f);
            if (tilemap.HasTile(tilemap.WorldToCell(testPostion))) return true;
        }

        return false;
    }
}
