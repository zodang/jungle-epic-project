using Define;
using UnityEngine;

public class InventorySlot : MonoBehaviour, ISlotType
{
    private Inventory _inventory;
    private Clickable _targetClickable;
    
    public SlotType GetSlotType()
    {
        return SlotType.InventorySlot;
    }

    public Clickable GetTargetClickable()
    {
        return _targetClickable;
    }
    
    public Inventory GetInventory()
    {
        return _inventory;
    }

    public void SetInventory(Inventory inventory)
    {
        _inventory = inventory;
    }

    public void SetBlockPositionToInventory(EngineBlock block)
    {
        block.transform.SetParent(transform);
    }

    public void SetTargetClickable(Clickable clickable)
    {
        _targetClickable = clickable;
    }
}

