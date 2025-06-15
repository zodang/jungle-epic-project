using System.Collections.Generic;
using Define;
using UnityEngine;

public class Clickable : MonoBehaviour, IClickable
{
    // 저장할 Profile 데이터
    public string ID;
    private ClickableProfile _profile;
    
    public List<BlockType> BlockTypeList { get; private set; } = new List<BlockType>();
    [SerializeField] private List<BlockType> defaultBlockTypes = new List<BlockType>();

    public void InitProfile(ClickableProfile profile)
    {
        _profile = profile;
    }

    public void InitDefaultBlock()
    {
        // Default Block 추가
        foreach (var type in defaultBlockTypes)
        {
            if (!BlockTypeList.Contains(type))
            {
                BlockTypeList.Add(type);
            }
        }
    }

    public void OnClicked()
    {
        // 클릭 시 InspectorUI 활성화
        StageBaseManager.Instance.EngineManager.ActivateEngineUI(this);
    }
    
    public void AddBlockToClickable(BlockType type)
    {
        // Inventory에서 Engine으로 드롭 시
        BlockTypeList.Add(type);
        StageBaseManager.Instance.EngineManager.NotifyBlockChanged(this);
    }
    
    public void RemoveBlockFromClickable(BlockType type)
    {
        // Engine에서 Inventory로 드롭 시
        BlockTypeList.Remove(type);
        StageBaseManager.Instance.EngineManager.NotifyBlockChanged(this);
    }

    public ClickableProfile GetProfile()
    {
        return _profile;
    }
}
