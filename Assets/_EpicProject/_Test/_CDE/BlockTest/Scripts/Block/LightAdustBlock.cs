using System;
using UnityEngine;
using UnityEngine.UI;

public class LightAdustBlock : Block
{
    private Slider _slider;
    private ILightAdjustable _adjustable;

    private void Awake()
    {
        _slider = GetComponentInChildren<Slider>();
        _slider.gameObject.SetActive(false);
    }
    
    public override Type RequiredFeatureType => typeof(ILightAdjustable);
    
    public override void Activate(object feature)
    {
        _adjustable = feature as ILightAdjustable;
        if (_adjustable == null) return;

        // 슬라이더의 최대, 최소, 현재 값 설정
        _slider.minValue = _adjustable.GetMinValue();
        _slider.maxValue = _adjustable.GetMaxValue();
        _slider.value =  _adjustable.GetCurrentValue();
        
        // 슬라이더 값 변경 시마다 value 전달
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
        _slider.gameObject.SetActive(true);
    }

    public override void Deactivate(object feature)
    {
        _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        _slider.gameObject.SetActive(false);
        
        _adjustable = null;
    }

    private void OnSliderValueChanged(float value)
    {
        _adjustable?.SetValue(value);
    }
}
