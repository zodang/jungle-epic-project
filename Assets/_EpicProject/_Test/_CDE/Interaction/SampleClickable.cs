using UnityEngine;

public class SampleClickable : MonoBehaviour, IFeatureResetable, IControllable
{
    // IControllable
    private Rigidbody2D _rigidbody2D;
    private Movement2D _movement2D;
    private bool _enableMove;
    
    // IScalable
    private ScaleHandler _scaleHandler;
    private float _minScale = 0.1f;
    private float _maxScale = 2f;
    private float _currentScale;
    
    // ILightAdjustable
    private LightHandler _lightHandler;
    private float _minBright = 0.5f;
    private float _maxBright = 3f;

    private void Awake()
    {
        ComponentHelper.TryGetOrAddComponent<ScaleHandler>(ref _scaleHandler, gameObject);
        ComponentHelper.TryGetOrAddComponent<LightHandler>(ref _lightHandler, gameObject);
        
        _movement2D = GetComponent<Movement2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.gravityScale = 0;
        
        _scaleHandler.Init(_minScale, _maxScale, 0f);
        _scaleHandler.OnSetValue += SetScale;

        _lightHandler.Init(_minBright, _maxBright, 1f);
        _lightHandler.OnSetValue += SetLight;

        DisableControl();
        ResetFeature();
    }

    #region FeatureSetting

    private void Update()
    {
        if (_enableMove)
        {
            _movement2D.MoveDir = StageManager.Instance.InputManager.MoveInput;
        }
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
    
    private void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }
    
    private void SetLight(float bright)
    {
        // Todo: 빛 기능
    }

    public void ResetFeature()
    {
        // Todo: 리셋 기능
    }

    #endregion FeatureSetting
}