using UnityEngine;
using UnityEngine.Rendering;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private RectTransform originalCursor;
    [SerializeField] private GameObject leftCursor;
    [SerializeField] private GameObject rightCursor;

    private float xOffset = -15f;
    private float yOffset = 5f;

    private void Awake()
    {
        // 기본 커서 숨김
        Cursor.visible = false;
    }

    private void Update()
    {
        originalCursor.position = Input.mousePosition + new Vector3(xOffset, yOffset, 0);

        // 좌클릭
        leftCursor.SetActive(Input.GetMouseButton(0));
        // 우클릭
        rightCursor.SetActive(Input.GetMouseButton(1));

    }
}
