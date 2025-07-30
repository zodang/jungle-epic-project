using System.Collections.Generic;
using Define;
using System;
using System.Linq;
using UnityEngine;

public class Inventory : BlockContainerBase
{
    private Clickable _currentTarget;
    private InventoryUIController _inventoryUI;

    public Dictionary<int, EngineBlock> BlockDictionary = new();
    public List<BlockType> DefaultBlockList = new();

    private List<InventorySlot> _slotList = new List<InventorySlot>();
    
    public bool StartGetPlayerControl = true;
    
    public event Action OnBlockChanged;
    
    protected void Awake()
    {
        _inventoryUI = FindAnyObjectByType<InventoryUIController>();
        _slotList = new List<InventorySlot>(FindAnyObjectByType<InventoryUIController>().GetComponentsInChildren<InventorySlot>());
        
        if (StartGetPlayerControl)
        {
            DefaultBlockList.Add(BlockType.PlayerControl);
        }
    }

    private void Start()
    {
        _currentTarget = StageBaseManager.Instance.PlayerManager.GetComponent<Clickable>();
        _inventoryUI.OnResetBtnClicked += ResetFeature;

        // Slot Setting
        foreach (var slot in _slotList)
        {
            slot.SetTargetClickable(_currentTarget);
            slot.SetBlockContainerBase(this);
        }

        // Inventory Setting
        SetDefaultBlock();
        
        // OnBlockChanged += CheckBlockDictionary;
    }

    private void OnDestroy()
    {
        _inventoryUI.OnResetBtnClicked -= ResetFeature;
    }

    private void CheckBlockDictionary()
    {
        var entries = BlockDictionary.Select(kvp => $"{kvp.Key}:{kvp.Value.name}");
        string values = string.Join(", ", entries); 
        Debug.Log($"@@DE: {_currentTarget.name} : {values}");
    }
    
    private void SetDefaultBlock()
    {
        foreach (var type in DefaultBlockList)
        {

            EngineBlock newBlock = SpawnBlock(type);
            TryAddBlock(newBlock);
        }
    }

    public EngineBlock SpawnBlock(BlockType type)
    {
        // 블록 생성
        var factory = StageManager.Instance.BlockFactory;
        EngineBlock block = factory.CreateBlock(type);
        RegisterBlockEvents(block);

        return block;
    }

    public bool TryAddBlock(EngineBlock block)
    {
        int emptyIndex = -1;
        
        for (int i = 0; i < _slotList.Count; i++)
        {
            if (!BlockDictionary.ContainsKey(i))
            {
                emptyIndex = i;
                break;
            }
        }

        // 빈 슬롯 없을 시 중단
        if (emptyIndex == -1) return false;

        WhenBlockDropped(block, _slotList[emptyIndex]);
        return true;
    }

    protected override void WhenBlockDropped(EngineBlock block, ISlot slot)
    {
        if (slot.GetSlotType() != SlotType.InventorySlot) return;
        
        // 이전 Target의 기능 비활성화
        if (block.CurrentTarget != null && block. CurrentFeature != null)
        {
            block.Deactivate(block.CurrentFeature);
            block.CurrentTarget.BlockContainerBase.RemoveBlock(block.CurrentSlotIndex);
        }
        
        // New Target 기능 활성화
        Clickable newTarget = slot.GetTargetClickable();
        object newFeature = newTarget.GetComponent(block.RequiredFeatureType);
        int targetIndex = slot.GetSlotIndex();
        
        var (movedBlock, movedBlockIndex) = TryAddOrMoveOrReplaceBlock(block, targetIndex);

        // movedBlock 처리
        if (movedBlock != null)
        {
            if (movedBlockIndex >= 0)
            {
                // 빈 슬롯으로 이동
                _slotList[movedBlockIndex].SetCurrentBlock(movedBlock);
                movedBlock.ChangeTargetInfo(newTarget, newFeature, movedBlockIndex, _slotList[movedBlockIndex]);
                movedBlock.transform.SetParent(_slotList[movedBlockIndex].transform, false);
            }
            else
            {
                // 기존 슬롯으로 이동
                ISlot prevSlot = block.CurrentSlot;
                Clickable prevTarget = block.CurrentTarget;
                object prevFeature = block.GetComponent(block.RequiredFeatureType);

                prevSlot.SetCurrentBlock(movedBlock);
                movedBlock.ChangeTargetInfo(prevTarget, prevFeature, prevSlot.GetSlotIndex(), prevSlot);
            }
        }

        // block 정보 갱신
        block.ChangeTargetInfo(newTarget, newFeature, targetIndex, slot);
        block.Activate(newFeature);
        
        slot.SetCurrentBlock(block);
    }

