public class InventorySlot : Slot
{
    public override void OnBlockDrop(DraggableBlock draggableBlock)
    {
        if (draggableBlock is FeatureBlock featureBlock)
        {
            var clickable = FindAnyObjectByType<EngineUI>().CurrentTarget;
            if (clickable == null) return;

            // clickable의 리스트에서 제거 및 비활성화
            clickable.RemoveBlock(featureBlock.Type);
            featureBlock.Deactivate(featureBlock);

            // Feature Block 제거 및 Block UI 생성
            Destroy(featureBlock.gameObject);
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
