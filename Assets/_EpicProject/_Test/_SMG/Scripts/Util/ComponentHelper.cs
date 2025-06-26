using Unity.VisualScripting;
using UnityEngine;

public static class ComponentHelper
{
    public static bool TryGetComponent<T>(ref T handler, Component target) where T: Component
    {
        if(handler.IsUnityNull())
        {
            handler = target.GetComponent<T>();
        }

        return !handler.IsUnityNull();
    }
}
