using UnityEngine;

public abstract class BlockContainerBase : MonoBehaviour
{
    protected abstract void WhenBlockDropped(EngineBlock block, ISlot slot);
    public abstract void RemoveBlock(int index);
    protected abstract void WhenLeftClicked(EngineBlock block);
    protected abstract void WhenRightClicked(EngineBlock block);
    
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
    
    private void OnBlockLeftClickHandler(EngineBlock block)
    {
        block.CurrentSlot.GetBlockContainerBase().WhenLeftClicked(block);
    }

    private void OnBlockRightClickHandler(EngineBlock block)
    {
        block.CurrentSlot.GetBlockContainerBase().WhenRightClicked(block);
    }
    
    private void OnBlockDragEndHandler(EngineBlock block, ISlot slot)
    {
        if (slot == null)
        {
            WhenDroppedNone(block);
            return;
        }
        
        // 각 Block Container Base에서 블록 처리
        slot.GetBlockContainerBase().WhenBlockDropped(block, slot);
    }

    protected void WhenDroppedNone(EngineBlock block)
    {
        block.transform.SetParent(block.CurrentSlot.GetTransform(), false);
        block.transform.localPosition = Vector3.zero;
        block.SetVisualState(block.CurrentSlot.GetSlotType());
    }
}
