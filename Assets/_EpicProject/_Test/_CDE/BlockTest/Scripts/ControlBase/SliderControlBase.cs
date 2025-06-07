using Unity.VisualScripting;
using UnityEngine.UI;

public abstract class SliderControlBase<TFeature> : EngineBlock where TFeature : class
{
    protected TFeature _feature;
    private Slider _slider;
    
    public override void Activate(object feature)
    {
        _feature = feature as TFeature;
        if (_feature == null) return;
        
        _slider = GetComponentInChildren<Slider>();
        if (_slider.GetComponent<SliderInteractionDetector>() == null)
        {
            _slider.AddComponent<SliderInteractionDetector>();
        }

        // 슬라이더의 최대, 최소, 현재 값 설정
        _slider.minValue = GetMinValue();
        _slider.maxValue = GetMaxValue();
        _slider.value = GetCurrentValue();
        
        // 슬라이더 값 변경 시마다 value 전달
        _slider.onValueChanged.AddListener(OnSliderChanged);
    }

    public override void Deactivate(object feature)
    {
        if (_slider == null) return;

        _slider.onValueChanged.RemoveListener(OnSliderChanged);
        _feature = null;
    }
    
    protected abstract float GetMinValue();
    protected abstract float GetMaxValue();
    protected abstract float GetCurrentValue();
    protected abstract void OnSliderChanged(float value);
}
