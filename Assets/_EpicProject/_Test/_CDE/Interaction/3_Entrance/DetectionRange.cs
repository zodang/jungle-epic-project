using System;
using UnityEngine;

public class DetectionRange : MonoBehaviour
{
    public Action<GameObject> OnPlayerDetected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        OnPlayerDetected?.Invoke(other.gameObject);
    }
}
