using System.Collections.Generic;
using Define;
using System;
using System.Linq;
using UnityEngine;

public class Clickable : MonoBehaviour, IClickable
{
    #region Profile

    // 프로필 관련 기능
    public string ID;
    private ClickableProfile _profile;
    
    public void InitProfile(ClickableProfile profile)
    {
        _profile = profile;
    }
    
    public ClickableProfile GetProfile()
    {
        return _profile;
    }

    #endregion

    #region Block

    // 블록 관련 기능
    public event Action OnBlockChanged;
    public Dictionary<int, EngineBlock> BlockDictionary = new();
    public List<BlockType> DefaultBlockList = new();

    public EngineController EngineController { get; private set; }

    private void Awake()
    {
        OnBlockChanged += CheckBlockDictionary;
    }

    private void CheckBlockDictionary()
    {
        var entries = BlockDictionary.Select(kvp => $"{kvp.Key}:{kvp.Value.name}");
        Debug.Log(string.Join(", ", entries));
    }

    public void InitClickable(EngineController engineController)
    {
        EngineController = engineController;
        
        for (int i = 0; i < DefaultBlockList.Count; i++)
        {
            BlockType type = DefaultBlockList[i];

            EngineBlock block = StageManager.Instance.BlockFactory.CreateBlock(type);
            if (block == null) continue;
            
            // 블록 기능 활성화
            block.InitDefaultBlock(this, SlotType.EngineSlot, i);
            engineController.EngineSlotList[i].SetBlock(block);
            
            // 블록 상태 갱신
            BlockDictionary[i] = block;
            OnBlockChanged?.Invoke();
        }
    }
    
    public (EngineBlock movedBlock, int movedBlockIndex) TryAddOrMoveOrReplaceBlock(int targetIndex, EngineBlock block)
    {
        int slotCount = EngineController.EngineSlotList.Count;

        // Target Index에 Block 없을 때
        if (!BlockDictionary.TryGetValue(targetIndex, out var existingBlock))
        {
            // Target Index에 Block 추가
            BlockDictionary[targetIndex] = block;
            OnBlockChanged?.Invoke();
            return (null, -1);
        }
        
        // Target Index에 Block 있을 때 
        int emptyIndex = -1;
        for (int i = 0; i < slotCount; i++)
        {
            // 빈 슬롯 검사
            if (BlockDictionary.ContainsKey(i)) continue;
            emptyIndex = i;
            break;
        }

        // 빈 슬롯이 있을 때
        if (emptyIndex >= 0)
        {
            // 기존 블록을 빈 슬롯으로 이동
            BlockDictionary[emptyIndex] = existingBlock;
            BlockDictionary.Remove(targetIndex);
            
            // Target Index에 Block 추가
            BlockDictionary[targetIndex] = block;
            OnBlockChanged?.Invoke();
            return (existingBlock, emptyIndex);
        }

        // Target Index에 Block 추가
        BlockDictionary[targetIndex] = block;
        OnBlockChanged?.Invoke();
        return (existingBlock, -1);
    }
    
    public void RemoveBlock(int index)
    {
        if (BlockDictionary.ContainsKey(index))
        {
            BlockDictionary.Remove(index);
            OnBlockChanged?.Invoke();
        }
    }

    #endregion
    
    public void OnClicked()
    {
        // 클릭 시 Engine UI 활성화
        if (GetComponent<PlayerManager>() != null) return;
        
        StageBaseManager.Instance.EngineManager.ActivateEngineUI(this);
    }
}
