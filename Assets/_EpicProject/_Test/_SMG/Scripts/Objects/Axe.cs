using SMG;
using UnityEngine;
using UnityEngine.InputSystem;

public class Axe : MonoBehaviour, IFeatureResetable, IScalable, IRotatable, IControllable, ILightAdjustable
{
    // IControllable
    private bool _enableMove;

    // IScalable
    private float _minScale = 0.1f;
    private float _maxScale = 6f;
    private float _currentScale;

    // IRotatable
    private float _minRotate = 0f;
    private float _maxRotate = 359f;
    private float _currentRotate;

    // ILightAdjustable
    private float _minBright = 0.5f;
    private float _maxBright = 3f;
    private float _currentBright;

    private Transform _model;
    //private GameObject _spriteOnly;
    private GameObject _bridgeSide;
    private GameObject _footCollider;
    private GameObject _TwinkleLv1;
    private GameObject _TwinkleLv2;

    Movement movement;
    Movement2D movement2D;

    // Input
    InputAction moveAction;



    private void Awake()
    {
        _model = transform.GetChild(0);
        //_spriteOnly = transform.GetChild(1).gameObject;
        _bridgeSide = _model.GetChild(3).gameObject;
        _footCollider = _model.GetChild(1).gameObject;
        _TwinkleLv1 = _model.GetChild(4).gameObject;
        _TwinkleLv2 = _model.GetChild(5).gameObject;

        movement = GetComponent<Movement>();
        movement2D = GetComponent<Movement2D>();
    }

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");

        ResetFeature();
    }

    private void Update()
    {
        if(_enableMove)
        {
            //movement.Move(moveAction.ReadValue<Vector2>());
            movement2D.MoveDir = InputManager.Instance.MoveInput; 
        }
    }

    //public void EnableActive()
    //{
    //    _model.gameObject.SetActive(true);
    //    _spriteOnly.SetActive(false);
    //}

    //public void DisableActive()
    //{
    //    _model.gameObject.SetActive(false);
    //    _spriteOnly.SetActive(true);
    //}

    void Resize(float scale)
    {
        _model.localScale = new Vector3(scale, scale, scale);
    }

    void Rotate(float angle)
    {
        _model.localEulerAngles = new Vector3(0f, 0, -angle);
    }

    void Twinkle(float bright)
    {
        if(bright >= 1.5f)
        {
            _TwinkleLv1.SetActive(true);
        }
        else
        {
            _TwinkleLv1.SetActive(false);
        }

        if(bright >= 2.5f)
        {
            _TwinkleLv2.SetActive(true);
        }
        else
        {
            _TwinkleLv2.SetActive(false);
        }
            
    }

    // IFeatureResetable
    public void ResetFeature()
    {
        ((IScalable)this).SetValue(1f);
        ((IRotatable)this).SetValue(0f);
        ((ILightAdjustable)this).SetValue(1f);
        // DisableControl();
    }

    #region IControllable
    public void EnableControl()
    {
        _enableMove = true;
        _bridgeSide.SetActive(!_enableMove);
        _footCollider.SetActive(_enableMove);
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        movement2D.MoveDir = Vector2.zero;
    }

    public void DisableControl()
    {
        _enableMove = false;
        _bridgeSide.SetActive(!_enableMove);
        _footCollider.SetActive(_enableMove);
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        movement2D.MoveDir = Vector2.zero;
    }
    #endregion

    #region IScalable
    float IScalable.GetMinValue() => _minScale;
    
    float IScalable.GetMaxValue() => _maxScale;

    float IScalable.GetCurrentValue() => _currentScale;

    void IScalable.SetValue(float value)
    {
        _currentScale = value;
        Resize(_currentScale);
    }
    #endregion

    #region IRotatable
    float IRotatable.GetMinValue() => _minRotate;

    float IRotatable.GetMaxValue() => _maxRotate;

    float IRotatable.GetCurrentValue() => _currentRotate;

    void IRotatable.SetValue(float value)
    {
        _currentRotate = value;
        Rotate(_currentRotate);
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
    #endregion
}
