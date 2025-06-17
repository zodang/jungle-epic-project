using UnityEngine;

public class ObjectPropertyController : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private YSortOrder _ySortOrder;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _ySortOrder = GetComponent<YSortOrder>();
    }

    // 물에 잠긴 상태 (Submerged)
    public void Submerge()
    {
        _spriteRenderer.sortingLayerName = "Ground";
        _ySortOrder.enabled = false;
        _ySortOrder.forceToBottom = false; // 상태 초기화
        _spriteRenderer.sortingOrder = 5;
    }

    // 땅 위에 나타난 상태 (Emerged / 컨트롤 X)
    public void Emerge()
    {
        _spriteRenderer.sortingLayerName = "PlayerAndObjects";
        _ySortOrder.enabled = true;
        // 땅 위에 나타났으므로, '강제 맨 뒤' 모드를 켭니다.
        _ySortOrder.forceToBottom = true;
    }

    // 컨트롤 중인 상태 (Controlled)
    public void AttachController()
    {
        _spriteRenderer.sortingLayerName = "ForePlayer";
        _ySortOrder.enabled = false;
        // 컨트롤 중에는 Y-Sort를 하지 않으므로, '강제 맨 뒤' 모드는 끕니다.
        _ySortOrder.forceToBottom = false;
        _spriteRenderer.sortingOrder = 0;
    }

    // 땅 위로 복귀 (컨트롤 O -> 컨트롤 X)
    public void DetachController()
    {
        _spriteRenderer.sortingLayerName = "PlayerAndObjects";
        _ySortOrder.enabled = true;
        // 다시 땅 위에 놓였으므로, '강제 맨 뒤' 모드를 다시 켭니다.
        _ySortOrder.forceToBottom = true;
    }
}