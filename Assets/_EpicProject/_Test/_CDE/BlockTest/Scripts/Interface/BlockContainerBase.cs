using Define;
using UnityEngine;

public abstract class BlockContainerBase : MonoBehaviour
{
    protected InventorySlot InventorySlot;

    protected  void Awake()
    {
        InventorySlot = FindAnyObjectByType<InventorySlot>();
    }

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

    private void OnBlockDragEndHandler(EngineBlock block, ISlot slot)
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
            case SlotType.ToolBoxSlot:
                WhenDroppedToolBox(block, slot);
                break;
            case SlotType.None:
                WhenDroppedNone(block, slot);
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
    
    protected void WhenDroppedInventorySlot(EngineBlock block, ISlot slot)
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

        block.ChangeTargetInfo(newTarget, newFeature, -1, inventorySlot);
    }
    
    private void WhenDroppedEngineSlot(EngineBlock block, ISlot slot)
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
        newTarget.EngineController.EngineSlotList[targetIndex].SetBlockPosition(block);
        
        var (movedBlock, movedBlockIndex) = newTarget.EngineController.TryAddOrMoveOrReplaceBlock(targetIndex, block);

        // Target Index에 Block 있을 때
        if (movedBlock != null)
        {
            if (movedBlockIndex >= 0)
            {
                // 빈 슬롯으로 이동
                newTarget.EngineController.EngineSlotList[movedBlockIndex].SetBlockPosition(movedBlock);
                movedBlock.ChangeTargetInfo(newTarget, newFeature, movedBlockIndex, newTarget.EngineController.EngineSlotList[movedBlockIndex]);
            }
            else
            {
                movedBlock.Deactivate(movedBlock.RequiredFeatureType);

                // 인벤토리로 이동
                var inventory = StageBaseManager.Instance.PlayerManager.Inventory;
                inventory.AddBlock(movedBlock);
                movedBlock.InitDefaultBlock(InventorySlot.GetTargetClickable(), InventorySlot, -1);
                movedBlock.transform.SetParent(InventorySlot.transform, false);
            }
        }
        
        block.ChangeTargetInfo(newTarget, newFeature, targetIndex, newTarget.EngineController.EngineSlotList[targetIndex]);
    }

    private void WhenDroppedToolBox(EngineBlock block, ISlot slot)
    {
        // 이전 Target의 기능 비활성화
        if (block.PrevTarget != null && block. PrevFeature != null)
        {
            block.Deactivate(block.PrevFeature);
            block.PrevTarget.BlockContainerBase.RemoveBlock(block.PrevSlotIndex);
        }
        
        slot.SetBlockPosition(block);
    }
    
    private void WhenDroppedNone(EngineBlock block, ISlot slot)
    {
        if (slot == null)
        {
            block.transform.SetParent(block.CurrentSlot.GetTransform(), false);
            block.transform.localPosition = Vector3.zero;
            block.SetVisualState(block.CurrentSlot.GetSlotType());
            return;
        }
        
        slot.SetBlockPosition(block);
    }
}
