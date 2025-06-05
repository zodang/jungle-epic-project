using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float collectRadius = 3f;
    [SerializeField] private LayerMask blockItemLayer;
    
    private Inventory _inventory;

    private void Start()
    {
        _inventory = GetComponent<Inventory>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryCollectNearestBlockItem();
        }
    }

    private void TryCollectNearestBlockItem()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, collectRadius);
        
        foreach (var hit in hits)
        {
            var blockItem = hit.GetComponent<BlockItem>();

            if (blockItem != null)
            {
                // Inventory에 추가
                _inventory.Collect(blockItem.GetBlockType());
                Destroy(blockItem.gameObject);
                break;
            }
        }
    }
}
