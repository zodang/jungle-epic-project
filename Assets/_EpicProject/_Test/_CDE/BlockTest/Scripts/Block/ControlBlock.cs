using Unity.VisualScripting;

public class ControlBlock : Block
{
    private ClickableController _target;
    public override void Activate(ClickableController target)
    {
        _target = target;
        _target.AddComponent<Movement>();
    }

    public override void Deactivate(ClickableController target)
    {
        Destroy(_target.GetComponent<Movement>());
        _target = null;
    }
}
