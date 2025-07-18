using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class SliderControlBase<TFeature> : EngineBlock where TFeature : class
{
    protected TFeature _feature;
    
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _percentText;
    
    private SliderInteractionDetector _sliderDetector;
    public bool IsControlStarted { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        _sliderDetector = _slider.GetComponent<SliderInteractionDetector>();
        _sliderDetector.OnControlStarted += WhenControlStarted;
    }

    public override void Activate(object feature)
    {
        _feature = feature as TFeature;
        if (_feature == null) return;
        
        // 슬라이더의 최대, 최소, 현재 값 설정
        _slider.minValue = GetMinValue();
        _slider.maxValue = GetMaxValue();
        _slider.value = GetCurrentValue();
        
        // 슬라이더 값 변경 시마다 value 전달
        _slider.onValueChanged.AddListener(OnSliderChanged);
        UpdatePercentText(_slider.value);
    }

    public override void Deactivate(object feature)
    {
        if (_slider == null) return;

        _slider.onValueChanged.RemoveListener(OnSliderChanged);
        _feature = null;
    }

    public override void ResetUI()
    {
        if (_slider == null) return;

        _slider.value = GetCurrentValue();
        UpdatePercentText(_slider.value);
    }

    protected void UpdatePercentText(float value)
    {
        if (_percentText == null || _slider == null) return;

        float percent = (_slider.maxValue - _slider.minValue <= 0f)
            ? 0f
            : (value - _slider.minValue) / (_slider.maxValue - _slider.minValue);
        _percentText.text = $"{Mathf.RoundToInt(percent * 100)}%";
        
        Debug.Log($"{value} / {Mathf.RoundToInt(percent * 100)}");
    }

    private void WhenControlStarted(bool isControl)
    {
        IsControlStarted = isControl;
    }

    protected abstract float GetMinValue();
    protected abstract float GetMaxValue();
    protected abstract float GetCurrentValue();
    protected abstract void OnSliderChanged(float value);
}
