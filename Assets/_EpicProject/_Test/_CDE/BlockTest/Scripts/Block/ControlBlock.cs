using System;

public class ControlBlock : Block
{
    private IControllable _controllable;
    
    public override Type RequiredFeatureType => typeof(IControllable);

    public override void Activate(object feature)
    {
        _controllable = feature as IControllable;
        _controllable?.EnableControl();
    }

    public override void Deactivate(object feature)
    {
        IControllable target = _controllable;
        if (target == null)
            target = feature as IControllable;

        if (target != null)
            target.DisableControl();
        
        _controllable = null;
    }
}
