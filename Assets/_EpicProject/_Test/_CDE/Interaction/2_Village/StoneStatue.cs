using UnityEngine;
using UnityEngine.UIElements;

public class StoneStatue : MonoBehaviour, IFeatureResetable, IControllable
{
    [SerializeField] private RotateHandler _rotateHandler;
    [SerializeField] private ScaleHandler _scaleHandler;
    [SerializeField] private LightHandler _lightHandler;

    // IFeatureResetable
    private float _defaultLight = 0f;
    private float _defaultRotation = 110f;
    private float _defaultScale = 1f;
        
    // ILightAdjustable
    private float _minBright = 0.5f;
    private float _maxBright = 3f;
    
    // IRotatable
    private float _minAngle = 0f;
    private float _maxAngle = 359f;
    
    // IScalable
    private float _minScale = 0.8f;
    private float _maxScale = 2.5f;
    
    // IControllable
    private bool _enableMove;    
    private Rigidbody2D _rigidbody2D;
    private Movement2D _movement2D;

    [SerializeField] private Transform model;
    [SerializeField] private GameObject foot;
    [SerializeField] private GameObject twinkleLv1;
    [SerializeField] private GameObject twinkleLv2;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _movement2D = GetComponent<Movement2D>();
        
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody2D.gravityScale = 0f;
        _rigidbody2D.freezeRotation = true;

        ComponentHelper.TryGetOrAddComponent<RotateHandler>(ref _rotateHandler, gameObject);
        ComponentHelper.TryGetOrAddComponent<ScaleHandler>(ref _scaleHandler, gameObject);
        ComponentHelper.TryGetOrAddComponent<LightHandler>(ref _lightHandler, gameObject);

        _rotateHandler.Init(_minAngle, _maxAngle, 1f);
        _rotateHandler.OnSetValue += SetRotate;

        _scaleHandler.Init(_minScale, _maxScale, 0f);
        _scaleHandler.OnSetValue += SetScale;

        _lightHandler.Init(_minBright, _maxBright, 1f);
        _lightHandler.OnSetValue += Twinkle;

        ResetFeature();
    }

    private void Start()
    {
        //foot.SetActive(false);
        twinkleLv1.SetActive(false);
        twinkleLv2.SetActive(false);
    }
    
    private void Update()
    {
        if(_enableMove)
        {
            _movement2D.MoveDir = StageManager.Instance.InputManager.MoveInput; 
        }
    }

    #region IFeatureResetable
    public void ResetFeature()
    {
        _rotateHandler.SetValue(_defaultRotation);
        _scaleHandler.SetValue(_defaultScale);
        _lightHandler.SetValue(_defaultLight);
    }
    #endregion

    private void Twinkle(float brightness)
    {
        twinkleLv1.SetActive(brightness >= 1.5f);
        twinkleLv2.SetActive(brightness >= 2.5f);
    }
    
    void SetRotate(float angle)
    {
        model.localEulerAngles = new Vector3(0, 0, -angle);
    }
    
    void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }

    #region IControllable
    public void EnableControl()
    {
        _enableMove = true;
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        //foot.SetActive(true);
        _movement2D.MoveDir = Vector3.zero;
    }

    public void DisableControl()
    {
        _enableMove = false;
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        //foot.SetActive(false);
        _movement2D.MoveDir = Vector3.zero;
    }
    #endregion
}