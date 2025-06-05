using System;
using UnityEngine.UI;
using Define;

public class RotateBlock : FeatureBlock
{
    private Slider _slider;
    private IRotatable _rotatable;
    public override BlockType Type => BlockType.Rotate;

    public override void Awake()
    {
        base.Awake();
        
        _slider = GetComponentInChildren<Slider>();
    }
    
    public override Type RequiredFeatureType => typeof(IRotatable);
    
    public override void Activate(object feature)
    {
        _rotatable = feature as IRotatable;
        if (_rotatable == null) return;

        // 슬라이더의 최대, 최소, 현재 값 설정
        _slider.minValue = _rotatable.GetMinValue();
        _slider.maxValue = _rotatable.GetMaxValue();
        _slider.value =  _rotatable.GetCurrentValue();
        
        // 슬라이더 값 변경 시마다 value 전달
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
        _slider.gameObject.SetActive(true);
    }

    public override void Deactivate(object feature)
    {
        _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        _slider.gameObject.SetActive(false);
        
        _rotatable = null;
    }

    private void OnSliderValueChanged(float value)
    {
        _rotatable?.SetValue(value);
    }
}
