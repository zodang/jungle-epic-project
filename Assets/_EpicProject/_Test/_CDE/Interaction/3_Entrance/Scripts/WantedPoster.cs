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

    private float _baseSpeed = 5.0f;
    private float _speed = 5.0f;
    private float _minPosX = 0.2f;
    private float _maxPosX = 2.5f;

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

        Vector2 moveInput = new Vector2 (StageManager.Instance.InputManager.MoveInput.x, 0);
        transform.Translate(moveInput * (_speed * Time.deltaTime));

        Vector3 pos = transform.localPosition;
        
        if(pos.x < _minPosX) pos.x = _minPosX;
        if(pos.x > _maxPosX) pos.x = _maxPosX;
        
        transform.localPosition = pos;
    }

    public void EnableControl()
    {
        _enableMove = true;
    }

    public void DisableControl()
    {
        _enableMove = false;
    }
    
    private void ChangeSpeed(int step)
    {
        float multiple = 0.5f + 0.5f * step;
        _speed = _baseSpeed * multiple;
    }
    
    public void ResetFeature()
    {
        _speedHandler.SetValue(_defaultSpeedStep);
        _graphicHandler.SetValue(_defaultGraphicType);
    }

    #endregion FeatureSetting
}
