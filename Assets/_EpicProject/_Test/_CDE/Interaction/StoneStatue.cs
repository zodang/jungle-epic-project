using UnityEngine;

public class StoneStatue : MonoBehaviour, IFeatureResetable, ILightAdjustable, IRotatable, IScalable, IControllable
{
    // IFeatureResetable
    private float _defaultLight = 0f;
    private float _defaultRotation = 110f;
    private float _defaultScale = 1f;
        
    // ILightAdjustable
    private float _minBright = 0.5f;
    private float _maxBright = 3f;
    private float _currentBright;
    
    // IRotatable
    private float _minAngle = 0f;
    private float _maxAngle = 359f;
    private float _currentAngle;
    
    // IScalable
    private float _minScale = 0.8f;
    private float _maxScale = 2.5f;
    private float _currentScale;
    
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
        ((ILightAdjustable)this).SetValue(_defaultLight);
        ((IScalable)this).SetValue(_defaultScale);
        ((IRotatable)this).SetValue(_defaultRotation);
    }
    #endregion

    #region ILightAdjustable
    float ILightAdjustable.GetMinValue() => _minBright;

    float ILightAdjustable.GetMaxValue () => _maxBright;

    float ILightAdjustable.GetCurrentValue() => _currentBright;

    void ILightAdjustable.SetValue(float value)
    {
        _currentBright = value;
        Twinkle(_currentBright);
    }
    
    private void Twinkle(float brightness)
    {
        twinkleLv1.SetActive(brightness >= 1.5f);
        twinkleLv2.SetActive(brightness >= 2.5f);
    }
    #endregion
    
    #region IRotatable
    float IRotatable.GetMinValue() => _minAngle;

    float IRotatable.GetMaxValue() => _maxAngle;

    float IRotatable.GetCurrentValue() => _currentAngle;
    
    void IRotatable.SetValue(float value)
    {
        _currentAngle = value;
        SetRotate(_currentAngle);
    }
    
    void SetRotate(float angle)
    {
        model.localEulerAngles = new Vector3(0, 0, -angle);
    }
    #endregion
    
    #region IScalable
    float IScalable.GetMinValue() => _minScale;

    float IScalable.GetMaxValue() => _maxScale;
    float IScalable.GetCurrentValue() => _currentScale;
    void IScalable.SetValue(float value)
    {
        _currentScale = value;
        transform.localScale = new Vector3(_currentScale, _currentScale, _currentScale);
    }
    #endregion
    
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