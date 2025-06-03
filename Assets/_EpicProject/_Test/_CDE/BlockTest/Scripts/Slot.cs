using Define;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public SlotType Type;
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
    
    public void OnBlockDrop(DraggableBlock draggableBlock)
    {
        _currentDraggableBlock = draggableBlock;
    }

    public void OnBlockRemoved()
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
