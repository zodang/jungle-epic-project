using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private LayerMask _interactableLayer;
    private float _collectRange = 1.5f;
    
    private void Start()
    {
        // 상호작용할 레이어 설정
        int layer = LayerMask.NameToLayer("Interactable");
        _interactableLayer = 1 << layer;
        
        // E키로 상호작용
        StageManager.Instance.InputManager.OnInteract += TryCollectBlock;
    }

    private void TryCollectBlock()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, _collectRange, _interactableLayer);
        if (hit == null) return;
        
        var interactable = hit.GetComponent<IInteractable>();
        if (interactable == null) return;
        
        // 상호작용 작동
        interactable?.Interact();
    }

    private void OnDestroy()
    {
        StageManager.Instance.InputManager.OnInteract -= TryCollectBlock;
    }
}
