using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;

public class Sun : MonoBehaviour, ILightAdjustable, IRotatable, IScalable, IControllable
{
    // ILightAdjustable: 빛 밝기 관련
    private float _minBright = 0.1f;
    private float _maxBright = 3f;
    private float _currentBright;

    // IRotatable: 회전 관련
    private float _minAngle = 0f;
    private float _maxAngle = 359f;
    private float _currentAngle;

    // IScalable
    private float _minScale = 0.8f;
    private float _maxScale = 1.2f;
    private float _currentScale;

    // IControllable
    private bool _enableMove;
    private float _speed = 5f;


    private Transform _lightDir;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _lightDir = transform.GetChild(1);

        ((ILightAdjustable)this).SetValue(1f);
        ((IRotatable)this).SetValue(60f);
        ((IScalable)this).SetValue(1f);
        DisableControl();
    }

    private void Update()
    {
        if(_enableMove)
        {
            Vector2 moveInput = new Vector2 (InputManager.Instance.MoveInput.x, 0);
            transform.Translate(moveInput * _speed * Time.deltaTime);
        }
    }

    void AdjustLight(float brightness)
    {
        // Color Alpha
        float alpha = brightness / 2.0f;
        Color color = _lightDir.GetComponentInChildren<SpriteRenderer>().color;
        color.a = alpha;
        _lightDir.GetComponentInChildren<SpriteRenderer>().color = color;

        // Brightness Trigger
        CheckTrigger();
    }

    // 0 ~ 359
    void SetRotate(float angle)
    {
        _lightDir.localEulerAngles = new Vector3(0, 0, -angle);
        // Brightness Trigger
        CheckTrigger();
    }

    void CheckTrigger()
    {
        if (_currentBright >= 2.0f && (_currentAngle >= 130 && _currentAngle <= 180))
        {
            Debug.Log("녹음, 증발 호출");
            EvaporationHandler[] evaporations = FindObjectsByType<EvaporationHandler>(FindObjectsSortMode.None);
            for (int i = 0; i < evaporations.Length; i++)
            {
                evaporations[i].Evaporate();
            }
        }
        else if (_currentBright <= 0.2f)
        {
            Debug.Log("밤");
        }
    }

    #region ILightAdjustable
    float ILightAdjustable.GetMinValue() => _minBright;

    float ILightAdjustable.GetMaxValue() => _maxBright;

    float ILightAdjustable.GetCurrentValue() => _currentBright;
    
    void ILightAdjustable.SetValue(float value)
    {
        _currentBright = value;
        AdjustLight(value);
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
    }

    public void DisableControl()
    {
        _enableMove = false;
    }
    #endregion
}
