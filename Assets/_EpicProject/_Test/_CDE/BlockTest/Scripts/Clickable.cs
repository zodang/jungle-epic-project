using System.Collections.Generic;
using Define;
using UnityEngine;

public class Clickable : MonoBehaviour, IClickable
{
    public List<BlockType> BlockTypeList { get; private set; } = new List<BlockType>();
    [SerializeField] private List<BlockType> defaultBlockTypes = new List<BlockType>();

    private EngineUI _engineUI;

    private void Awake()
    {
        _engineUI = FindAnyObjectByType<EngineUI>();
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
        _engineUI.OpenInspector(this);
    }
    
    public void AddBlock(BlockType type)
    {
        // Inventory에서 Inspector에서 드롭 시
        BlockTypeList.Add(type);
        _engineUI.RefreshSlot(this);
    }
    
    public void RemoveBlock(BlockType type)
    {
        // Inspector에서 Inventory로 드롭 시
        BlockTypeList.Remove(type);
        _engineUI.RefreshSlot(this);
    }
}
