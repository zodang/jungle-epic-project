using System.Collections.Generic;
using Define;
using UnityEngine;

public class Clickable : MonoBehaviour, IClickable
{
    public List<BlockType> BlockTypeList  = new List<BlockType>();
    [SerializeField] private List<BlockType> defaultBlockTypes = new List<BlockType>();

    private PopInspectorUI _popInspectorUI;

    private void Awake()
    {
        _popInspectorUI = FindAnyObjectByType<PopInspectorUI>();
    }

    private void Start()
    {
        // Default Block 추가
        foreach (var type in defaultBlockTypes)
        {
            if (!BlockTypeList.Contains(type))
            {
                BlockTypeList.Add(type);
            }
        }
        BlockManager.ApplyBlockToTarget(this, FindAnyObjectByType<BlockFactory>());
    }

    public void OnClicked()
    {
        // 클릭 시 InspectorUI 활성화
        _popInspectorUI.OpenInspector(this);
    }
    
    public void AddBlock(BlockType type)
    {
        // Inventory에서 Inspector에서 드롭 시
        BlockTypeList.Add(type);
        _popInspectorUI.RefreshSlot(this);
    }
    
    public void RemoveBlock(BlockType type)
    {
        // Inspector에서 Inventory로 드롭 시
        BlockTypeList.Remove(type);
        _popInspectorUI.RefreshSlot(this);
    }
}
