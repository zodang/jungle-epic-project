using Unity.VisualScripting;
using UnityEngine;

public class Sun : MonoBehaviour, IFeatureResetable, IControllable//, ILightAdjustable, IRotatable, IScalable, 
{
    [SerializeField] private RotateHandler _rotateHandler;
    [SerializeField] private ScaleHandler _scaleHandler;
    [SerializeField] private LightHandler _lightHandler;

    // LightHandler
    private float _minBright = 1f;
    private float _maxBright = 2f;
    private const float _defaultBright = 1.7f;

    // RotateHandler
    private float _minAngle = 0f;
    private float _maxAngle = 359.9f;
    private const float _defaultAngle = 160f;

    // ScaleHandler
    private float _minScale = 0.8f;
    private float _maxScale = 1.5f;
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

        ComponentHelper.TryGetOrAddComponent<RotateHandler>(ref _rotateHandler, gameObject);
        ComponentHelper.TryGetOrAddComponent<ScaleHandler>(ref _scaleHandler, gameObject);
        ComponentHelper.TryGetOrAddComponent<LightHandler>(ref _lightHandler, gameObject);

        _rotateHandler.Init(_minAngle, _maxAngle, _defaultAngle);
        _rotateHandler.OnSetValue += SetRotate;

        _scaleHandler.Init(_minScale, _maxScale, _defaultScale);
        _scaleHandler.OnSetValue += SetScale;

        _lightHandler.Init(_minBright, _maxBright, _defaultBright);
        _lightHandler.OnSetValue += AdjustLight;

        ResetFeature();
    }

    private void Start()
    {
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
        if (brightness < _minBright) 
            return;
        _lightDir.localScale = (brightness - _minBright) * Vector3.one;
    }

    // 0 ~ 359
    void SetRotate(float angle)
    {
        _model.localEulerAngles = new Vector3(0, 0, -angle);
        _lightDir.localEulerAngles = new Vector3(0, 0, -angle);
    }

    void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }

    void CheckTrigger()
    {
        if(_currentPosX > -3f && _currentPosX < 6f)
        {
            float currentAngle = _rotateHandler.CurrentRotate;
            float currentLight = _lightHandler.CurrentBright;
            if(currentAngle > 120 && currentAngle <= 210 && currentAngle > 1.6f && currentLight > 1.645f)
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
        _rotateHandler.SetValue(_defaultAngle);
        _scaleHandler.SetValue(_defaultScale);
        _lightHandler.SetValue(_defaultBright);

        // DisableControl();
    }


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
