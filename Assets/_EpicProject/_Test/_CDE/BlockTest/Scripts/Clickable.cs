using System.Collections.Generic;
using Define;
using UnityEngine;

public class Clickable : MonoBehaviour, IClickable
{
    public List<BlockType> BlockTypeList = new List<BlockType>();

    private PopInspectorUI _popInspectorUI;

    private void Awake()
    {
        _popInspectorUI = FindAnyObjectByType<PopInspectorUI>();
    }
    
    public void OnClicked()
    {
        _popInspectorUI.OpenInspector(this);
    }

    /// <summary>
    /// Inventory에서 Inspector에서 드롭 시
    /// </summary>
    public void AddBlock(BlockType type)
    {
        BlockTypeList.Add(type);
        _popInspectorUI.RefreshSlot(this);
    }

    /// <summary>
    /// Inspector에서 Inventory로 드롭 시
    /// </summary>
    public void RemoveBlock(BlockType type)
    {
        BlockTypeList.Remove(type);
        _popInspectorUI.RefreshSlot(this);
    }
}
