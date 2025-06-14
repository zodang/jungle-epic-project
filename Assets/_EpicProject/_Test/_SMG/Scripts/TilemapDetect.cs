using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


[System.Serializable]
public class TilePair
{
    public TileBase[] Original;
    public TileBase Replace;
}


public class TilemapDetect : MonoBehaviour
{
    public TilePair[] tilePairs;

    public Tilemap groundTilemap;

    public float Radius;
    public float Angle;

    public List<Vector3> testTilesPos = new List<Vector3>();

    private void Update()
    {
        if (groundTilemap.IsUnityNull()) return;

        Radius = 6.25f * transform.parent.lossyScale.x;

        GetTilesInSector(groundTilemap, transform.position, transform.up, Radius, Angle, ref testTilesPos);
        for(int i = 0; i < testTilesPos.Count; i++)
        {
            ChangeTile(groundTilemap, testTilesPos[i]);
        }
    }

    void ChangeTile(Tilemap tilemap, Vector3 pos)
    {
        if (tilemap.IsUnityNull()) return;

        Vector3Int centerCell = tilemap.WorldToCell(pos);
        TileBase tileBase = tilemap.GetTile(centerCell);

        TileBase newTile = GetReplace(tileBase);
        if (newTile != null)
        {
            tilemap.SetTile(centerCell, newTile);
        }
    }

    public TileBase GetReplace(TileBase original)
    {
        for(int i = 0; i < tilePairs.Length; i++)
        {
            TilePair tilePair = tilePairs[i];
            for(int j = 0; j < tilePair.Original.Length; j++)
            {
                if (tilePair.Original[j] == original)
                    return tilePair.Replace;
            }
        }
        return null;
    }

    bool IsPointInSector(Vector2 point, Vector2 center, Vector2 direction, float radius, float angle)
    {
        Vector2 toPoint = point - center;

        if (toPoint.magnitude > radius)
            return false;

        float halfAngle = angle * 0.5f;
        float angleToPoint = Vector2.Angle(direction, toPoint);

        return angleToPoint <= halfAngle;
    }

    public void GetTilesInSector(Tilemap tilemap, Vector2 center, Vector2 direction, float radius, float angle, ref List<Vector3> tilesPos)
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
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 center = transform.position;

        // 원의 중심
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(center, 0.1f);

        // 부채꼴의 두 번째, 세 번째 라인을 긋려고 할 direction 계산
        float half = Angle * 0.5f;
        Vector2 left = RotateVector(transform.up, -half);
        Vector2 right = RotateVector(transform.up, half);

        // 중심에서 왼/우로 한 줄
        Gizmos.DrawLine(center, center + (Vector3)left * Radius);
        Gizmos.DrawLine(center, center + (Vector3)right * Radius);

        // 원호를 한 바퀴 도는 점들로 연결해서 만들려고 할 수 있습니다.
        int segments = 20;
        Vector3[] points = new Vector3[segments + 1];
        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            float ang = -half + t * Angle;
            Vector2 dir = RotateVector(transform.up, ang);
            points[i] = (Vector2)center + dir * Radius;
        }
        Gizmos.color = Color.yellow;
        for (int i = 0; i < segments; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
    }
#endif

    private Vector2 RotateVector(Vector2 v, float degree)
    {
        float rad = degree * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);
        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
        );
    }
}
