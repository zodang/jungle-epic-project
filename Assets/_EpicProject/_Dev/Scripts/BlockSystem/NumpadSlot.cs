using Define;
using UnityEngine;

public class NumpadSlot : MonoBehaviour, ISlotType
{
    private Clickable _targetClickable; 
    public SlotType GetSlotType()
    {
        return SlotType.NumpadSlot;
    }

    public Clickable GetTargetClickable()
    {
        return _targetClickable;
    }

    public void SetTargetClickable(Clickable clickable)
    {
        _targetClickable = clickable;
    }
}
