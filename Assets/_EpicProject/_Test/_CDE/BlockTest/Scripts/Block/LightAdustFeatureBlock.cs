using System;
using Define;
using UnityEngine;

public class LightAdustFeatureBlock : SliderControlBase<ILightAdjustable>
{
    public override BlockType Type => BlockType.Light;
    public override Type RequiredFeatureType => typeof(ILightAdjustable);
    
    protected override float GetMinValue() => _feature.GetMinValue();
    protected override float GetMaxValue() => _feature.GetMaxValue();
    protected override float GetCurrentValue() => _feature.GetCurrentValue();
    protected override void OnSliderChanged(float value)
    {
        // slider 값 snap 후 전달
        float step = (_feature.GetMaxValue() - GetMinValue()) / 100f;
        float snappedValue = Mathf.Round((value - GetMinValue()) / step) * step + GetMinValue();
        _feature.SetValue(snappedValue);   
        UpdatePercentText(snappedValue);
    }
}
