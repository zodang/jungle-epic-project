using Define;
using System;
using UnityEngine;

public class EngineSlot : MonoBehaviour, ISlot
{
    public event Action<EngineBlock> OnBlockSet;

    [SerializeField] private int index = -1;
    private Clickable _targetClickable;
    private EngineBlock _currentBlock;
    private BlockContainerBase _blockContainer;

    public int GetSlotIndex() => index;
    public SlotType GetSlotType() => SlotType.EngineSlot; 
    public Clickable GetTargetClickable() => _targetClickable;
    public Transform GetTransform() => transform;
    public BlockContainerBase GetBlockContainerBase() => _blockContainer;

    public void SetCurrentBlock(EngineBlock block)
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
        block.OnBlockAnimation();
        block.SetVisualState(SlotType.EngineSlot);
        
        _currentBlock = block;
    }
    
    public void SetTargetClickable(Clickable clickable)
    {
        _targetClickable = clickable;
    }

    public void SetBlockContainerBase(BlockContainerBase blockContainer)
    {
        _blockContainer = blockContainer;
    }
    
    private void Awake()
    {
        if (index == -1) Debug.LogWarning("EngineSlot Index 설정 필요!");
    }

    public EngineBlock GetCurrentBlock()
    {
        return _currentBlock;
    }
}
