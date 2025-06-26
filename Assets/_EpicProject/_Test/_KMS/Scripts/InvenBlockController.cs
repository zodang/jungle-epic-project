using UnityEngine;
using UnityEngine.EventSystems;

public class InvenBlockController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("마우스 오버 시 활성화할 백그라운드")]
    [SerializeField] private GameObject _blockInventoryBackGround;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_blockInventoryBackGround != null)
            _blockInventoryBackGround.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_blockInventoryBackGround != null)
            _blockInventoryBackGround.SetActive(true);

    }

    // 포인터가 이 UI에서 벗어났을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_blockInventoryBackGround != null)
            _blockInventoryBackGround.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_blockInventoryBackGround != null)
            _blockInventoryBackGround.SetActive(false);
    }

}
