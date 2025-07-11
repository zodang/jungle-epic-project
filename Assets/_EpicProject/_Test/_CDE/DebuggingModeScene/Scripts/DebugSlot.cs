using Define;
using Mono.Cecil.Cil;
using System;
using UnityEngine;

public class DebugSlot : MonoBehaviour, ISlot
{
    public event Action<EngineBlock> OnBlockSet;
    private Clickable _targetClickable;
    [SerializeField] private int index;

    public int GetSlotIndex()
    {
        return index;
    }

    public SlotType GetSlotType()
    {
        return SlotType.DebugSlot;
    }

    public Clickable GetTargetClickable()
    {
        return _targetClickable;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void SetBlockPosition(EngineBlock block)
    {
        OnBlockSet?.Invoke(block);
        
        block.transform.SetParent(transform);
        block.transform.localPosition = Vector3.zero;
        block.SetVisualState(SlotType.DebugSlot);
    }

    public void SetTargetClickable(Clickable clickable)
    {
        _targetClickable = clickable;
    }
    
    
}
