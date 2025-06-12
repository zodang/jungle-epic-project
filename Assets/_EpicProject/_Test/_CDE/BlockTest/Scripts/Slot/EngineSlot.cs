public class EngineSlot : Slot
{
    private EngineController _engineController;

    private void Awake()
    {
        _engineController = GetComponentInParent<EngineController>();
    }

    public override void OnBlockDrop(DraggableBlock draggableBlock, Slot endSlot)
    {
        if (draggableBlock is InventoryBlock inventoryBlock)
        {
            DropInventoryToEngine(inventoryBlock);
            return;
        }
        
        if (draggableBlock is EngineBlock engineBlock)
        {
            DropEngineToEngine(engineBlock, endSlot);
            return;
        }
        
        base.OnBlockDrop(draggableBlock, endSlot);
    }

    private void DropInventoryToEngine(InventoryBlock inventoryBlock)
    {
        // 인벤토리 → 엔진
        var clickable = _engineController.CurrentTarget;
        if (clickable == null) return;

        // Clickable에 Block 추가 후 UI 삭제
        clickable.AddBlockToClickable(inventoryBlock.Type);
        Destroy(inventoryBlock.gameObject);
    }
    
    private void DropEngineToEngine(EngineBlock engineBlock, Slot endSlot)
    {
        if (engineBlock.PrevSlot == null)
        {
            // 기본 블록
            base.OnBlockDrop(engineBlock, endSlot);
            return;
        }
            
        var prevController = engineBlock.PrevSlot.GetComponentInParent<EngineController>();
        
        var prevClickable = prevController?.CurrentTarget;
        var thisClickable = _engineController.CurrentTarget;
        
        if (prevClickable == thisClickable)
        {
            // 엔진 → 같은 엔진
            base.OnBlockDrop(engineBlock, endSlot);
            return;
        }
            
        // 엔진 → 다른 엔진
        prevClickable.RemoveBlockFromClickable(engineBlock.Type);
        engineBlock.Deactivate(engineBlock);
                
        thisClickable.AddBlockToClickable(engineBlock.Type);
    }
    
}
