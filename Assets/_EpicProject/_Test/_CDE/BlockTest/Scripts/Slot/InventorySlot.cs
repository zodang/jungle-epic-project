using UnityEngine;

public class InventorySlot : Slot
{
    public override void OnBlockDrop(DraggableBlock draggableBlock)
    {
        base.OnBlockDrop(draggableBlock);
        Block block = draggableBlock.GetComponent<Block>();
        
        // TODO
        // 1. Clickable에 블록 제거
        
        
        // 2. Inventory에 블록 추가
    }

    public override void OnBlockRemoved()
    {
        base.OnBlockRemoved();
    }
}
