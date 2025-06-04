using System;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public Action<DraggableBlock, Slot, Slot> OnBlockDropped; // 블록, 이전 Slot, 이후 Slot 
    private DraggableBlock _currentDraggableBlock;

    private void Start()
    {
        // 초기 블록 체크
        DraggableBlock draggableBlock = GetChildBlock();
        if (draggableBlock != null)
        {
            OnBlockDrop(draggableBlock);
        }
    }

    public bool CanDrop()
    {
        return _currentDraggableBlock == null;
    }
    
    public virtual void OnBlockDrop(DraggableBlock draggableBlock)
    {
        _currentDraggableBlock = draggableBlock;
        
        OnBlockDropped?.Invoke(draggableBlock, draggableBlock.PrevSlot, this);
    }

    public virtual void OnBlockRemoved()
    {
        _currentDraggableBlock = null;
    }

    private DraggableBlock GetChildBlock()
    {
        foreach (Transform child in transform)
        {
            DraggableBlock draggableBlock = child.GetComponent<DraggableBlock>(); 
            if (draggableBlock != null)
                return draggableBlock;
        }
        return null;
    }
}
