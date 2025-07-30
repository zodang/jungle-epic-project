using Define;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleSlot : BlockContainerBase, ISlot
{
    [SerializeField] private BlockType blockType;
    [SerializeField] private Clickable targetClickable;
    
    public event Action<EngineBlock> OnBlockSet;

    [SerializeField] private int index = -1;
    private Clickable _targetClickable;
    private EngineBlock _currentBlock;
    private BlockContainerBase _blockContainer;
    
    public int GetSlotIndex() => index;
    public SlotType GetSlotType() => SlotType.SimpleSlot;
    public Clickable GetTargetClickable() => _targetClickable;
    public Transform GetTransform() => transform;
    public BlockContainerBase GetBlockContainerBase() => _blockContainer;
    
    public void SetCurrentBlock(EngineBlock block)
    {
        OnBlockSet?.Invoke(block);
        
        block.transform.SetParent(transform);
        block.transform.localPosition = Vector3.zero;
        block.SetVisualState(SlotType.SimpleSlot);
    }
    
    public void SetTargetClickable(Clickable clickable)
    {
        _targetClickable = clickable;
    }

    public void SetBlockContainerBase(BlockContainerBase blockContainer)
    {
        _blockContainer = blockContainer;
    }

    private void Start()
    {
        // Slot Setting
        SetTargetClickable(targetClickable);
        SetBlockContainerBase(this);
        
        // Block Setting
        InitBlock();
    }

    private void InitBlock()
    {
        _currentBlock = StageManager.Instance.BlockFactory.CreateBlock(blockType, transform);
        RegisterBlockEvents(_currentBlock);
        _currentBlock.InitDefaultBlock(targetClickable, this);
        
        // 블록 상호작용 비활성화
        _currentBlock.SetInteraction(false,false, false);
        _currentBlock.SetVisualState(SlotType.InventorySlot);
    }

    public void AddEvents()
    {
        // 블록 상호작용 활성화
        _currentBlock.SetInteraction(false,false,true);
        _currentBlock.AddComponent<BlockCanvasConverter>();
        _currentBlock.SetVisualState(SlotType.SimpleSlot);
    }

    protected override void WhenBlockDropped(EngineBlock block, ISlot slot)
    {
        // @@ 다른 slot으로 장착 / None으로 이동
        // Debug.Log("@@DE : Drop Block");

    }

    public override void RemoveBlock(int index)
    {
        // Debug.Log("@@DE : Remove Block");
    }
    
    protected override void WhenLeftClicked(EngineBlock block)
    {
        // 기능 X
    }
    
    protected override void WhenRightClicked(EngineBlock block)
    {
        // Debug.Log("@@DE : Right Clicked");
        // engine slot으로 이동
    }
}
