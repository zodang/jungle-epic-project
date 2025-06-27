using Unity.VisualScripting;
using UnityEngine;

public static class ComponentHelper
{
    public static bool TryGetComponent<T>(ref T handler, GameObject target) where T : Component
    {
        if (handler.IsUnityNull())
        {
            handler = target.GetComponent<T>();
        }

        return !handler.IsUnityNull();
    }

    public static void TryGetOrAddComponent<T>(ref T handler, GameObject target) where T : Component
    {
        if (!TryGetComponent<T>(ref handler, target))
        {
            handler = target.AddComponent<T>();
        }
    }
}
