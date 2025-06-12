using UnityEngine;

public class InventorySlot : Slot
{
    public override void OnBlockDrop(DraggableBlock draggableBlock, Slot endSlot)
    {
        // 엔진 → 인벤토리  
        if (draggableBlock is EngineBlock engineBlock)
        {
            DropEngineToInventory(engineBlock);
            return;
        }
        
        base.OnBlockDrop(draggableBlock, endSlot);
    }

    private void DropEngineToInventory(EngineBlock engineBlock)
    {
        Debug.Log("@@DE --> 엔진 \u2192 인벤토리");

        if (engineBlock.PrevSlot is EngineSlot engineSlot)
        {
            EngineController engineController = engineSlot.GetComponentInParent<EngineController>();
            Clickable clickable = engineController?.CurrentTarget;
            if (clickable == null) return;

            // Clickable에서 Block 제거 후 기능 비활성화
            clickable.RemoveBlockFromClickable(engineBlock.Type);
            engineBlock.Deactivate(engineBlock);
        }

        // Inventory에 Block 추가 후 UI 삭제
        FindAnyObjectByType<Inventory>().AddBlockToInventory(engineBlock.Type);
        Destroy(engineBlock.gameObject);

        // 슬롯 상태 갱신
        base.OnBlockRemoved();
    }
}
