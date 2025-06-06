using System;
using UnityEngine.UI;
using Define;

public class RotateBlock : FeatureBlock
{
    private IRotatable _rotatable;
    private Slider _slider;

    public override BlockType Type => BlockType.Rotate;
    public override Type RequiredFeatureType => typeof(IRotatable);
    
    public override void Activate(object feature)
    {
        _rotatable = feature as IRotatable;
        if (_rotatable == null) return;

        _slider = GetComponentInChildren<Slider>();

        // 슬라이더의 최대, 최소, 현재 값 설정
        _slider.minValue = _rotatable.GetMinValue();
        _slider.maxValue = _rotatable.GetMaxValue();
        _slider.value =  _rotatable.GetCurrentValue();
        
        // 슬라이더 값 변경 시마다 value 전달
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public override void Deactivate(object feature)
    {
        _rotatable = feature as IRotatable;
        if (_rotatable == null) return;
        
        _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        _rotatable = null;
    }

    private void OnSliderValueChanged(float value)
    {
        _rotatable?.SetValue(value);
    }
}
