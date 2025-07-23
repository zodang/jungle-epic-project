using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class SnapSliderControlBase<TFeature> : EngineBlock where TFeature : class
{
    protected TFeature Feature;
    
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _speedText;
    
    private SliderInteractionDetector _sliderDetector;
    public bool IsControlStarted { get; private set; }
    
    protected override void Awake()
    {
        base.Awake();
        _sliderDetector = _slider.GetComponent<SliderInteractionDetector>();
        
        _sliderDetector.OnControlStarted += WhenControlStarted;
        _sliderDetector.OnControlEnd += WhenControlEnd;
    }
    
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
        _speedText.text = $"{(value + 1) * 10}km/h";
    }
    
    private void WhenControlStarted()
    {
        IsControlStarted = true;
    }
    
    private void WhenControlEnd()
    {
        IsControlStarted = false;
        
        // 로그시스템
        string stageId = StageBaseManager.Instance.StageId;
        string sectionId = StageBaseManager.Instance.SectionId;
        string blockType = Type.ToString();
        string blockValue = _speedText.text;
        string targetObj = CurrentSlot.GetTargetClickable().name;
        GameManager.Instance.LogManager.LogBlockControl(stageId, sectionId, blockType, blockValue, targetObj);
        // Debug.Log($"@@DE ---> {stageId} / {sectionId} / {blockType} / {blockValue} / {targetObj}");
    }
    
    protected abstract float GetCurrentValue();
    protected abstract void OnSliderChanged(float value);
}
