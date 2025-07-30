using Define;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ToolBoxSlot : BlockContainerBase, ISlot
{
    [SerializeField] private Clickable tempTarget;
    [SerializeField] private List<BlockType> defaultBlockList;
    
    public event Action<EngineBlock> OnBlockSet;

    [SerializeField] private int index = -1;
    private Clickable _targetClickable;
    private EngineBlock _currentBlock;
    private BlockContainerBase _blockContainer;

    private DebugSlot _debugSlot;

    private void Start()
    {
        SetTargetClickable(tempTarget);
        SetBlockContainerBase(this);
        
        for (int i = 0; i < defaultBlockList.Count; i++)
        {
            BlockType type = defaultBlockList[i];

            EngineBlock block = StageManager.Instance.BlockFactory.CreateBlock(type, transform);
            RegisterBlockEvents(block);
            
            block.InitDefaultBlock(tempTarget, this);
            block.SetInteraction(true, true, true);
        }
    }

    public void TryAddBlock(EngineBlock block)
    {
        WhenBlockDropped(block, this);
    }

    protected override void WhenBlockDropped(EngineBlock block, ISlot slot)
    {
        // 이전 Target의 기능 비활성화
        if (block.CurrentTarget != null && block. CurrentFeature != null)
        {
            block.Deactivate(block.CurrentFeature);
            block.CurrentTarget.BlockContainerBase.RemoveBlock(block.CurrentSlotIndex);
        }
        
        slot.SetCurrentBlock(block);
    }
    
    public override void RemoveBlock(int index)
    {
    }
    
    protected override void WhenLeftClicked(EngineBlock block)
    {
        // 기능 X
    }
    
    protected override void WhenRightClicked(EngineBlock block)
    {
        if (FindAnyObjectByType<DebugSlot>().GetCurrentBlock() != null) return;
        
        EngineController activeEngine = null;
        foreach (EngineController engineController in FindObjectsByType<EngineController>(FindObjectsSortMode.None))
        {
            if (engineController.IsActivate) activeEngine = engineController;
        }

        if (activeEngine == null) return;

        activeEngine.TryAddBlock(block);
    }

    #region ISlot

    public int GetSlotIndex() => index;
    public SlotType GetSlotType() => SlotType.ToolBoxSlot;

    public Clickable GetTargetClickable() => _targetClickable;
    public Transform GetTransform() => transform;

    public BlockContainerBase GetBlockContainerBase() => _blockContainer;

    public void SetCurrentBlock(EngineBlock block)
    {
        block.transform.SetParent(transform);
        block.transform.localPosition = Vector3.zero;
        block.SetVisualState(SlotType.ToolBoxSlot);

        OnBlockSet?.Invoke(block);
    }

    public void SetTargetClickable(Clickable clickable)
    {
        _targetClickable = clickable;
    }

    public void SetBlockContainerBase(BlockContainerBase blockContainer)
    {
        _blockContainer = blockContainer;
    }

    #endregion
}
