using System;
using Define;

public class ScaleBlock : SliderControlBase<IScalable>
{
    public override BlockType Type => BlockType.Scale;
    public override Type RequiredFeatureType => typeof(IScalable);
    
    protected override float GetMinValue() => _feature.GetMinValue();
    protected override float GetMaxValue() => _feature.GetMaxValue();
    protected override float GetCurrentValue() => _feature.GetCurrentValue();

    protected override void OnSliderChanged(float value)
    {
        _feature.SetValue(value);   
        UpdatePercentText(value);
    }
}
