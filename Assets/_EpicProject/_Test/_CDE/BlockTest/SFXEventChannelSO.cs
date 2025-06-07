using Define;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SFXEventChannelSO", menuName = "Scriptable Objects/SFXEventChannelSO")]
public class SFXEventChannelSO : ScriptableObject
{
    public event Action<Sfx> OnEventRaised;

    public void RaiseEvent(Sfx sfx)
    {
        OnEventRaised?.Invoke(sfx);
    }
}
