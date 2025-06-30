using UnityEngine;

public class EntranceDetectionGuard : MonoBehaviour
{
    private DetectionRange _range;

    private void Awake()
    {
        _range = GetComponentInChildren<DetectionRange>();
    }

    private void Start()
    {
        _range.OnPlayerDetected += WhenPlayerDetected;
    }

    private void WhenPlayerDetected()
    {
        // TODO: 플레이어 감지 시
    }
}
