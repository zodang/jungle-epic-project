using Define;
using UnityEngine;

public class BlockItem : MonoBehaviour, IInteractable
{
    public BlockType BlockType;
    public BlockType GetBlockType() => BlockType;
    
    private Inventory _inventory;
    
    private void Awake()
    {
        _inventory = FindAnyObjectByType<Inventory>();
    }
    public void Interact()
    {
        // Inventory에 추가
        _inventory.Collect(BlockType);
        Destroy(gameObject);
    }
}
