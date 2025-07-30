using Define;
using System;
using UnityEngine;

public class DebugSlot : MonoBehaviour, ISlot
{
    public event Action<EngineBlock> OnBlockSet;

    [SerializeField] private int index = -1;
    private Clickable _targetClickable;
    public EngineBlock _currentBlock;
    private BlockContainerBase _blockContainer;

    public int GetSlotIndex() => index;
    public SlotType GetSlotType() => SlotType.DebugSlot;
    public Clickable GetTargetClickable() => _targetClickable;
    public Transform GetTransform() => transform;

    public BlockContainerBase GetBlockContainerBase() => _blockContainer;

    public void SetCurrentBlock(EngineBlock block)
    {
        if (block == null) return;
        
        SetSlotBlock(block);
        
        block.transform.SetParent(transform);
        block.transform.localPosition = Vector3.zero;
        block.SetVisualState(SlotType.DebugSlot);
        
        OnBlockSet?.Invoke(block);
    }

    public void SetTargetClickable(Clickable clickable)
    {
        _targetClickable = clickable;
    }
    
    public void SetSlotBlock(EngineBlock block)
    {
        if (block == null)
        {
            _currentBlock = null;
            return;
        }
        
        OnBlockSet?.Invoke(block);
        
        block.transform.SetParent(transform, false);
        block.transform.localPosition = Vector3.zero;
        
        block.DropEngineBlock();
        block.SetVisualState(SlotType.EngineSlot);
        
        _currentBlock = block;
    }

    public void SetBlockContainerBase(BlockContainerBase blockContainer)
    {
        _blockContainer = blockContainer;
    }

    public EngineBlock GetCurrentBlock() => _currentBlock;
    
}
