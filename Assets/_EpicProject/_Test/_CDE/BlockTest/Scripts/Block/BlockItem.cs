using Define;
using UnityEngine;

public class BlockItem : MonoBehaviour, IInteractable
{
    [SerializeField] private BlockType blockType;
    public BlockType GetBlockType() => blockType;
    
    private Inventory _inventory;
    
    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
        _inventory = FindAnyObjectByType<Inventory>();
    }
    public void Interact()
    {
        AudioManager.instance.playSfx(SfxType.Get);
        
        // Inventory에 추가
        _inventory.Collect(blockType);
        Destroy(gameObject);
    }
}
