using Define;

public interface ISlotType
{
    SlotType GetSlotType();
    Clickable GetTargetClickable();

    void SetTargetClickable(Clickable clickable);
}
