using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TileDetector
{
    public static void GetTilesInSector(Tilemap tilemap, Vector2 center, Vector2 direction, float radius, float angle, ref List<Vector3> tilesPos)
    {
        tilesPos.Clear();

        int cellRadius = Mathf.CeilToInt(radius / tilemap.cellSize.x);
        float half = angle * 0.5f;

        for (int x = -cellRadius; x <= cellRadius; x++)
        {
            for (int y = -cellRadius; y <= cellRadius; y++)
            {
                Vector3Int cellPos = tilemap.WorldToCell((Vector2)center + new Vector2(x * tilemap.cellSize.x, y * tilemap.cellSize.y));

                Vector2 cellWorldPos = tilemap.CellToWorld(cellPos) + tilemap.cellSize * 0.5f;

                if (IsPointInSector(cellWorldPos, center, direction, radius, angle))
                {
                    if (tilemap.HasTile(cellPos))
                        tilesPos.Add(cellWorldPos);
                }
            }
        }
    }

    static bool IsPointInSector(Vector2 point, Vector2 center, Vector2 direction, float radius, float angle)
    {
        Vector2 toPoint = point - center;

        if (toPoint.magnitude > radius)
            return false;

        float halfAngle = angle * 0.5f;
        float angleToPoint = Vector2.Angle(direction, toPoint);

        return angleToPoint <= halfAngle;
    }

}
