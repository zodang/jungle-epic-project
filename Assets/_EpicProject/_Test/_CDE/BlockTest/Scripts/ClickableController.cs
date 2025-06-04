using System;
using UnityEngine;

public class ClickableController : MonoBehaviour, IClickable
{
    public InspectorData InspectorData = new InspectorData();
    
    private PopInspectorUI _popInspector;
    
    private Action OnObjectClicked;

    private void Awake()
    {
        _popInspector = FindAnyObjectByType<PopInspectorUI>();
        OnObjectClicked += WhenClicked;
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

    public void AddBlock(Block block)
    {
        if (InspectorData.BlockList.Contains(block)) return;
        
        InspectorData.BlockList.Add(block);
        block.Activate(this);
    }

    public void RemoveBlock(Block block)
    {
        if (!InspectorData.BlockList.Contains(block)) return;
        
        block.Deactivate(this);
        InspectorData.BlockList.Remove(block);
    }
}
