using System;
using UnityEngine;

public class ClickableController : MonoBehaviour, IClickable
{
    public InspectorData InspectorData = new InspectorData();
    
    private InspectorSlot[] _slots;
    private PopInspectorUI _popInspector;
    
    private Action OnObjectClicked;

    private void Awake()
    {
        _popInspector = FindAnyObjectByType<PopInspectorUI>();
        _slots = FindObjectsByType<InspectorSlot>(FindObjectsSortMode.None);
        
        OnObjectClicked += WhenClicked;

        foreach (var slot in _slots)
        {
            slot.OnBlockDropped += OnBlockDropped;
        }
    }
    
    private void OnDestroy()
    {
        OnObjectClicked -= WhenClicked;
    }

    public void OnClicked()
    {
        OnObjectClicked?.Invoke();
    }

    private void WhenClicked()
    {
        _popInspector.Show(this);
    }

    private void OnBlockDropped(DraggableBlock draggable, Slot start, Slot end)
    {
        Block block = draggable.GetComponent<Block>();

        if (start is InventorySlot slot)
        {
            // 인벤토리에서 제거
        }

        AddBlock(block);
    }
    private void AddBlock(Block block)
    {
        if (InspectorData.BlockList.Contains(block)) return;

        var feature = GetComponent(block.RequiredFeatureType);
        InspectorData.BlockList.Add(block);
        block.Activate(feature);
    }

    public void RemoveBlock(Block block)
    {
        if (!InspectorData.BlockList.Contains(block)) return;
        
        block.Deactivate(this);
        InspectorData.BlockList.Remove(block);
    }
}
