using Define;
using System;

public class SpeedBlock : SnapSliderControlBase<ISpeedChangeable>
{
    public override BlockType Type => BlockType.Speed;
    public override Type RequiredFeatureType => typeof(ISpeedChangeable);

    protected override float GetCurrentValue() => Feature.GetCurrentValue();

    protected override void OnSliderChanged(float value)
    {
        Feature.SetValue((int)value);   
        UpdatePercentText(value);
    }
}
