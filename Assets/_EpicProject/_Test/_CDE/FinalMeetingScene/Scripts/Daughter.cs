using System;
using UnityEngine;

public class Daughter : MonoBehaviour, IEmotionAvailable
{
    public event Action OnEmotionEnabled;
    
    public void EnableEmotion()
    {
        OnEmotionEnabled?.Invoke();
    }

    public void DisableEmotion()
    {
        
    }
}
