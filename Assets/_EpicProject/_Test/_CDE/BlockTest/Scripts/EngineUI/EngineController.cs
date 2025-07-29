using Define;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EngineController : BlockContainerBase
{
    public bool IsActivate { get; private set; } // 창 활성화 여부 체크
    [SerializeField] private EngineActivationType activationType = EngineActivationType.LeftToRightType; 
    
    private Clickable _currentTarget;
    private EngineUIController _engineUIController;
    
    private Dictionary<int, EngineBlock> _blockDictionary = new();
    private List<BlockType> _defaultBlockList;
    
    private List<ISlot> _slotList = new List<ISlot>();

    private Inventory _inventory;
    private ToolBoxSlot _toolBox;
    public event Action OnBlockChanged;
    
    protected void Awake()
    {
        _engineUIController = GetComponent<EngineUIController>();
        _slotList = new List<ISlot>(GetComponentsInChildren<ISlot>());
    }

    private void Start()
    {
        _inventory = StageBaseManager.Instance.PlayerManager.Inventory;
        _toolBox = FindAnyObjectByType<ToolBoxSlot>();

        // Button 기능 연결
        _engineUIController.OnClickCloseBtn += Deactivate;
        _engineUIController.OnResetBtnClicked += ResetFeature;
        _engineUIController.OnTabBtnClicked += Deactivate;
        _engineUIController.OnClearBtnClicked += ClearAllBlock;
        
        // OnBlockChanged += CheckBlockDictionary;
        gameObject.SetActive(false);
    }
    
    private void CheckBlockDictionary()
    {
        var entries = _blockDictionary.Select(kvp => $"{kvp.Key}:{kvp.Value.name}");
        string values = string.Join(", ", entries); 
        Debug.Log($"@@DE: {_currentTarget.name} : {values}");
    }

    public void InitEngineController(Clickable target, List<BlockType> defaultBlockList)
    {
        _currentTarget = target;
        _defaultBlockList = defaultBlockList;
        
        // Slot의 Target Clickable 설정
        foreach (var slot in _slotList)
        {
            slot.SetTargetClickable(_currentTarget);
            slot.SetBlockContainerBase(this);
        }
        
        // Clickable의 기본 블록 세팅
        SetDefaultBlock();

        // UI 세팅
        _engineUIController.SetProfile(target.GetProfile());
    }

    private void SetDefaultBlock()
    {
        for (int i = 0; i < _defaultBlockList.Count; i++)
        {
            BlockType type = _defaultBlockList[i];

            EngineBlock block = StageManager.Instance.BlockFactory.CreateBlock(type);
            if (block == null) continue;
            
            // 블록 기능 활성화
            block.InitDefaultBlock(_currentTarget, _slotList[i], i);
            _slotList[i].SetCurrentBlock(block);
            
            // 블록 상태 갱신
            _blockDictionary[i] = block;
            OnBlockChanged?.Invoke();
            
            RegisterBlockEvents(block);
        }
    }
    
    public void Activate()
    {
        if (IsActivate) return;
        IsActivate = true;

        _engineUIController.ActivateEffect(activationType);
        GameManager.Instance.AudioManager.PlaySfx(SfxType.Open);
    }
    
    public void Deactivate()
    {
        IsActivate = false;
        if (!gameObject.activeSelf) return;
        
        _engineUIController.DeactivateEffect(activationType);
        GameManager.Instance.AudioManager.PlaySfx(SfxType.Close);
        ClearAllBlock();
    }

    public void DeactivateSilently()
    {
        IsActivate = false;
        if (!gameObject.activeSelf) return;
        
        _engineUIController.DeactivateEffect(activationType);
        ClearAllBlock();
    }

    public bool TryAddBlock(EngineBlock block)
    {
        int emptyIndex = -1;

        for (int i = 0; i < _slotList.Count; i++)
        {
            if (!_blockDictionary.ContainsKey(i))
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
        if (slot.GetSlotType() == SlotType.EngineSlot)
        {
            // Prev Target의 기능 비활성화
            if (block.CurrentTarget != null && block.CurrentFeature != null)
            {
                block.CurrentTarget.BlockContainerBase.RemoveBlock(block.CurrentSlotIndex);
                block.Deactivate(block.CurrentFeature);
            }
        
            // New Target 기능 활성화
            Clickable newTarget = slot.GetTargetClickable();
            object newFeature = newTarget.GetComponent(block.RequiredFeatureType);
            int targetIndex = slot.GetSlotIndex();
        
            var (movedBlock, movedBlockIndex) = newTarget.EngineController.TryAddOrMoveOrReplaceBlock(targetIndex, block);

            // Moved Block 관련 처리
            if (movedBlock != null)
            {
                if (movedBlockIndex >= 0)
                {
                    // 빈 슬롯으로 이동
                    newTarget.EngineController._slotList[movedBlockIndex].SetCurrentBlock(movedBlock);
                    movedBlock.ChangeTargetInfo(newTarget, newFeature, movedBlockIndex, newTarget.EngineController._slotList[movedBlockIndex]);
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
        
            block.ChangeTargetInfo(newTarget, newFeature, targetIndex, slot);
            block.Activate(newFeature);
        
            slot.SetCurrentBlock(block);
            return;
        }

        if (slot.GetSlotType() == SlotType.DebugSlot)
        {
            DropToDebugSlot(block, slot);
        }
    }
    
    private (EngineBlock movedBlock, int movedBlockIndex) TryAddOrMoveOrReplaceBlock(int targetIndex, EngineBlock block)
    {
        int slotCount = _slotList.Count;

        // Target Index에 Block 없을 때
        if (!_blockDictionary.TryGetValue(targetIndex, out var existingBlock))
        {
            // Target Index에 Block 추가
            _blockDictionary[targetIndex] = block;
            OnBlockChanged?.Invoke();
            return (null, -1);
        }
        
        // Target Index에 Block 있을 때 
        int emptyIndex = -1;
        for (int i = 0; i < slotCount; i++)
        {
            // 빈 슬롯 검사
            if (_blockDictionary.ContainsKey(i)) continue;
            emptyIndex = i;
            break;
        }

        // 빈 슬롯이 있을 때
        if (emptyIndex >= 0)
        {
            // 기존 블록을 빈 슬롯으로 이동
            _blockDictionary[emptyIndex] = existingBlock;
            _blockDictionary.Remove(targetIndex);
            
            // Target Index에 Block 추가
            _blockDictionary[targetIndex] = block;
            OnBlockChanged?.Invoke();
            return (existingBlock, emptyIndex);
        }

        // Target Index에 Block 추가
        _blockDictionary[targetIndex] = block;
        OnBlockChanged?.Invoke();
        return (existingBlock, -1);
    }

    public override void RemoveBlock(int index)
    {
        if (!_blockDictionary.ContainsKey(index)) return;
        
        _blockDictionary.Remove(index);
        _slotList[index].SetCurrentBlock(null);
        OnBlockChanged?.Invoke();
    }

    protected override void WhenLeftClicked(EngineBlock block)
    {
        // 기능 X
    }
    
    protected override void WhenRightClicked(EngineBlock block)
    {
        int index = block.CurrentSlotIndex;
        if (!_blockDictionary.ContainsKey(index)) return;
        ClearBlock(index);
    }

    private void ResetFeature()
    {
        if (_currentTarget == null) return;

        // Clickable의 기능 초기화
        IFeatureResetable resettable = _currentTarget.GetComponent<IFeatureResetable>();
        resettable?.ResetFeature();
        
        // Block의 UI 초기화
        EngineBlock[] blocks = GetComponentsInChildren<EngineBlock>(includeInactive: true);
        foreach (EngineBlock block in blocks)
        {
            block.ResetUI();
        }
    }

    private void ClearBlock(int index)
    {
        if (_inventory == null) return;
        
        bool canDrop = _inventory.TryAddBlock(_blockDictionary[index]);
        if (canDrop) RemoveBlock(index);
    }
    
    private void ClearAllBlock()
    {
        if (_inventory == null) return;

        // 인벤토리로 블록 이동
        foreach (var kvp in _blockDictionary.ToList())
        {
            int index = kvp.Key;
            EngineBlock block = kvp.Value;
    
            bool canDrop = _inventory.TryAddBlock(block);
            if (canDrop) RemoveBlock(index);
        }
    }
    
    public void DropToToolBoxSlot(EngineBlock block)
    {
        if (_toolBox == null) return;
        _toolBox.TryAddBlock(block);
    }

    private void DropToDebugSlot(EngineBlock block, ISlot slot)
    {
        DebugSlot debugSlot = slot as DebugSlot;
        if (debugSlot != null && debugSlot.GetCurrentBlock() != null)
        {
            WhenDroppedNone(block);
            return;
        }
        
        // New Target 기능 활성화
        Clickable newTarget = slot.GetTargetClickable();
        object newFeature = newTarget.GetComponent(block.RequiredFeatureType);
        
        slot.SetCurrentBlock(block);
        block.Activate(newFeature);
    }
}
