using UnityEngine;

public class CaveFairy : MonoBehaviour, IFeatureResetable, IControllable
{
    [SerializeField] private Transform model;
    [SerializeField] private GameObject foot;
    
    // IFeatureResettable
    private float _defaultLight = 0f;
    private float _defaultRotation = 0f;
    private float _defaultScale = 1f;
    
    // ILightAdjustable
    private LightHandler _lightHandler;
    private float _minBright = 0.5f;
    private float _maxBright = 3f;
    private float _currentBright;
    
    // IScalable
    private ScaleHandler _scaleHandler;
    private float _minScale = 0.8f;
    private float _maxScale = 2.5f;
    private float _currentScale;
    
    // IControllable
    private Rigidbody2D _rigidbody2D;
    private Movement2D _movement2D;
    private bool _enableMove;
    
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
        if(_enableMove)
        {
            _movement2D.MoveDir = StageManager.Instance.InputManager.MoveInput; 
        }
    }

    private void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }

    private void SetLight(float bright)
    {
        // Todo: Light 변경 효과
    }
    
    public void EnableControl()
    {
        _enableMove = true;
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        foot.SetActive(true);
        _movement2D.MoveDir = Vector3.zero;
    }

    public void DisableControl()
    {
        _enableMove = false;
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        foot.SetActive(false);
        _movement2D.MoveDir = Vector3.zero;
    }
    
    public void ResetFeature()
    {
        _scaleHandler.SetValue(_defaultScale);
        _lightHandler.SetValue(_defaultLight);
    }
    #endregion FeatureSetting
}
