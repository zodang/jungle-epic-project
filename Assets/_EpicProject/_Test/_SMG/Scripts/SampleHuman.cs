using Define;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class SampleHuman : MonoBehaviour, IFeatureResetable, IControllable
{
    private Transform _visualRoot;

    [SerializeField] private RotateHandler _rotateHandler;
    [SerializeField] private ScaleHandler _scaleHandler;
    [SerializeField] private LightHandler _lightHandler;
    [SerializeField] private SpeedHandler _speedHandler;
    [SerializeField] private GraphicHandler _graphicHandler;

    // IControllable
    private Rigidbody2D _rigidbody2D;
    private Movement2D _movement2D;
    private bool _enableMove;

    [Header("Rotate Setting")]
    public float DefaultRotate;
    public float MinRotate = 0f;
    public float MaxRotate = 359f;

    [Header("Scale Setting")]
    public float DefaultScale = 1f;
    public float MinScale = 0.5f;
    public float MaxScale = 2.0f;

    [Header("Light Setting")]
    public float DefaultLight = 0f;
    public float MinLight = 0f;
    public float MaxLight = 2f;
    private GameObject _TwinkleLv1;
    private GameObject _TwinkleLv2;

    [Header("Speed Setting")]
    public int DefaultSpeed = 1;

    [Header("Graphic Setting")]
    public GraphicType DefaultGraphicType = GraphicType.Middle;

    public event Action<bool> OnControlEnabled;
    public event Action<PlayerSkinType> OnPlayerTwinkled;

    private void Awake()
    {
        _movement2D = GetComponent<Movement2D>();

        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.gravityScale = 0;

        _visualRoot = transform.GetChild(0);

        ComponentHelper.TryGetOrAddComponent<RotateHandler>(ref _rotateHandler, gameObject);
        _rotateHandler.Init(MinRotate, MaxRotate, 1f);
        _rotateHandler.OnSetValue += SetRotate;

        ComponentHelper.TryGetOrAddComponent<ScaleHandler>(ref _scaleHandler, gameObject);
        _scaleHandler.Init(MinScale, MaxScale, 1f);
        _scaleHandler.OnSetValue += SetScale;

        ComponentHelper.TryGetOrAddComponent<LightHandler>(ref _lightHandler, gameObject);
        _lightHandler.Init(MinLight, MaxLight, 1f);
        _lightHandler.OnSetValue += Twinkle;
        _TwinkleLv1 = _visualRoot.GetChild(0).gameObject;
        _TwinkleLv2 = _visualRoot.GetChild(1).gameObject;

        ComponentHelper.TryGetOrAddComponent<SpeedHandler>(ref _speedHandler, gameObject);
        _speedHandler.Init(DefaultSpeed);
        _speedHandler.OnSetValue += ChangeSpeed;

        
        
    }

    void Update()
    {
        if (!_enableMove) return;

        Move();

        _graphicHandler?.SetSpriteDirection(StageManager.Instance.InputManager.MoveInput);
    }

    private void OnDestroy()
    {
        OnControlEnabled = null;
        OnPlayerTwinkled = null;
    }

    public void ResetFeature()
    {
        _rotateHandler.SetValue(DefaultRotate);
        _scaleHandler.SetValue(DefaultScale);
        _lightHandler.SetValue(DefaultLight);
        _speedHandler.SetValue(DefaultSpeed);
        _graphicHandler.SetValue(DefaultGraphicType);
    }

    public void EnableControl()
    {
        SetEnableControl(true);
    }

    public void DisableControl()
    {
        SetEnableControl(false);
    }

    void SetEnableControl(bool enable)
    {
        _enableMove = enable;
        _movement2D.MoveDir = Vector2.zero;
        _rigidbody2D.bodyType = enable ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
        OnControlEnabled?.Invoke(enable);
    }

    void Move()
    {
        _movement2D.MoveDir = StageManager.Instance.InputManager.MoveInput;
    }

    void SetRotate(float rotate)
    {
        _visualRoot.localEulerAngles = new Vector3(0, 0, -rotate);
    }

    void SetScale(float scale)
    {
        _visualRoot.localScale = new Vector3(scale, scale, 1f);
    }

    void Twinkle(float light)
    {
        if (!_TwinkleLv1.IsUnityNull())
        {
            _TwinkleLv1.SetActive(light >= 1.5f); // _TwinkleLv1 활성화
        }

        if (!_TwinkleLv2.IsUnityNull())
        {
            bool isBright = light >= 2.5f;
            _TwinkleLv2.SetActive(isBright); // _TwinkleLv2 활성화
            OnPlayerTwinkled?.Invoke(isBright ? PlayerSkinType.BaldHead : PlayerSkinType.Default); // 스킨 변경

            if (!StageBaseManager.Instance.IsUnityNull())
            {
                StageBaseManager.Instance.FlagManager.SetFlag("baldHead", isBright); // 예시로 baldHead 플래그 설정
            }
        }
    }

    void ChangeSpeed(int speedStep)
    {
        float multiple = (speedStep == 0) ? 0.5f : speedStep;
        _movement2D.MultiplySpeed(multiple);
    }

    
}
