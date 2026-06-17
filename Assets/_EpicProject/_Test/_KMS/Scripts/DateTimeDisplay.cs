using UnityEngine;
using TMPro;

public class DateTimeDisplay : MonoBehaviour
{
    [Header("Assign your UI Text here")]
    [SerializeField] private TextMeshProUGUI dateTimeText;
    private const float UpdateInterval = 1f;
    private float _nextUpdateTime;

    void Start()
    {
        if (dateTimeText == null)
            Debug.LogError("DateTimeText에 UI 컴포넌트를 할당하세요!");
    }

    void Update()
    {
        // 현재 로컬 시간 기준으로 포맷팅
        if (dateTimeText == null || Time.time < _nextUpdateTime) return;

        _nextUpdateTime = Time.time + UpdateInterval;
        dateTimeText.text = System.DateTime.UtcNow.ToLocalTime().ToString("yyyy.MM.dd / HH:mm");
    }
}
