using SMG;
using Unity.VisualScripting;
using UnityEngine;

public class TempPopInspector : MonoBehaviour
{

    [Header("IControllable")]
    [SerializeField, ReadOnly] private bool _hasIControllable;
    public bool CanControl;
    private IControllable _controllable;

    [Header("IScalable")]
    [SerializeField, ReadOnly] private bool _hasIScalable;
    [SerializeField, ReadOnly] private float _scaleMin;
    [SerializeField, ReadOnly] private float _scaleMax;
    [SerializeField, ReadOnly] private float _currentScale;
    public float SetScale;
    private IScalable _scalable;

    [Header("IRotatable")]
    [SerializeField, ReadOnly] private bool _hasIRotatable;
    [SerializeField, ReadOnly] private float _rotateMin;
    [SerializeField, ReadOnly] private float _rotateMax;
    [SerializeField, ReadOnly] private float _currentRotate;
    public float SetRotate;
    private IRotatable _rotatable;

    [Header("ILightAdjustable")]
    [SerializeField, ReadOnly] private bool _hasILightAdjustable;
    [SerializeField, ReadOnly] private float _lightMin;
    [SerializeField, ReadOnly] private float _lightMax;
    [SerializeField, ReadOnly] private float _currentLight;
    public float SetLight;
    private ILightAdjustable _lightAdjustable;


    private void Start()
    {
        _hasIControllable = TryGetComponent<IControllable>(out _controllable);

        _hasIScalable = TryGetComponent<IScalable>(out _scalable);
        if (_hasIScalable)
        {
            _scaleMin = _scalable.GetMinValue();
            _scaleMax = _scalable.GetMaxValue();
        }

        _hasIRotatable = TryGetComponent<IRotatable>(out _rotatable);
        if (_hasIRotatable)
        {
            _rotateMin = _rotatable.GetMinValue();
            _rotateMax = _rotatable.GetMaxValue();
        }

        _hasILightAdjustable = TryGetComponent<ILightAdjustable>(out _lightAdjustable);
        if (_hasILightAdjustable)
        {
            _lightMin = _lightAdjustable.GetMinValue();
            _lightMax = _lightAdjustable.GetMaxValue();
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!_controllable.IsUnityNull())
        {
            if (CanControl) _controllable.EnableControl();
            else _controllable.DisableControl();
        }
        if (!_scalable.IsUnityNull())
        {
            _currentScale = _scalable.GetCurrentValue();
            _scalable.SetValue(SetScale);
        }
        if (!_rotatable.IsUnityNull())
        {
            _currentRotate = _rotatable.GetCurrentValue();
            _rotatable.SetValue(SetRotate);
        }
        if (!_lightAdjustable.IsUnityNull())
        {
            _currentLight = _lightAdjustable.GetCurrentValue();
            _lightAdjustable.SetValue(SetLight);
        }


    }
}
