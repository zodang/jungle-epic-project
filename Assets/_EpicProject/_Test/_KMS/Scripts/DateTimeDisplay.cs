using UnityEngine;
using TMPro;             // TextMeshPro 사용 시
// using UnityEngine.UI;  // Unity UI Text 사용 시

public class DateTimeDisplay : MonoBehaviour
{
    [Header("Assign your UI Text here")]
    [SerializeField] private TextMeshProUGUI dateTimeText;
    // [SerializeField] private Text dateTimeText; // Unity UI Text 용

    void Start()
    {
        if (dateTimeText == null)
            Debug.LogError("DateTimeText에 UI 컴포넌트를 할당하세요!");
    }

    void Update()
    {
        // 현재 로컬 시간(Asia/Seoul) 기준으로 포맷팅
        dateTimeText.text = System.DateTime.Now.ToString("yyyy.MM.dd / HH:mm");
    }
}
