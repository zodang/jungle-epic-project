using System.Collections.Generic;
using UnityEngine;

public static class ColliderDetector
{
    public static void GetCollidersInsector(Vector2 center, Vector2 direction, float radius, float angle, ref List<Collider2D> colliders, GameObject self)
    {
        colliders.Clear();

        Collider2D[] rangedColls = Physics2D.OverlapCircleAll(center, radius);

        for (int i = 0; i < rangedColls.Length; i++)
        {
            Collider2D col = rangedColls[i];
            if (col.gameObject == self)
            {
                continue;
            }
            
            if(IsPointInSector(col.transform.position, center, direction, radius, angle))
            {
                colliders.Add(col);
            }
        }
    }

    static bool IsPointInSector(Vector2 point, Vector2 center, Vector2 direction, float radius, float angle)
    {
        Vector2 toPoint = point - center;

        float halfAngle = angle * 0.5f;
        float angleToPoint = Vector2.Angle(direction, toPoint);

        return angleToPoint <= halfAngle;
    }
}
