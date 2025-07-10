using Define;
using UnityEngine;

public class BrokenEmotionBlock : MonoBehaviour, IControllable, IGraphicChangeable, ILightAdjustable, IRotatable, IScalable, ISpeedChangeable
{
    public void EnableControl()
    {
        throw new System.NotImplementedException();
    }

    public void DisableControl()
    {
        throw new System.NotImplementedException();
    }

    float ILightAdjustable.GetMinValue()
    {
        throw new System.NotImplementedException();
    }

    float IScalable.GetMaxValue()
    {
        throw new System.NotImplementedException();
    }

    float IScalable.GetCurrentValue()
    {
        throw new System.NotImplementedException();
    }

    public void SetValue(int value)
    {
        throw new System.NotImplementedException();
    }

    void IScalable.SetValue(float value)
    {
        throw new System.NotImplementedException();
    }

    float IScalable.GetMinValue()
    {
        throw new System.NotImplementedException();
    }

    float IRotatable.GetMaxValue()
    {
        throw new System.NotImplementedException();
    }

    float IRotatable.GetCurrentValue()
    {
        throw new System.NotImplementedException();
    }

    void IRotatable.SetValue(float value)
    {
        throw new System.NotImplementedException();
    }

    float IRotatable.GetMinValue()
    {
        throw new System.NotImplementedException();
    }

    float ILightAdjustable.GetMaxValue()
    {
        throw new System.NotImplementedException();
    }

    float ILightAdjustable.GetCurrentValue()
    {
        throw new System.NotImplementedException();
    }

    void ILightAdjustable.SetValue(float value)
    {
        throw new System.NotImplementedException();
    }

    GraphicType IGraphicChangeable.GetCurrentValue()
    {
        throw new System.NotImplementedException();
    }

    public void SetValue(GraphicType type)
    {
        throw new System.NotImplementedException();
    }

    float ISpeedChangeable.GetCurrentValue()
    {
        throw new System.NotImplementedException();
    }
}
