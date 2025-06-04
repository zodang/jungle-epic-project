using UnityEngine;

public abstract class Block : MonoBehaviour
{
    public abstract void Activate(ClickableController clickable);
    public abstract void Deactivate(ClickableController clickable);
}
