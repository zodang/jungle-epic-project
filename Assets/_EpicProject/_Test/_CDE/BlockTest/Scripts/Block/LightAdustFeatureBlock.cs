using System;
using UnityEngine.UI;
using Define;

public class LightAdustFeatureBlock : FeatureBlock
{
    private ILightAdjustable _adjustable;
    private Slider _slider;

    public override BlockType Type => BlockType.Light;
    public override Type RequiredFeatureType => typeof(ILightAdjustable);
    
    public override void Activate(object feature)
    {
        _adjustable = feature as ILightAdjustable;
        if (_adjustable == null) return;
        
        _slider = GetComponentInChildren<Slider>();

        // 슬라이더의 최대, 최소, 현재 값 설정
        _slider.minValue = _adjustable.GetMinValue();
        _slider.maxValue = _adjustable.GetMaxValue();
        _slider.value =  _adjustable.GetCurrentValue();
        
        // 슬라이더 값 변경 시마다 value 전달
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public override void Deactivate(object feature)
    {
        _adjustable = feature as ILightAdjustable;
        if (_adjustable == null) return;
        
        _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        _adjustable = null;
    }

    private void OnSliderValueChanged(float value)
    {
        _adjustable?.SetValue(value);
    }
}
