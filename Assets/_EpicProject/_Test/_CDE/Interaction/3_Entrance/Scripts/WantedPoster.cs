using Define;
using UnityEngine;

public class WantedPoster : MonoBehaviour, IFeatureResetable, IControllable
{
    // IControllable
    private Rigidbody2D _rigidbody2D;
    private Movement2D _movement2D;
    private bool _enableMove;
    
    // ISpeedChangeable
    private SpeedHandler _speedHandler;
    private readonly int _defaultSpeedStep = 1;
    
    // IGraphicChangeable
    private GraphicHandler _graphicHandler;
    private readonly GraphicType _defaultGraphicType = GraphicType.Middle;

    private void Awake()
    {
        ComponentHelper.TryGetOrAddComponent<SpeedHandler>(ref _speedHandler, gameObject);
        _graphicHandler = GetComponent<GraphicHandler>();
        
        _speedHandler.Init(_defaultSpeedStep);
        _speedHandler.OnSetValue += ChangeSpeed;
        
        _graphicHandler.Init(_defaultGraphicType);
        
        _movement2D = GetComponent<Movement2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.gravityScale = 0;
    }
    
    private void Start()
    {
        DisableControl();
        ResetFeature(); 
    }

    #region FeatureSetting

    private void Update()
    {
        if (!_enableMove) return;

        _movement2D.MoveDir = StageManager.Instance.InputManager.MoveInput;
        _graphicHandler?.SetSpriteDirection(StageManager.Instance.InputManager.MoveInput);
    }

    public void EnableControl()
    {
        _enableMove = true;
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _movement2D.MoveDir = Vector3.zero;
    }

    public void DisableControl()
    {
        _enableMove = false;
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _movement2D.MoveDir = Vector3.zero;
    }
    
    private void ChangeSpeed(int step)
    {
        float multiple = 0.5f + 0.5f * step;
        _movement2D.MultiplySpeed(multiple);
    }
    
    public void ResetFeature()
    {
        _speedHandler.SetValue(_defaultSpeedStep);
        _graphicHandler.SetValue(_defaultGraphicType);
    }

    #endregion FeatureSetting
}
