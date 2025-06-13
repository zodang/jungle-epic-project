using Define;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerFeature : MonoBehaviour, IControllable, IScalable, ILightAdjustable, IRotatable, IFeatureResetable
{
    private Rigidbody2D _rigidbody2D;
    private Vector2 _moveInput;

    public bool _enableMove;
    private float _speed = 5f;
    public float scaleMin = 0.5f, scaleMax = 2.0f;

    // ILightAdjustable
    private float _minBright = 1f;
    private float _maxBright = 3f;
    private float _currentBright;

    // IRotatable
    private float _minRotate = 0f;
    private float _maxRotate = 359f;
    private float _currentRotate;


    private Transform _model;
    private GameObject _TwinkleLv1;
    private GameObject _TwinkleLv2;
    private PlayerAnimation _playerAnimation;
    private void Awake()
    {
        TryGetComponent<Rigidbody2D>(out _rigidbody2D);

        _model = transform.GetChild(1);
        _TwinkleLv1 = _model.GetChild(0).gameObject;
        _TwinkleLv2 = _model.GetChild(1).gameObject;
        _playerAnimation = transform.GetComponentInChildren<PlayerAnimation>();
    }

    private void Start()
    {
        ResetFeature();
    }

    private void Update()
    {
        if (!_enableMove) return;
        Move();
    }

    private void FixedUpdate()
    {
        _rigidbody2D.linearVelocity = _moveInput * _speed;
    }

    void Rotate(float angle)
    {
        _model.localEulerAngles = new Vector3(0, 0, -angle);
    }

    void Twinkle(float bright)
    {
        if(!_TwinkleLv1.IsUnityNull())
        {
            if (bright >= 1.5f)
            {
                _TwinkleLv1.SetActive(true);
            }
            else
            {
                _TwinkleLv1.SetActive(false);
            }
        }
        
        if(!_TwinkleLv2.IsUnityNull())
        {
            if (bright >= 2.5f)
            {
                _TwinkleLv2.SetActive(true);
                _playerAnimation.ChangeSkin(PlayerSkinType.BaldHead);
                FlagManager.Instance.SetFlag("baldHead", true); // 예시로 baldHead 플래그 설정
            }
            else
            {
                _TwinkleLv2.SetActive(false);
                _playerAnimation.ChangeSkin(PlayerSkinType.Default);
                FlagManager.Instance.SetFlag("baldHead", false); // 예시로 baldHead 플래그 해제
            }
        }
    }

    public void ResetFeature()
    {
        ((IScalable)this).SetValue(1f);
        ((IRotatable)this).SetValue(0f);
        ((ILightAdjustable)this).SetValue(1f);
    }

    #region Control
    public void EnableControl()
    {
        _enableMove = true;
        _moveInput = Vector2.zero;
    }

    public void DisableControl()
    {
        _enableMove = false;
        _moveInput = Vector2.zero;
    }
    private void Move()
    {
        _moveInput = InputManager.Instance.MoveInput;

        //transform.Translate(moveInput * (_speed * Time.deltaTime));
    }
    #endregion

    #region Scale
    float IScalable.GetMinValue() => scaleMin;
    float IScalable.GetMaxValue() => scaleMax;
    float IScalable.GetCurrentValue() => transform.localScale.x;
    void IScalable.SetValue(float v)
    {
        transform.localScale = new Vector3(v, v, 1f);
    }
    #endregion

    #region ILightAdjustable
    float ILightAdjustable.GetMinValue() => _minBright;

    float ILightAdjustable.GetMaxValue() => _maxBright;

    float ILightAdjustable.GetCurrentValue() => _currentBright;
    
    void ILightAdjustable.SetValue(float value)
    {
        _currentBright = value;
        Twinkle(_currentBright);
    }
    #endregion

    #region IRotatable
    float IRotatable.GetMinValue() => _minRotate;

    float IRotatable.GetMaxValue() => _maxRotate;

    float IRotatable.GetCurrentValue() => _currentRotate;
    
    void IRotatable.SetValue(float value)
    {
        _currentRotate = value;
        Rotate(_currentRotate);
    }
    #endregion



}
