using UnityEngine;

public enum ClickableMaskSortOrder
{
    MidGround,
    PlayerAndObject,
    ForePlayer
}

public class ClickableMask : MonoBehaviour
{
    public ClickableMaskSortOrder SortOrder;

    private const float _zPos = -4f;
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, _zPos);
        
        // 6: Clickable
        if (gameObject.layer != Define.Layers.Clickable)
            gameObject.layer = Define.Layers.Clickable;
    }
}
