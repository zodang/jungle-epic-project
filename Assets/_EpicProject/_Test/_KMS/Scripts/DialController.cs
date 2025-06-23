using UnityEngine;
using UnityEngine.EventSystems;

public class DialController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("마우스 오버 시 활성화할 백그라운드")]
    [SerializeField] private GameObject dialHandleBack;

    private void Start()
    {
        if (dialHandleBack != null)
            dialHandleBack.SetActive(false);
    }

    // 포인터가 이 UI 위로 진입했을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (dialHandleBack != null)
            dialHandleBack.SetActive(true);
    }

    // 포인터가 이 UI에서 벗어났을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        if (dialHandleBack != null)
            dialHandleBack.SetActive(false);
    }
}
