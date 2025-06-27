using System;
using UnityEngine;

public class RotateHandler : MonoBehaviour, IRotatable
{
    [SerializeField] private float _minRotate = 0f;
    [SerializeField] private float _maxRotate = 359.9f;
    [SerializeField] private float _currentRotate;

    public Action<float> OnSetValue;

    public float CurrentRotate
    {
        get => _currentRotate;
        private set => _currentRotate = value;
    }

    public float GetCurrentValue() => _currentRotate;
    public float GetMaxValue() => _maxRotate;
    public float GetMinValue() => _minRotate;

    public void Init(float min, float max, float current)
    {
        _minRotate = min;
        _maxRotate = max;
        _currentRotate = current;
    }

    public void SetValue(float value)
    {
        // 여기서 min, max clamp 검사를 하는 게 좋은가?
        _currentRotate = value;
        OnSetValue?.Invoke(_currentRotate);
    }
}
