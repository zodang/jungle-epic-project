using UnityEngine;
using SMG;
using Unity.VisualScripting;

public class TempRotateBlock : MonoBehaviour
{
    [SerializeField, ReadOnly] private bool _hasIRotatable;
    [SerializeField, ReadOnly] private float _rotateMin;
    [SerializeField, ReadOnly] private float _rotateMax;
    [SerializeField, ReadOnly] private float _currentRotate;
    public float SetRotate;
    private IRotatable _rotatable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _hasIRotatable = TryGetComponent<IRotatable>(out _rotatable);
        if (_hasIRotatable)
        {
            _rotateMin = _rotatable.GetMinValue();
            _rotateMax = _rotatable.GetMaxValue();
            SetRotate = _rotatable.GetCurrentValue();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_rotatable.IsUnityNull())
        {
            _currentRotate = _rotatable.GetCurrentValue();
            _rotatable.SetValue(SetRotate);
        }
    }
}
