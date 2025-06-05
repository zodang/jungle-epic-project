using System;
using UnityEngine;

public class Slot : MonoBehaviour
{
    private DraggableBlock _currentDraggableBlock;

    public bool CanDrop()
    {
        return _currentDraggableBlock == null;
    }
    
    public virtual void OnBlockDrop(DraggableBlock draggableBlock)
    {
        _currentDraggableBlock = draggableBlock;
        
        // 위치 변경
        draggableBlock.transform.SetParent(transform, false);
        draggableBlock.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

    }

    public virtual void OnBlockRemoved()
    {
        _currentDraggableBlock = null;
    }

    public DraggableBlock GetChildBlock()
    {
        foreach (Transform child in transform)
        {
            var draggable = child.GetComponent<DraggableBlock>();
            if (draggable != null) return draggable;
        }

        return null;
    }
}
