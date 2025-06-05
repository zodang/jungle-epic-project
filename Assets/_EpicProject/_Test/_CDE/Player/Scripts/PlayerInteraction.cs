using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    private float _collectRange = 1.5f;
    
    private void Start()
    {
        InputManager.Instance.OnInteract += TryCollectBlock;
    }

    private void TryCollectBlock()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, _collectRange, interactableLayer);
        var interactable = hit.GetComponent<IInteractable>();
        
        if (interactable == null) return;
        
        // 상호작용 작동
        interactable?.Interact();
    }

    private void OnDestroy()
    {
        InputManager.Instance.OnInteract -= TryCollectBlock;
    }
}
