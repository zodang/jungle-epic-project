using Define;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockFactory : MonoBehaviour
{
    [Header("Engine Block")] 
    [SerializeField] private List<EngineBlockEntry> engineBlockEntries;
    
    [Header("Inventory Block")] 
    [SerializeField] private List<InventoryBlockEntry> inventoryBlockEntries;

    [Header("Block Icon")] 
    [SerializeField] private List<BlockIconEntry> _blockIconEntries;

    private Dictionary<BlockType, EngineBlock> _engineBlockDic;
    private Dictionary<BlockType, InventoryBlock> _inventoryBlockDic;
    private Dictionary<BlockType, Sprite> _blockIconDic;
    
    private void Awake()
    {
        // Engine Block Dictionary 설정
        _engineBlockDic = new Dictionary<BlockType, EngineBlock>();
        foreach (var entry in engineBlockEntries)
        {
            if (!_engineBlockDic.ContainsKey(entry.Type))
            {
                _engineBlockDic.Add(entry.Type, entry.Prefab);
            }
        }

        // Inventory Block Dictionary 설정
        _inventoryBlockDic = new Dictionary<BlockType, InventoryBlock>();
        foreach (var entry in inventoryBlockEntries)
        {
            if (!_inventoryBlockDic.ContainsKey(entry.Type))
            {
                _inventoryBlockDic.Add(entry.Type, entry.Prefab);
            }
        }
        
        // Block Icon Dictionary 설정
        _blockIconDic = new Dictionary<BlockType, Sprite>();
        foreach (var entry in _blockIconEntries)
        {
            if (!_blockIconDic.ContainsKey(entry.Type))
            {
                _blockIconDic.Add(entry.Type, entry.Icon);
            }          
        }
    }

    public EngineBlock CreateFeatureBlock(BlockType type, Transform parent = null)
    {
        // Engine Block 생성
        if (_engineBlockDic.TryGetValue(type, out EngineBlock prefab))
        {
            EngineBlock engineBlock = Instantiate(prefab, parent);
            return engineBlock;
        }

        Debug.LogWarning($"{type}의 Engine Block 없음!");
        return null;
    }
    
    public InventoryBlock CreateBlockUI(BlockType type, Transform parent = null)
    {
        // Inventory Block 생성
        if (_inventoryBlockDic.TryGetValue(type, out InventoryBlock prefab))
        {
            InventoryBlock inventoryBlock = Instantiate(prefab, parent);
            inventoryBlock.Init(type);

            return inventoryBlock;
        }
        
        Debug.LogWarning($"{type}의 Inventory Block 없음!");
        return null;
    }

    public Sprite GetIcon(BlockType type)
    {
        return _blockIconDic.GetValueOrDefault(type);
    }
}

[Serializable]
public class EngineBlockEntry
{
    public BlockType Type;
    public EngineBlock Prefab;
}

[Serializable]
public class InventoryBlockEntry
{
    public BlockType Type;
    public InventoryBlock Prefab;
}

[Serializable]
public class BlockIconEntry
{
    public BlockType Type;
    public Sprite Icon;
}