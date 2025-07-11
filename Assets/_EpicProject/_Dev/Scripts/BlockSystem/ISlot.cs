using Define;
using UnityEngine;

public interface ISlot
{
    SlotType GetSlotType();
    Clickable GetTargetClickable();
    Transform GetTransform();

    void SetBlockPosition(EngineBlock block);
    void SetTargetClickable(Clickable clickable);
}
