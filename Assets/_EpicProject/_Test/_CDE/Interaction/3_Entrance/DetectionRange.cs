using System;
using UnityEngine;

public class DetectionRange : MonoBehaviour
{
    public Action OnPlayerDetected;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        // Todo: player의 Graphic 상태 검사
    }
}
