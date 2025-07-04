using System.Collections.Generic;
using Define;
using System;
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
    public List<BlockType> DefaultBlockList = new ();

    public EngineController EngineController { get; private set; }

    public void InitClickable(EngineController engineController)
    {
        EngineController = engineController;
        
        for (int i = 0; i < DefaultBlockList.Count; i++)
        {
            BlockType type = DefaultBlockList[i];

            EngineBlock block = StageManager.Instance.BlockFactory.CreateBlock(type);
            if (block == null) continue;

            if (i >= engineController.NumpadList.Count) continue;
            
            // 블록 상태 갱신
            BlockDictionary[i] = block;
            
            // 블록 기능 활성화
            block.InitDefaultBlock(this);

            // UI 표시
            engineController.ShowBlock(i);
        }
        
        EngineController.ChangeAllNumpadVisual();
    }
    
    public (bool canAdd, int index) TryAddBlock(EngineBlock block)
    {
        int slotCount = EngineController.NumpadList.Count;

        // Selected Index가 비어있을 때
        if (!BlockDictionary.ContainsKey(EngineController.SelectedIndex))
        {
            BlockDictionary[EngineController.SelectedIndex] = block;
            OnBlockChanged?.Invoke();
            return (true, EngineController.SelectedIndex);
        }

        // Selected Index가 채워져있을 때
        for (int i = 0; i < slotCount; i++)
        {
            if (!BlockDictionary.ContainsKey(i))
            {
                BlockDictionary[i] = block;
                OnBlockChanged?.Invoke();
                return (true, i);
            }
        }

        // 모든 칸이 채워져있을 때
        return (false, -1);
    }
    
    public (bool canAdd, int index) TryAddBlock(int preferredIndex, EngineBlock block)
    {
        int slotCount = EngineController.NumpadList.Count;

        // Preferred Index가 비어있을 때
        if (!BlockDictionary.ContainsKey(preferredIndex))
        {
            BlockDictionary[preferredIndex] = block;
            OnBlockChanged?.Invoke();
            return (true, preferredIndex);
        }

        // Preferred Index가 채워져있을 때
        for (int i = 0; i < slotCount; i++)
        {
            if (!BlockDictionary.ContainsKey(i))
            {
                BlockDictionary[i] = block;
                OnBlockChanged?.Invoke();
                return (true, i);
            }
        }

        // 모든 칸이 채워져있을 때
        return (false, -1);
    }
    
    public void RemoveBlock(int index = -1)
    {
        // 지정된 index 값 없을 시 Selected Index 제거
        if (index == -1) index = EngineController.SelectedIndex;
     
        BlockDictionary.Remove(index);
        OnBlockChanged?.Invoke();
    }

    #endregion
    
    public void OnClicked()
    {
        // 클릭 시 Engine UI 활성화
        if (GetComponent<PlayerManager>() != null) return;
        StageBaseManager.Instance.EngineManager.ActivateEngineUI(this);
    }
}
