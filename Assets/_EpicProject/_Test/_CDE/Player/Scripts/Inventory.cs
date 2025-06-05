using System.Collections.Generic;
using Define;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<InventorySlot> slots;
    
    public List<BlockType> BlockTypeList = new List<BlockType>();

    private BlockFactory _blockFactory;

    private void Start()
    {
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
        foreach (var slot in slots)
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
