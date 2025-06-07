using Define;
using System;

public class ControlBlock : TriggerControlBase<IControllable>
{
    public override BlockType Type => BlockType.PlayerControl;
    public override Type RequiredFeatureType => typeof(IControllable);

    protected override void EnableFeature(IControllable feature) => feature.EnableControl();
    protected override void DisableFeature(IControllable feature) => feature.DisableControl();
}
