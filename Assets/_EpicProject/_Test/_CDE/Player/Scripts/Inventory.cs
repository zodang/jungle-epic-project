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
        Clickable player = StageBaseManager.Instance.PlayerManager.GetComponent<Clickable>();
        _inventorySlot.SetInventory(this);
        _inventorySlot.SetTargetClickable(player);
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
