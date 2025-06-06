public class InspectorSlot : Slot
{
    public override void OnBlockDrop(DraggableBlock draggableBlock)
    {
        if (draggableBlock is InventoryBlock blockUI)
        {
            var clickable = FindAnyObjectByType<EngineUI>().CurrentTarget;
            if (clickable == null) return;

            clickable.AddBlock(blockUI.Type);

            // UI 제거
            Destroy(blockUI.gameObject);
        }
        else
        {
            base.OnBlockDrop(draggableBlock);
        }
    }
}
