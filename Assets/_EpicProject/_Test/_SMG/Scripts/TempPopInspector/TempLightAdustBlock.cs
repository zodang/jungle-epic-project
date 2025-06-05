using UnityEngine;
using SMG;
using Unity.VisualScripting;

public class TempLightAdustBlock : MonoBehaviour
{
    [SerializeField, ReadOnly] private bool _hasILightAdjustable;
    [SerializeField, ReadOnly] private float _lightMin;
    [SerializeField, ReadOnly] private float _lightMax;
    [SerializeField, ReadOnly] private float _currentLight;
    public float SetLight;
    private ILightAdjustable _lightAdjustable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _hasILightAdjustable = TryGetComponent<ILightAdjustable>(out _lightAdjustable);
        if (_hasILightAdjustable)
        {
            _lightMin = _lightAdjustable.GetMinValue();
            _lightMax = _lightAdjustable.GetMaxValue();
            SetLight = _lightAdjustable.GetCurrentValue();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_lightAdjustable.IsUnityNull())
        {
            _currentLight = _lightAdjustable.GetCurrentValue();
            _lightAdjustable.SetValue(SetLight);
        }
    }
}
