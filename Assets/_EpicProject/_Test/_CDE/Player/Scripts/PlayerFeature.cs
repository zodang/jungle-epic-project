using Define;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerFeature : MonoBehaviour, IControllable, IFeatureResetable
{
    // Action
    public event Action<bool> OnControlEnabled;
    public event Action<PlayerSkinType> OnPlayerTwinkled;

    private Rigidbody2D _rigidbody2D;
    private Movement2D _movement2D;
    private Vector2 _moveInput;

    public bool _enableMove;
    private float _minScale = 0.5f;
    private float _maxScale = 2.0f;

    [SerializeField] private RotateHandler _rotateHandler;
    [SerializeField] private ScaleHandler _scaleHandler;
    [SerializeField] private LightHandler _lightHandler;
    [SerializeField] private SpeedHandler _speedHandler;
    private GraphicHandler _graphicHandler;

    // ILightAdjustable
    private float _minBright = 1f;
    private float _maxBright = 3f;
    private float _currentBright;

    // IRotatable
    private float _minRotate = 0f;
    private float _maxRotate = 359f;
    private float _currentRotate;
    
    // ISpeedChangeable
    private readonly int _defaultSpeedStep = 1;
    
    // IGraphicChangeable
    private GraphicType _defaultGraphicType = GraphicType.Middle;

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

        ComponentHelper.TryGetOrAddComponent<RotateHandler>(ref _rotateHandler, gameObject);
        ComponentHelper.TryGetOrAddComponent<ScaleHandler>(ref _scaleHandler, gameObject);
        ComponentHelper.TryGetOrAddComponent<LightHandler>(ref _lightHandler, gameObject);
        ComponentHelper.TryGetOrAddComponent<SpeedHandler>(ref _speedHandler, gameObject);
        TryGetComponent<GraphicHandler>(out _graphicHandler);

        _rotateHandler.Init(_minRotate, _maxRotate, 1f);
        _rotateHandler.OnSetValue += Rotate;

        _scaleHandler.Init(_minScale, _maxScale, 1f);
        _scaleHandler.OnSetValue += Scale;

        _lightHandler.Init(_minBright, _maxBright, 1f);
        _lightHandler.OnSetValue += Twinkle;
        
        _speedHandler.Init(_defaultSpeedStep);
        _speedHandler.OnSetValue += ChangeSpeed;
    }

    private void Start()
    {
        ResetFeature();
    }

    private void Update()
    {
        if (!_enableMove) return;
        Move();
        _graphicHandler?.SetSpriteDirection(StageManager.Instance.InputManager.MoveInput);
    }

    void Rotate(float angle)
    {
        _model.localEulerAngles = new Vector3(0, 0, -angle);
    }

    void Scale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, 1f);
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
    
    private void ChangeSpeed(int step)
    {
        float multiple = (step == 0) ? 0.5f : step;
        _movement2D.MultiplySpeed(multiple);
    }

    public void ResetFeature()
    {
        _rotateHandler.SetValue(0f);
        _scaleHandler.SetValue(1f);
        _lightHandler.SetValue(1f);
        _speedHandler.SetValue(_defaultSpeedStep);
        _graphicHandler.SetValue(_defaultGraphicType);
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

    private void OnDestroy()
    {
        OnControlEnabled = null;
        OnPlayerTwinkled = null;
    }
}
