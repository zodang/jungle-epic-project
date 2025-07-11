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
    public List<EngineSlot> EngineSlotList = new List<EngineSlot>();
    
    public event Action OnBlockChanged;
    
    protected void Awake()
    {
        base.Awake();
        
        _engineUIController = GetComponent<EngineUIController>();
        EngineSlotList = new List<EngineSlot>(GetComponentsInChildren<EngineSlot>());

        for (int i = 0; i < EngineSlotList.Count; i++)
        {
            EngineSlotList[i].Init(i);
        }
    }

    private void Start()
    {
        // Button 기능 연결
        _engineUIController.OnClickCloseBtn += Deactivate;
        _engineUIController.OnResetBtnClicked += ResetFeature;
        _engineUIController.OnTabBtnClicked += Deactivate;
        _engineUIController.OnClearBtnClicked += ClearBlock;
        
        // OnBlockChanged += CheckBlockDictionary;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _engineUIController.OnClickCloseBtn -= Deactivate;
        _engineUIController.OnResetBtnClicked -= ResetFeature;
        _engineUIController.OnTabBtnClicked -= Deactivate;
        _engineUIController.OnClearBtnClicked -= ClearBlock;
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
        List<ISlot> slots = new(transform.GetComponentsInChildren<ISlot>());
        foreach (var slot in slots)
        {
            slot.SetTargetClickable(_currentTarget);
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
            block.InitDefaultBlock(_currentTarget, EngineSlotList[i], i);
            EngineSlotList[i].SetBlockPosition(block);
            
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
    
    private void Deactivate()
    {
        IsActivate = false;
        if (!gameObject.activeSelf) return;
        
        _engineUIController.DeactivateEffect(activationType);
        GameManager.Instance.AudioManager.PlaySfx(SfxType.Close);
        ClearBlock();

    }

    public void DeactivateSilently()
    {
        IsActivate = false;
        if (!gameObject.activeSelf) return;
        
        _engineUIController.DeactivateEffect(activationType);
        ClearBlock();
    }
    
    public (EngineBlock movedBlock, int movedBlockIndex) TryAddOrMoveOrReplaceBlock(int targetIndex, EngineBlock block)
    {
        int slotCount = EngineSlotList.Count;

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

    protected override void RemoveBlock(int index)
    {
        if (!_blockDictionary.ContainsKey(index)) return;
        _blockDictionary.Remove(index);
        EngineSlotList[index].SetBlockPosition(null);
        OnBlockChanged?.Invoke();
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

    private void ClearBlock()
    {
        foreach (var block in _blockDictionary.ToList())
        {
            DropToInventorySlot(block.Value);
        }
    }
    
    public void DropToInventorySlot(EngineBlock block)
    {
        // 인벤토리로 블록 이동
        if (InventorySlot == null) return;
        WhenDroppedInventorySlot(block, InventorySlot);
        block.SetVisualState(SlotType.InventorySlot);
    }

    public void DisableEngineDeactivate()
    {
        _engineUIController.OnTabBtnClicked -= Deactivate;
        _engineUIController.DisableTabBtn();
    }
}
