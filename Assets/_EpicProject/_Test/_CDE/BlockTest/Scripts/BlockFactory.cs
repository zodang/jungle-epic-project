using Define;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockFactory : MonoBehaviour
{
    public List<EngineBlock> BlockPrefabs;
    private Dictionary<BlockType, EngineBlock> _blockPrefabMap;
    
    [Header("Block Icon")] 
    [SerializeField] private List<BlockIconEntry> _blockIconEntries;

    private Dictionary<BlockType, EngineBlock> _engineBlockDic;
    private Dictionary<BlockType, InventoryBlock> _inventoryBlockDic;
    private Dictionary<BlockType, Sprite> _blockIconDic;
    
    private void Awake()
    {
        // Block 설정
        _blockPrefabMap = new Dictionary<BlockType, EngineBlock>();

        foreach (var block in BlockPrefabs)
        {
            if (block == null) continue;
            if (_blockPrefabMap.ContainsKey(block.Type))
            {
                continue;
            }

            _blockPrefabMap.Add(block.Type, block);
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
    
    public EngineBlock CreateBlock(BlockType type, Transform parent = null)
    {
        if (!_blockPrefabMap.TryGetValue(type, out var prefab))
        {
            return null;
        }

        EngineBlock instance = Instantiate(prefab, parent);
        instance.name = $"{type}Block";
        return instance;
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
public class BlockIconEntry
{
    public BlockType Type;
    public Sprite Icon;
}