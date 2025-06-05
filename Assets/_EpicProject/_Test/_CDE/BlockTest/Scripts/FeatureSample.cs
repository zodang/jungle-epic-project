using UnityEngine;

public class FeatureSample : MonoBehaviour, IControllable, ILightAdjustable, IScalable, IRotatable
{
    private bool _canMove = false;
    
    public float scaleMin = 0.5f, scaleMax = 2.0f;
    public float rotationMin = 0f, rotationMax = 360f;
    public float intensityMin = 0f, intensityMax = 10f;
    
    private void Update()
    {
        if (!_canMove) return;
        // Move
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        Vector3 move = new Vector3(h, v, 0f);
        transform.position += move * (5f * Time.deltaTime);
    }

    #region IControllable

    public void EnableControl()
    {
        _canMove = true;
    }

    public void DisableControl()
    {
        _canMove = false;
    }

    #endregion

    #region IScalable
    float IScalable.GetMinValue() => scaleMin;
    float IScalable.GetMaxValue() => scaleMax;
    float IScalable.GetCurrentValue() => transform.localScale.x;
    void IScalable.SetValue(float v)
    {
        transform.localScale = new Vector3(v, v, 1f);
    }
    #endregion

    #region IRotatable

    float IRotatable.GetMinValue() => rotationMin;
    float IRotatable.GetMaxValue() => rotationMax;
    float IRotatable.GetCurrentValue() => transform.eulerAngles.z;
    void IRotatable.SetValue(float v)
    {
        Vector3 euler = transform.eulerAngles;
        euler.z = v;
        transform.eulerAngles = euler;
    }

    #endregion
    
    #region ILightIntensityReceiver
    float ILightAdjustable.GetMinValue() => intensityMin;
    float ILightAdjustable.GetMaxValue() => intensityMax;
    float ILightAdjustable.GetCurrentValue()
    {
        var light = GetComponent<Light>();
        return light != null ? light.intensity : 0f;
    }
    void ILightAdjustable.SetValue(float value) { }
    #endregion
}