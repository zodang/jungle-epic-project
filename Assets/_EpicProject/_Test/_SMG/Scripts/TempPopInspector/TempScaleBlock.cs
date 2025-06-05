using UnityEngine;
using SMG;
using Unity.VisualScripting;

public class TempScaleBlock : MonoBehaviour
{
    [SerializeField, ReadOnly] private bool _hasIScalable;
    [SerializeField, ReadOnly] private float _scaleMin;
    [SerializeField, ReadOnly] private float _scaleMax;
    [SerializeField, ReadOnly] private float _currentScale;
    public float SetScale;
    private IScalable _scalable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _hasIScalable = TryGetComponent<IScalable>(out _scalable);
        if (_hasIScalable)
        {
            _scaleMin = _scalable.GetMinValue();
            _scaleMax = _scalable.GetMaxValue();
            SetScale = _scalable.GetCurrentValue();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_scalable.IsUnityNull())
        {
            _currentScale = _scalable.GetCurrentValue();
            _scalable.SetValue(SetScale);
        }
    }
}
