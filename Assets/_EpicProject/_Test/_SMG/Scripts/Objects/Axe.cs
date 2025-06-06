using SMG;
using UnityEngine;
using UnityEngine.InputSystem;

public class Axe : MonoBehaviour, IScalable, IRotatable, IControllable
{
    // IControllable
    private bool _enableMove;

    // IScalable
    private float _minScale = 0.1f;
    private float _maxScale = 10f;
    private float _currentScale;

    // IRotatable
    private float _minRotate = 0f;
    private float _maxRotate = 359f;
    private float _currentRotate;

    private Transform _model;
    private GameObject _bridge;
    private GameObject _footCollider;

    Movement movement;

    // Input
    InputAction moveAction;



    private void Awake()
    {
        _model = transform.GetChild(0);
        _bridge = _model.GetChild(2).gameObject;
        _footCollider = _model.GetChild(3).gameObject;

        movement = GetComponent<Movement>();

        ((IScalable)this).SetValue(1f);
        ((IRotatable)this).SetValue(0f);
    }

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    private void Update()
    {
        if(_enableMove)
        {
            movement.Move(moveAction.ReadValue<Vector2>());
        }
    }


    void Resize(float scale)
    {
        _model.localScale = new Vector3(scale, scale, scale);
    }

    void Rotate(float angle)
    {
        _model.localEulerAngles = new Vector3(0f, 0, -angle);
    }

    #region 
    public void EnableControl()
    {
        _enableMove = true;
        _bridge.SetActive(!_enableMove);
        _footCollider.SetActive(_enableMove);
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
    }

    public void DisableControl()
    {
        _enableMove = false;
        _bridge.SetActive(!_enableMove);
        _footCollider.SetActive(_enableMove);
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
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
}
