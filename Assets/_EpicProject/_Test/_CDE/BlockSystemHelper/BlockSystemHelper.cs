using Define;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockSystemHelper : MonoBehaviour
{
    [SerializeField] private BlinkingSlot blinkingSlotPref;
    
    private List<GameObject> _inventorySlot = new();
    private List<GameObject> _engineSlot = new();
    private List<BlinkingSlot> _inventoryBlinkingSlots = new();
    private List<BlinkingSlot> _engineBlinkingSlots = new();

    private void Awake()
    {
        FindAnyObjectByType<EngineManager>().OnEngineSettingEnd += SetBlinkingSlots;
    }
    
    private void SetBlinkingSlots()
    {
        // 슬롯 탐색
        foreach (var slot in FindObjectsByType<InventorySlot>(FindObjectsSortMode.None))
        {
            _inventorySlot.Add(slot.GetComponentInChildren<Image>().gameObject);
        }

        foreach (var slot in FindObjectsByType<EngineSlot>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            _engineSlot.Add(slot.gameObject);
        }
        
        // blinking slot 추가
        foreach (GameObject slot in _inventorySlot)
        {
            BlinkingSlot newBlinking = Instantiate(blinkingSlotPref, slot.transform);
            _inventoryBlinkingSlots.Add(newBlinking);
        }

        foreach (GameObject slot in _engineSlot)
        {
            BlinkingSlot newBlinking = Instantiate(blinkingSlotPref, slot.transform);
            _engineBlinkingSlots.Add(newBlinking);
        }
    }

    public void StartBlinking(EngineBlock block, ISlot slot)
    {
        if (slot.GetSlotType() == SlotType.InventorySlot)
        {
            StartEngineBlinking();
        }
        
        else if (slot.GetSlotType() == SlotType.EngineSlot)
        {
            StartInventoryBlinking();
        }
    }

    public void StopBlinking(EngineBlock block, ISlot slot)
    {
        StopInventoryBlinking();
        StopEngineBlinking();
    }

    private void StartInventoryBlinking()
    {
        foreach (var slot in _inventoryBlinkingSlots)
        {
            slot.StartBlinking();
        }
    }
    

    private void StopInventoryBlinking()
    {
        foreach (var slot in _inventoryBlinkingSlots)
        {
            slot.StopBlinking();
        }
    }

    private void StartEngineBlinking()
    {
        foreach (var slot in _engineBlinkingSlots)
        {
            slot.StartBlinking();
        }
    }

    private void StopEngineBlinking()
    {
        foreach (var slot in _engineBlinkingSlots)
        {
            slot.StopBlinking();
        }
    }
}
