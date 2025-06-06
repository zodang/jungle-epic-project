using System;
using UnityEngine.UI;
using Define;

public class ScaleBlock : FeatureBlock
{
    private IScalable _scalable;
    private Slider _slider;

    public override BlockType Type => BlockType.Scale;
    public override Type RequiredFeatureType => typeof(IScalable);
    
    public override void Activate(object feature)
    {
        _scalable = feature as IScalable;
        if (_scalable == null) return;
            
        // 슬라이더의 최대, 최소, 현재 값 설정
        _slider = GetComponentInChildren<Slider>();
        
        _slider.minValue = _scalable.GetMinValue();
        _slider.maxValue = _scalable.GetMaxValue();
        _slider.value =  _scalable.GetCurrentValue();
        
        // 슬라이더 값 변경 시마다 value 전달
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public override void Deactivate(object feature)
    {
        _scalable = feature as IScalable;
        if (_scalable == null) return;
        
        _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        _scalable = null;
    }

    private void OnSliderValueChanged(float value)
    {
        _scalable?.SetValue(value);
    }
}
