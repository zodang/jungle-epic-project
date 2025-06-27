using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private RectTransform originalCursor;
    [SerializeField] private GameObject leftCursor;
    [SerializeField] private GameObject rightCursor;
    
    private void Awake()
    {
        // 기본 커서 숨김
        Cursor.visible = false;
    }

    private void Update()
    {
        originalCursor.position = Input.mousePosition;

        // 좌클릭
        leftCursor.SetActive(Input.GetMouseButton(0));
        // 우클릭
        rightCursor.SetActive(Input.GetMouseButton(1));

    }
}