    public override void RemoveBlock(int index)
    {
        if (!BlockDictionary.ContainsKey(index)) return;
        
        BlockDictionary.Remove(index);
        _slotList[index].SetCurrentBlock(null);
        
        OnBlockChanged?.Invoke();
    }
    
    private (EngineBlock movedBlock, int movedBlockIndex) TryAddOrMoveOrReplaceBlock(EngineBlock block, int targetIndex)
    {
        int slotCount = _slotList.Count;
        bool isSameContainer = block.CurrentTarget == _currentTarget;

        if (!BlockDictionary.TryGetValue(targetIndex, out var existingBlock))
        {
            BlockDictionary[targetIndex] = block;
            OnBlockChanged?.Invoke();
            return (null, -1);
        }

        // 같은 슬롯 내 이동 시 Swap
        if (isSameContainer)
        {
            int originalIndex = block.CurrentSlotIndex;
            if (originalIndex >= 0 && originalIndex != targetIndex)
            {
                // swap 위치
                BlockDictionary[originalIndex] = existingBlock;
                BlockDictionary[targetIndex] = block;
                OnBlockChanged?.Invoke();
                return (existingBlock, originalIndex);
            }
        }

        // 다른 슬롯 내 이동 시 빈 슬롯 탐색
        int emptyIndex = -1;
        for (int i = 0; i < slotCount; i++)
        {
            if (!BlockDictionary.ContainsKey(i))
            {
                emptyIndex = i;
                break;
            }
        }

        if (emptyIndex >= 0)
        {
            BlockDictionary[emptyIndex] = existingBlock;
            BlockDictionary[targetIndex] = block;
            OnBlockChanged?.Invoke();
            return (existingBlock, emptyIndex);  // 기존 블록은 빈 슬롯으로 이동
        }

        // 빈 슬롯이 없을 때
        BlockDictionary[targetIndex] = block;
        OnBlockChanged?.Invoke();
        return (existingBlock, -1);
    }
    
    private void ResetFeature()
    {
        if (_currentTarget == null) return;

        // Clickable의 기능 초기화
        IFeatureResetable resettable = _currentTarget.GetComponent<IFeatureResetable>();
        resettable?.ResetFeature();
        
        // Block의 UI 초기화
        EngineBlock[] blocks = _inventoryUI.GetComponentsInChildren<EngineBlock>(includeInactive: true);
        foreach (EngineBlock block in blocks)
        {
            block.ResetUI();
        }
    }

    protected override void WhenLeftClicked(EngineBlock block)
    {
        block.ToggleEngineBlock();
    }
    
    protected override void WhenRightClicked(EngineBlock block)
    {
        EngineController activeEngine = null;
        foreach (EngineController engineController in FindObjectsByType<EngineController>(FindObjectsSortMode.None))
        {
            if (engineController.IsActivate) activeEngine = engineController;
        }

        if (activeEngine == null) return;
        activeEngine.TryAddBlock(block);
    }

    private void Update()
    {
        for (int i = 1; i <= 4; i++)
        {
            KeyCode key = KeyCode.Alpha0 + i;
            int index = i - 1;

            if (Input.GetKeyDown(key))
            {
                if (BlockDictionary.TryGetValue(index, out var block))
                {
                    block.RaiseEngineBlock();
                }
            }
            else if (Input.GetKeyUp(key))
            {
                if (BlockDictionary.TryGetValue(index, out var block))
                {
                    block.DropEngineBlock();
                }
            }
        }
    }
}
