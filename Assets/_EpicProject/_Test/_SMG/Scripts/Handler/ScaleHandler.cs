using System;
using UnityEngine;

public class ScaleHandler : MonoBehaviour, IScalable
{
    [SerializeField] private float _minScale = 0.5f;
    [SerializeField] private float _maxScale = 2.5f;
    [SerializeField] private float _currentScale;

    public Action<float> OnSetValue;

    public float CurrentScale
    {
        get => _currentScale;
        private set => _currentScale = value;
    }

    public float GetCurrentValue() => _currentScale;
    public float GetMaxValue() => _maxScale;
    public float GetMinValue() => _minScale;

    public void Init(float min, float max, float current)
    {
        _minScale = min;
        _maxScale = max;
        _currentScale = current;
    }

    public void SetValue(float value)
    {
        _currentScale = value;
        OnSetValue?.Invoke(_currentScale);
    }
}
