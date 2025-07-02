using Define;
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

    private PlayerAnimation _animation;

    private void Awake()
    {
        ComponentHelper.TryGetOrAddComponent<SpeedHandler>(ref _speedHandler, gameObject);
        _graphicHandler = GetComponent<GraphicHandler>();

        _speedHandler.Init(1);
        _speedHandler.OnSetValue += ChangeSpeed;

        _graphicHandler.Init(_defaultGraphicType);
        _graphicHandler.OnSetValue += ChangeGraphic;

        _animation = GetComponentInChildren<PlayerAnimation>();

        _movement2D = GetComponent<Movement2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.gravityScale = 0;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DisableControl();
        ResetFeature();
    }

    // Update is called once per frame
    void Update()
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
        _animation.ActivateAnimation(true);
    }

    public void DisableControl()
    {
        _enableMove = false;
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _movement2D.MoveDir = Vector3.zero;
        _animation.ActivateAnimation(false);
    }

    private void ChangeSpeed(int step)
    {
        float multiple = 0.5f + 0.5f * step;
        _movement2D.MultiplySpeed(multiple);
    }

    private void ChangeGraphic(int type)
    {
        shadow.SetActive(type == 1);
    }
}
