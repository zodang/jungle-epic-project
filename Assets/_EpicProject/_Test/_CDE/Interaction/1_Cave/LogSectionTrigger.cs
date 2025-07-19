using System;
using UnityEngine;

public class LogSectionTrigger : MonoBehaviour
{
    private bool _isTriggered;
    public Action OnSectionTriggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isTriggered) return;
        
        // 통과 체크
        _isTriggered = true;
        OnSectionTriggered?.Invoke();
    }
}
