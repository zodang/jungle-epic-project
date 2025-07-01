using System;
using UnityEngine;

public class SpeedHandler : MonoBehaviour, ISpeedChangeable
{
    public Action<int> OnSetValue;
    [SerializeField] private int _currentStep;

    public float GetCurrentValue()
    {
        return _currentStep;
    }

    public void Init(int current)
    {
        _currentStep = current;
    }
    
    public void SetValue(int value)
    {
        _currentStep = value;
        OnSetValue?.Invoke(value);
    }
}
