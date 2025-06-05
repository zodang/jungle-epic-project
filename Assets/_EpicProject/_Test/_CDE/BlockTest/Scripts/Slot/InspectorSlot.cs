public class InspectorSlot : Slot
{
    public override void OnBlockDrop(DraggableBlock draggableBlock)
    {
        if (draggableBlock is BlockUI blockUI)
        {
            var clickable = FindAnyObjectByType<PopInspectorUI>().CurrentTarget;
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
