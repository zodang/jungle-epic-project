using Define;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleSlot : BlockContainerBase, ISlot
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
        _block.InitDefaultBlock(targetClickable, this);
        
        // 블록 상호작용 비활성화
        _block.SetInteraction(false,false, false);
        _block.SetVisualState(SlotType.InventorySlot);
    }

    public void AddEvents()
    {
        // 블록 상호작용 활성화
        _block.SetInteraction(false,false,true);
        _block.AddComponent<BlockCanvasConverter>();
        _block.SetVisualState(SlotType.SimpleSlot);
    }

    public SlotType GetSlotType()
    {
        return SlotType.SimpleSlot;
    }

    public void SetBlockPosition(EngineBlock block)
    {
        block.transform.SetParent(transform);
        block.transform.localPosition = Vector3.zero;
        block.SetVisualState(SlotType.SimpleSlot);
    }
    
    public Clickable GetTargetClickable()
    {
        return null;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void SetTargetClickable(Clickable clickable)
    {
        clickable = null;
    }
}
