using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;

public class Sun : MonoBehaviour, ILightAdjustable, IRotatable
{
    // ILightAdjustable: 빛 밝기 관련
    private float _minBright = 0.1f;
    private float _maxBright = 3f;
    private float _currentBright;

    // IRotatable: 회전 관련
    private float _minAngle = 0f;
    private float _maxAngle = 359f;
    private float _currentAngle;

    private Transform _lightDir;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _lightDir = transform.GetChild(1);

        ((ILightAdjustable)this).SetValue(1f);
        ((IRotatable)this).SetValue(60f);
    }

    void AdjustLight(float brightness)
    {
        // Color Alpha
        float alpha = brightness / 2.0f;
        Color color = _lightDir.GetComponentInChildren<SpriteRenderer>().color;
        color.a = alpha;
        _lightDir.GetComponentInChildren<SpriteRenderer>().color = color;

        // Brightness Trigger
        if (brightness >= 2.0f && (_currentAngle >= 130 && _currentAngle <= 180))
        {
            Debug.Log("녹음, 증발 호출");
            EvaporationHandler[] evaporations = FindObjectsByType<EvaporationHandler>(FindObjectsSortMode.None);
            for(int i = 0; i < evaporations.Length; i++)
            {
                evaporations[i].Evaporate();
            }
        }
        else if (brightness <= 0.2f)
        {
            Debug.Log("밤");
        }
    }

    // 0 ~ 359
    void SetRotate(float angle)
    {
        _lightDir.localEulerAngles = new Vector3(0, 0, -angle);
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
}
