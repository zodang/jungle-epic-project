using Define;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerFeature : MonoBehaviour, IControllable, IScalable, ILightAdjustable, IRotatable, IFeatureResetable
{
    // Action
    public event Action<bool> OnControlEnabled;
    public event Action<PlayerSkinType> OnPlayerTwinkled;

    private Rigidbody2D _rigidbody2D;
    private Movement2D _movement2D;
    private Vector2 _moveInput;

    public bool _enableMove;
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
    
    private void Awake()
    {
        TryGetComponent<Rigidbody2D>(out _rigidbody2D);
        TryGetComponent<Movement2D>(out _movement2D);

        _model = transform.GetChild(1);
        _TwinkleLv1 = _model.GetChild(0).gameObject;
        _TwinkleLv2 = _model.GetChild(1).gameObject;
        
        ResetFeature();
    }

    private void Update()
    {
        if (!_enableMove) return;
        Move();
    }

    //private void FixedUpdate()
    //{
    //    _rigidbody2D.linearVelocity = _moveInput * _speed;
    //}

    void Rotate(float angle)
    {
        _model.localEulerAngles = new Vector3(0, 0, -angle);
    }

    void Twinkle(float bright)
    {
        if(!_TwinkleLv1.IsUnityNull())
        {
            _TwinkleLv1.SetActive(bright >= 1.5f); // _TwinkleLv1 활성화
        }
        
        if(!_TwinkleLv2.IsUnityNull())
        {
            bool isBright = bright >= 2.5f;
            _TwinkleLv2.SetActive(isBright); // _TwinkleLv2 활성화
            OnPlayerTwinkled?.Invoke(isBright? PlayerSkinType.BaldHead : PlayerSkinType.Default); // 스킨 변경
            
            if(!StageBaseManager.Instance.IsUnityNull())
            {
                StageBaseManager.Instance.FlagManager.SetFlag("baldHead", isBright); // 예시로 baldHead 플래그 설정
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
        if (!_movement2D.IsUnityNull())
        {
            _movement2D.MoveDir = Vector2.zero;
        }
        else
        {
            _moveInput = Vector2.zero;
        }
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        OnControlEnabled?.Invoke(true);
    }

    public void DisableControl()
    {
        _enableMove = false;
        if (!_movement2D.IsUnityNull())
        {
            _movement2D.MoveDir = Vector2.zero;
        }
        else
        {
            _moveInput = Vector2.zero;
        }
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        OnControlEnabled?.Invoke(false);
    }
    private void Move()
    {
        if (!_movement2D.IsUnityNull())
        {
            _movement2D.MoveDir = StageManager.Instance.InputManager.MoveInput;
        }
        else
        {
            _moveInput = StageManager.Instance.InputManager.MoveInput;
        }
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

    private void OnDestroy()
    {
        OnControlEnabled = null;
        OnPlayerTwinkled = null;
    }
}
