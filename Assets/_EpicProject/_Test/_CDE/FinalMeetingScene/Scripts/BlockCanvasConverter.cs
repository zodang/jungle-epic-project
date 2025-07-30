using UnityEngine;
using UnityEngine.EventSystems;

public class BlockCanvasConverter : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    private Canvas _dragCanvas;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _dragCanvas = FindAnyObjectByType<EngineManager>().GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // SpaceScreen Canvas로 이동
        transform.SetParent(_dragCanvas.transform, false);

        _rectTransform.localScale = Vector3.one;
        _rectTransform.position = Vector2.zero;
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.position = Input.mousePosition;
    }
}
