using UnityEngine;
using UnityEngine.Events;

public class SwitchCollider : MonoBehaviour
{
    public Collider2D VerticalCollider2D;
    public Collider2D HorizontalCollider2D;

    private void Awake()
    {
        var colls = GetComponents<Collider2D>();
        bool firstIsVertical = colls[0].bounds.size.y >= colls[1].bounds.size.y;
        VerticalCollider2D = colls[firstIsVertical ? 0 : 1];
        HorizontalCollider2D = colls[firstIsVertical ? 1 : 0];
    }


    [ContextMenu("Test(true)")]
    void t1()
    {
        Test(true);
    }

    [ContextMenu("Test(false)")]
    void t2()
    {
        Test(false);
    }

    void Test(bool isVertical)
    {
        VerticalCollider2D.enabled = isVertical;
        HorizontalCollider2D.enabled = !isVertical;
    }
}
