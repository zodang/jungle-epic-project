using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EngineSlotGroup : MonoBehaviour
{
    [SerializeField] private List<Transform> slots;
    [SerializeField] private List<Button> slotBtns;

    private int _currentFocusedSlot = -1;

    private void Start()
    {
        for (int i = 0; i < slotBtns.Count; i++)
        {
            int index = i;
            slotBtns[i].onClick.AddListener(() => FocusSlot(index));
        }
    }

    private void FocusSlot(int index)
    {
        if (index < 0 || index >= slots.Count) return;
        if (_currentFocusedSlot == index) return;
        
        // UI 전면으로 이동
        slots[index].SetAsLastSibling();
        _currentFocusedSlot = index;
    }
}

