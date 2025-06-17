using System.Collections.Generic;
using Define;
using UnityEngine;

public class Clickable : MonoBehaviour, IClickable
{
    // 저장할 Profile 데이터
    public string ID;
    private ClickableProfile _profile;

    public Dictionary<int, BlockType> SlotBlockMap { get; private set; } = new();
    public List<BlockType> BlockTypeList { get; private set; } = new List<BlockType>();
    [SerializeField] private List<BlockType> defaultBlockTypes = new List<BlockType>();

    public void InitProfile(ClickableProfile profile)
    {
        _profile = profile;
    }

    public void InitDefaultBlock()
    {
        // Default Block 추가
        for (int i = 0; i < defaultBlockTypes.Count; i++)
        {
            var type = defaultBlockTypes[i];
            if (!SlotBlockMap.ContainsKey(i))
            {
                SlotBlockMap.Add(i, type);
            }
        }
    }

    public void OnClicked()
    {
        // 클릭 시 InspectorUI 활성화
        StageBaseManager.Instance.EngineManager.ActivateEngineUI(this);
    }
    
    public void AddBlockToClickable(BlockType type, int slotIndex)
    {
        // Inventory에서 Engine으로 드롭 시
        SlotBlockMap[slotIndex] = type;
        StageBaseManager.Instance.EngineManager.NotifyBlockChanged(this);
    }
    
    public void RemoveBlockFromClickable(BlockType type)
    {
        // Engine에서 Inventory로 드롭 시
        int targetKey = -1;

        foreach (var pair in SlotBlockMap)
        {
            if (pair.Value == type)
            {
                targetKey = pair.Key;
                break;
            }
        }

        if (targetKey != -1)
        {
            SlotBlockMap.Remove(targetKey);
            StageBaseManager.Instance.EngineManager.NotifyBlockChanged(this);
        }
    }

    public ClickableProfile GetProfile()
    {
        return _profile;
    }
}
