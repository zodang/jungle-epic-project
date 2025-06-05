public class InventorySlot : Slot
{
    public override void OnBlockDrop(DraggableBlock draggableBlock)
    {
        if (draggableBlock is FeatureBlock featureBlock)
        {
            var clickable = FindAnyObjectByType<PopInspectorUI>().CurrentTarget;
            if (clickable == null) return;

            // 상태 제거
            clickable.RemoveBlock(featureBlock.Type);

            // UI 제거
            Destroy(featureBlock.gameObject);

            // BlockUI 생성
            FindAnyObjectByType<Inventory>().AddBlock(featureBlock.Type);

            // 슬롯 상태 갱신
            base.OnBlockRemoved();
        }
        else
        {
            base.OnBlockDrop(draggableBlock);
        }
    }
}
