using Define;
using System;
using UnityEngine;

public class Horse : MonoBehaviour, IControllable, IFeatureResetable
{
    [SerializeField] private SpeedHandler _speedHandler;
    [SerializeField] private GraphicHandler _graphicHandler;
    [SerializeField] private GameObject shadow;

    // IControllable
    private Rigidbody2D _rigidbody2D;
    private Movement2D _movement2D;
    private bool _enableMove;

    // ISpeedChangeable
    private readonly int _defaultSpeedStep = 3;

    // IGraphicChangeable
    private GraphicType _defaultGraphicType = GraphicType.Middle;
    private GraphicType _currentGraphicType;

    private PlayerAnimation _animation;
    
    public Action OnControlEnabled;
    public Action OnControlDisabled;

    private void Awake()
    {
        ComponentHelper.TryGetOrAddComponent<SpeedHandler>(ref _speedHandler, gameObject);
        _graphicHandler = GetComponent<GraphicHandler>();

        _speedHandler.Init(1);
        _speedHandler.OnSetValue += ChangeSpeed;

        _graphicHandler.Init(_defaultGraphicType);
        _graphicHandler.OnSetValue += ChangeGraphic;
        _currentGraphicType = _defaultGraphicType;

        _animation = GetComponentInChildren<PlayerAnimation>();

        _movement2D = GetComponent<Movement2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.gravityScale = 0;
    }

    private void Start()
    {
        DisableControl();
        ResetFeature();
        
        _animation.ActivateAnimation(true);
    }

    private void Update()
    {
        if (!_enableMove) return;
        _movement2D.MoveDir = StageManager.Instance.InputManager.MoveInput;
        _graphicHandler?.SetSpriteDirection(StageManager.Instance.InputManager.MoveInput);
    }

    public void ResetFeature()
    {
        _speedHandler.SetValue(_defaultSpeedStep);
        _graphicHandler.SetValue(_defaultGraphicType);
    }

    public void EnableControl()
    {
        _enableMove = true;
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _movement2D.MoveDir = Vector3.zero;
        
        OnControlEnabled?.Invoke();
    }

    public void DisableControl()
    {
        _enableMove = false;
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _movement2D.MoveDir = Vector3.zero;
        _graphicHandler.SetValue(_currentGraphicType);
        
        OnControlDisabled?.Invoke();
    }

    private void ChangeSpeed(int step)
    {
        // Speed 블록에 의한 속도 변경
        float multiple = 0.5f + 0.5f * step;
        _movement2D.MultiplySpeed(multiple);
    }

    private void ChangeGraphic(int type)
    {
        // 그림자 비활성화
        shadow.SetActive(type == 1);
        _currentGraphicType = (GraphicType)type;
    }
    
    public void SetMoveDirection(Vector2 dir)
    {
        // FSM 상태에서 이동
        _movement2D.MoveDir = dir;
    }
}
