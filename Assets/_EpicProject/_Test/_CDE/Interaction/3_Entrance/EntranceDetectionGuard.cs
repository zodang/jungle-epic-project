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
        _range.OnPlayerDetected += WhenDetectPlayer;
    }

    private void WhenDetectPlayer()
    {
        // TODO: 플레이어 감지 시
    }
}
