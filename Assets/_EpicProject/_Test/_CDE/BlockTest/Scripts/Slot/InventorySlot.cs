using Define;
using System;
using UnityEngine;

public class InventorySlot : MonoBehaviour, ISlot
{
    public event Action<EngineBlock> OnBlockSet;

    [SerializeField] private int index = -1;
    private Clickable _targetClickable;
    private EngineBlock _currentBlock;
    private BlockContainerBase _blockContainer;

    public int GetSlotIndex() => index;

    public SlotType GetSlotType() => SlotType.InventorySlot;
    public Clickable GetTargetClickable() => _targetClickable;
    public Transform GetTransform() => transform;

    public BlockContainerBase GetBlockContainerBase() => _blockContainer;

    public void SetCurrentBlock(EngineBlock block)
    {
        if (block == null) return;
        
        OnBlockSet?.Invoke(block);
        
        block.transform.SetParent(transform);
        block.transform.localPosition = Vector3.zero;
        block.transform.localScale = Vector3.one;
        
        block.SetVisualState(SlotType.InventorySlot);
    }

    public void SetTargetClickable(Clickable clickable)
    {
        _targetClickable = clickable;
    }

    public void SetBlockContainerBase(BlockContainerBase blockContainer)
    {
        _blockContainer = blockContainer;
    }
}

