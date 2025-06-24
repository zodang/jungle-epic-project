using UnityEngine;

public abstract class DialControlBase<TFeature>: EngineBlock where TFeature : class
{
    protected TFeature _feature;
    [SerializeField] private DialHandle _dialHandle; // 회전할 이미지

    public override void Activate(object feature)
    {
        _feature = feature as TFeature;
        if (_feature == null) return;
        
        _dialHandle.OnValueChanged += HandleDialChanged;

        float current = Mathf.Clamp(GetCurrentValue(), GetMinValue(), GetMaxValue());
        float normalized = Mathf.InverseLerp(GetMinValue(), GetMaxValue(), current);
        _dialHandle.SetRotationByValue(normalized);
    }

    public override void Deactivate(object feature)
    {
        if (_dialHandle == null) return;
        
        _dialHandle.OnValueChanged -= HandleDialChanged;
        _feature = null;
    }

    public override void ResetUI()
    {
        if (_dialHandle == null) return;
        
        float current = Mathf.Clamp(GetCurrentValue(), GetMinValue(), GetMaxValue());
        float normalized = Mathf.InverseLerp(GetMinValue(), GetMaxValue(), current);
        _dialHandle.SetRotationByValue(normalized); // UI 초기화
    }

    private void HandleDialChanged(float normalized)
    {
        float actual = Mathf.Lerp(GetMinValue(), GetMaxValue(), normalized);
        OnDialValueChanged(actual);
    }
    
    protected abstract float GetMinValue();
    protected abstract float GetMaxValue();
    protected abstract float GetCurrentValue();
    protected abstract void OnDialValueChanged(float value);
}
