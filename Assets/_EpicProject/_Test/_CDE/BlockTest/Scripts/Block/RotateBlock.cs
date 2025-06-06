using System;
using Define;

public class RotateBlock : DialControlBase<IRotatable>
{
    public override BlockType Type => BlockType.Rotate;
    public override Type RequiredFeatureType => typeof(IRotatable);

    protected override float GetMinValue() => _feature.GetMinValue();

    protected override float GetMaxValue() => _feature.GetMaxValue();

    protected override float GetCurrentValue() => _feature.GetCurrentValue();
    protected override void OnDialValueChanged(float value) => _feature.SetValue(value);
}
