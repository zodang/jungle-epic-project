using System.Collections.Generic;
using Define;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public List<BlockType> BlockTypeList { get; private set; } = new List<BlockType>();

    private InventorySlotGroup _inventorySlotGroup;
    private List<InventorySlot> _slotList;
    private BlockFactory _blockFactory;

    private void Start()
    {
        _inventorySlotGroup = FindAnyObjectByType<InventorySlotGroup>();
        _slotList = new List<InventorySlot>(_inventorySlotGroup.GetComponentsInChildren<InventorySlot>());
        
        _blockFactory = FindAnyObjectByType<BlockFactory>();
    }

    public void Collect(BlockType type)
    {
        BlockTypeList.Add(type);
        AddBlock(type);
    }

    public void AddBlock(BlockType type)
    {
        // 빈 슬롯 찾기
        foreach (var slot in _slotList)
        {
            if (slot.GetChildBlock() == null)
            {
                var blockUI = _blockFactory.CreateBlockUI(type, slot.transform);
                slot.OnBlockDrop(blockUI);
                break;
            }
        }
    }

    public void RemoveBlock(BlockUI blockUI)
    {
        Destroy(blockUI.gameObject);
    }
}
