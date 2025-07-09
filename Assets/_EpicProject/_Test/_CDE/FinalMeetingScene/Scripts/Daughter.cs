using System;
using UnityEngine;

public class Daughter : MonoBehaviour, IEmotionAvailable
{
    public event Action OnEmotionEnabled;
    
    public void EnableEmotion()
    {
        Debug.Log("감정블록 장착");
        OnEmotionEnabled?.Invoke();
    }

    public void DisableEmotion()
    {
        
    }
}
