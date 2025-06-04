using UnityEngine;

public class InspectorSlot : Slot
{
    private ClickableController _clickable;

    public void SetSlot(ClickableController clickable)
    {
        _clickable = clickable;
    }
    public override void OnBlockDrop(DraggableBlock draggableBlock)
    {
        base.OnBlockDrop(draggableBlock);
        Block block = draggableBlock.GetComponent<Block>();
        
        // TODO
        // 1. Inventory에서 블록 제거
        
        // 2. Clickable에 블록 추가
        _clickable.AddBlock(block);  
    }

    public override void OnBlockRemoved()
    {
        base.OnBlockRemoved();
    }
}
