using System;
using Define;

public class LightAdustFeatureBlock : SliderControlBase<ILightAdjustable>
{
    public override BlockType Type => BlockType.Light;
    public override Type RequiredFeatureType => typeof(ILightAdjustable);
    
    protected override float GetMinValue() => _feature.GetMinValue();
    protected override float GetMaxValue() => _feature.GetMaxValue();
    protected override float GetCurrentValue() => _feature.GetCurrentValue();
    protected override void OnSliderChanged(float value)
    {
        _feature.SetValue(value);   
        UpdatePercentText(value);
    }
}
