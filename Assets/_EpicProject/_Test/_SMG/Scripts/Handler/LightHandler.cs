using System;
using UnityEngine;

public class LightHandler : MonoBehaviour, ILightAdjustable
{
    [SerializeField] private float _minBright = 0.5f;
    [SerializeField] private float _maxBright = 3f;
    [SerializeField] private float _currentBright;

    public Action<float> OnSetValue;

    public float CurrentBright
    {
        get => _currentBright;
        private set => _currentBright = value;
    }

    public float GetCurrentValue() => _currentBright;
    public float GetMaxValue() => _maxBright;
    public float GetMinValue() => _minBright;

    public void Init(float min, float max, float current)
    {
        _minBright = min;
        _maxBright = max;
        _currentBright = current;
    }

    public void SetValue(float value)
    {
        _currentBright = value;
        OnSetValue?.Invoke(_currentBright);
    }
}
