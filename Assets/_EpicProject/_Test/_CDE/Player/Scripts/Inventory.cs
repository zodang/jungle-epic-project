using System.Collections.Generic;
using Define;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<EngineBlock> BlockList = new List<EngineBlock>();
    private InventorySlot _inventorySlot;

    private void Awake()
    {
        _inventorySlot = FindAnyObjectByType<InventorySlot>();
    }

    private void Start()
    {
        _inventorySlot.SetInventory(this);
    }

    public void AddBlock(EngineBlock block)
    {
        if (!BlockList.Contains(block))
        {
            BlockList.Add(block);
        }
    }

    public void Collect(BlockType type)
    {
        var factory = FindAnyObjectByType<BlockFactory>();
        AddBlock(factory.CreateBlock(type, _inventorySlot.transform));
    }
}
