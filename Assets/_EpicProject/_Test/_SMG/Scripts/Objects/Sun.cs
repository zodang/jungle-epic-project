using UnityEngine;

public class Sun : MonoBehaviour, IFeatureResetable, ILightAdjustable, IRotatable, IScalable, IControllable
{
    // ILightAdjustable: 빛 밝기 관련
    private float _minBright = 1f;  //0.1f; 
    private float _maxBright = 2f;  //3f
    private float _currentBright;
    private const float _defaultBright = 2f;

    // IRotatable: 회전 관련
    private float _minAngle = 0f;
    private float _maxAngle = 359f;
    private float _currentAngle;
    private const float _defaultAngle = 150f;

    // IScalable
    private float _minScale = 0.8f;
    private float _maxScale = 1.5f;
    private float _currentScale;
    private const float _defaultScale = 1f;

    // IControllable
    private bool _enableMove;
    private float _speed = 5f;

    private Transform _model;
    private Transform _lightDir;

    [Header("Move Position")]
    public float MinPosX;
    public float MaxPosX;
    private float _currentPosX;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _model = transform.GetChild(0);
        _lightDir = transform.GetChild(1);
    }

    private void Start()
    {
        ResetFeature();
        _currentPosX = transform.position.x;
    }

    private void Update()
    {
        _currentPosX = transform.position.x;
        if (_enableMove)
        {
            Vector2 moveInput = new Vector2 (StageManager.Instance.InputManager.MoveInput.x, 0);
            transform.Translate(moveInput * _speed * Time.deltaTime);

            Vector3 pos = transform.localPosition;
            if(pos.x < MinPosX)
            {
                pos.x = MinPosX;
            }
            if(pos.x > MaxPosX)
            {
                pos.x = MaxPosX;
            }
            transform.localPosition = pos;
        }
        CheckTrigger();
    }

    void AdjustLight(float brightness)
    {
        // Sun Dir
        if (_currentBright < 1f)
            _currentBright = 1f;
        _lightDir.localScale = (_currentBright - 1f) * Vector3.one;
    }

    // 0 ~ 359
    void SetRotate(float angle)
    {
        _model.localEulerAngles = new Vector3(0, 0, -angle);
        _lightDir.localEulerAngles = new Vector3(0, 0, -angle);
    }

    void CheckTrigger()
    {
        // 2f
        if(_currentPosX > -3f && _currentPosX < 6f)
        {
            if(_currentAngle > 120 && _currentAngle <= 210 && _currentBright >1.6f)
            {
                EvaporationHandler[] evaporations = FindObjectsByType<EvaporationHandler>(FindObjectsSortMode.None);
                for (int i = 0; i < evaporations.Length; i++)
                {
                    evaporations[i].Evaporate();
                }
            }
        }
    }

    // IFeatureResetable
    public void ResetFeature()
    {
        ((ILightAdjustable)this).SetValue(_defaultBright);
        ((IRotatable)this).SetValue(_defaultAngle);
        ((IScalable)this).SetValue(_defaultScale);
        // DisableControl();
    }

    #region ILightAdjustable
    float ILightAdjustable.GetMinValue() => _minBright;

    float ILightAdjustable.GetMaxValue() => _maxBright;

    float ILightAdjustable.GetCurrentValue() => _currentBright;
    
    void ILightAdjustable.SetValue(float value)
    {
        _currentBright = value;
        AdjustLight(value);
    }
    #endregion

    #region IRotatable
    float IRotatable.GetMinValue() => _minAngle;

    float IRotatable.GetMaxValue() => _maxAngle;

    float IRotatable.GetCurrentValue() => _currentAngle;
    
    void IRotatable.SetValue(float value)
    {
        _currentAngle = value;
        SetRotate(_currentAngle);
    }
    #endregion

    #region IScalable
    float IScalable.GetMinValue() => _minScale;

    float IScalable.GetMaxValue() => _maxScale;
    float IScalable.GetCurrentValue() => _currentScale;
    void IScalable.SetValue(float value)
    {
        _currentScale = value;
        transform.localScale = new Vector3(_currentScale, _currentScale, _currentScale);
    }
    #endregion

    #region IControllable
    public void EnableControl()
    {
        _enableMove = true;
    }

    public void DisableControl()
    {
        _enableMove = false;
    }
    #endregion
    
}
