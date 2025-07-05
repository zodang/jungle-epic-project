using System.Collections.Generic;
using Define;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<EngineBlock> BlockList = new List<EngineBlock>();
    public List<BlockType> DefaultBlockList = new();
    
    private InventorySlot _inventorySlot;
    private Clickable _target;

    private void Awake()
    {
        _inventorySlot = FindAnyObjectByType<InventorySlot>();
        DefaultBlockList.Add(BlockType.PlayerControl);
    }

    private void Start()
    {
        _target = StageBaseManager.Instance.PlayerManager.GetComponent<Clickable>();
        _inventorySlot.SetInventory(this);
        _inventorySlot.SetTargetClickable(_target);

        InitInventory();
    }

    public void AddBlock(EngineBlock block)
    {
        if (!BlockList.Contains(block))
        {
            BlockList.Add(block);
            block.InitDefaultBlock(_target, SlotType.InventorySlot);
        }
    }

    public void RemoveBlock(EngineBlock block)
    {
        if (!BlockList.Contains(block)) return;

        BlockList.Remove(block);
        block.Deactivate(_target.GetComponent(block.RequiredFeatureType));
    }

    public void Collect(BlockType type)
    {
        var factory = StageManager.Instance.BlockFactory;
        AddBlock(factory.CreateBlock(type, _inventorySlot.transform));
    }

    private void InitInventory()
    {
        for (int i = 0; i < DefaultBlockList.Count; i++)
        {
            BlockType type = DefaultBlockList[i];
            Collect(type);
        }
    }
    
    private void Update()
    {
        for (int i = 1; i <= 4; i++)
        {
            KeyCode key = KeyCode.Alpha0 + i;
            int idx = i;

            if (Input.GetKeyDown(key))
            {
                if (_inventorySlot.transform.childCount > idx)
                {
                    var block = _inventorySlot.transform.GetChild(idx).GetComponent<EngineBlock>();
                    block?.ActivateEngineBlock();
                }
            }
            else if (Input.GetKeyUp(key))
            {
                if (_inventorySlot.transform.childCount > idx)
                {
                    var block = _inventorySlot.transform.GetChild(idx).GetComponent<EngineBlock>();
                    block?.DeactivateEngineBlock();
                }
            }
        }
    }
}
