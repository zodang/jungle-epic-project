using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class SnapSliderControlBase<TFeature> : EngineBlock where TFeature : class
{
    protected TFeature Feature;
    
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _speedText;
    
    public override void Activate(object feature)
    {
        Feature = feature as TFeature;
        if (Feature == null) return;
        
        // 슬라이더의 최대, 최소, 현재 값 설정
        _slider.minValue = 0;
        _slider.value = GetCurrentValue();
        
        // 슬라이더 값 변경 시마다 value 전달
        _slider.onValueChanged.AddListener(OnSliderChanged);
        UpdatePercentText(_slider.value);
    }

    public override void Deactivate(object feature)
    {
        if (_slider == null) return;

        _slider.onValueChanged.RemoveListener(OnSliderChanged);
        Feature = null;
    }

    public override void ResetUI()
    {
        if (_slider == null) return;

        _slider.value = GetCurrentValue();
        UpdatePercentText(_slider.value);
    }

    protected void UpdatePercentText(float value)
    {
        if (_speedText == null || _slider == null) return;
        _speedText.text = $"{value + 1} km/s";
    }
    
    protected abstract float GetCurrentValue();
    protected abstract void OnSliderChanged(float value);
}
