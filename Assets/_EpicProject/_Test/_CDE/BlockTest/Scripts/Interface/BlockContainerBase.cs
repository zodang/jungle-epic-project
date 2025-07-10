using Define;
using UnityEngine;

public abstract class BlockContainerBase : MonoBehaviour
{
    protected abstract void RemoveBlock(int index);
    
    protected void RegisterBlockEvents(EngineBlock block)
    {
        UnregisterBlockEvents(block);
        
        block.OnBlockDragEnd += OnBlockDragEndHandler;
        block.OnBlockLeftClick += OnBlockLeftClickHandler;
        block.OnBlockRightClick += OnBlockRightClickHandler;
    }

    private void UnregisterBlockEvents(EngineBlock block)
    {
        block.OnBlockDragEnd -= OnBlockDragEndHandler;
        block.OnBlockLeftClick -= OnBlockLeftClickHandler;
        block.OnBlockRightClick -= OnBlockRightClickHandler;
    }

    protected void UnregisterClickEvents(EngineBlock block)
    {
        // 클릭 이벤트 제외
        block.OnBlockLeftClick -= OnBlockLeftClickHandler;
        block.OnBlockRightClick -= OnBlockRightClickHandler; 
    }

    private void OnBlockDragEndHandler(EngineBlock block, ISlotType slot)
    {
        SlotType slotType = slot?.GetSlotType() ?? SlotType.None;

        switch (slotType)
        {
            case SlotType.InventorySlot:
                WhenDroppedInventorySlot(block, slot);
                break;
            case SlotType.EngineSlot:
                WhenDroppedEngineSlot(block, slot);
                break;
            case SlotType.None:
                WhenDroppedNone(block);
                break;
        }
    }

    private void OnBlockLeftClickHandler(EngineBlock block)
    {
        block.ToggleEngineBlock();
    }

    private void OnBlockRightClickHandler(EngineBlock block)
    {
        if (block.PrevTarget.EngineController == null) return;
        block.PrevTarget.EngineController.DropToInventorySlot(block);
    }
    
    protected void WhenDroppedInventorySlot(EngineBlock block,ISlotType slot)
    {
        InventorySlot inventorySlot = slot as InventorySlot;
        Clickable newTarget = slot.GetTargetClickable();
        
        // 이전 Target의 기능 비활성화
        if (block.PrevTarget != null && block. PrevFeature != null)
        {
            block.Deactivate(block.PrevFeature);
            block.PrevTarget.BlockContainerBase.RemoveBlock(block.PrevSlotIndex);
        }
        
        object newFeature = newTarget.GetComponent(block.RequiredFeatureType);
        if (newFeature == null) return;

        // Inventory에 Block 추가
        Inventory inventory = inventorySlot.GetInventory(); 
        inventory.AddBlock(block);
        
        // 새로운 Target의 기능 활성화
        block.Activate(newFeature);
        inventorySlot.SetBlockPositionToInventory(block);

        block.ChangeTargetInfo(newTarget, newFeature, -1, SlotType.InventorySlot);
    }
    
    private void WhenDroppedEngineSlot(EngineBlock block, ISlotType slot)
    {
        EngineSlot engineSlot = slot as EngineSlot;
        int targetIndex = engineSlot.Index;
        
        // Prev Target의 기능 비활성화
        if (block.PrevTarget != null && block.PrevFeature != null)
        {
            block.PrevTarget.BlockContainerBase.RemoveBlock(block.PrevSlotIndex);
            block.Deactivate(block.PrevFeature);
        }
        
        // New Target 기능 활성화
        Clickable newTarget = slot.GetTargetClickable();
        object newFeature = newTarget.GetComponent(block.RequiredFeatureType);
        
        block.Activate(newFeature);
        newTarget.EngineController.EngineSlotList[targetIndex].SetBlock(block);
        
        var (movedBlock, movedBlockIndex) = newTarget.EngineController.TryAddOrMoveOrReplaceBlock(targetIndex, block);

        // Target Index에 Block 있을 때
        if (movedBlock != null)
        {
            if (movedBlockIndex >= 0)
            {
                // 빈 슬롯으로 이동
                newTarget.EngineController.EngineSlotList[movedBlockIndex].SetBlock(movedBlock);
            }
            else
            {
                movedBlock.Deactivate(movedBlock.RequiredFeatureType);

                // 인벤토리로 이동
                var inventory = StageBaseManager.Instance.PlayerManager.Inventory;
                inventory.AddBlock(movedBlock);
                movedBlock.InitDefaultBlock(FindAnyObjectByType<InventorySlot>().GetTargetClickable(), SlotType.InventorySlot, -1);
                movedBlock.transform.SetParent(FindAnyObjectByType<InventorySlot>().transform, false);
            }
            movedBlock.ChangeTargetInfo(newTarget, newFeature, movedBlockIndex, SlotType.EngineSlot);
        }
        block.ChangeTargetInfo(newTarget, newFeature, targetIndex, SlotType.EngineSlot);
    }
    
    private void WhenDroppedNone(EngineBlock block)
    {
        if (block.CurrentSlotType == SlotType.EngineSlot)
        {
            var engineSlot = block.PrevTarget.EngineController.EngineSlotList[block.PrevSlotIndex];
            engineSlot.SetBlock(block);
            block.SetVisualState(SlotType.EngineSlot);
        }
        else if (block.SimpleSlot != null)
        {
            // SimpleSlot을 사용하는 Block일 시 SimpleSlot으로 복귀
            block.transform.SetParent(block.SimpleSlot.transform, false);
            block.transform.localPosition = Vector3.zero;
            block.SetVisualState(SlotType.SimpleSlot);

        }
        else
        {
            block.transform.SetParent(block.InventorySlot.transform, false);
            block.transform.localPosition = Vector3.zero;
            block.SetVisualState(SlotType.InventorySlot);
        }
    }
}
