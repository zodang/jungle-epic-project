using Define;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleSlot : BlockContainerBase
{
    [SerializeField] private BlockType blockType;
    [SerializeField] private Clickable targetClickable;

    private EngineBlock _block;

    private void Start()
    {
        InitBlock();
    }

    protected override void RemoveBlock(int index)
    {
        Debug.Log("@@DE : Remove Block");
    }

    private void InitBlock()
    {
        _block = StageManager.Instance.BlockFactory.CreateBlock(blockType, transform);
        RegisterBlockEvents(_block);
        _block.InitDefaultBlock(targetClickable, SlotType.SimpleSlot);
        
        // 블록 상호작용 비활성화
        _block.SetInteraction(false);
        _block.SetVisualState(SlotType.InventorySlot);
    }

    public void AddEvents()
    {
        // 블록 상호작용 활성화
        _block.SetInteraction(true);
        _block.AddComponent<BlockCanvasConverter>();
        _block.SetVisualState(SlotType.SimpleSlot);
    }
}
