using Define;
using UnityEngine;
using UnityEngine.UI;

public class EngineSlot : MonoBehaviour, ISlotType
{
    private Clickable _targetClickable;
    [SerializeField] private GameObject blockContainer;
    [SerializeField] private Image slotIcon;

    public SlotType GetSlotType()
    {
        return SlotType.EngineSlot;
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
