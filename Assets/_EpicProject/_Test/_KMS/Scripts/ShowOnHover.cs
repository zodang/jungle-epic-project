using UnityEngine;
using UnityEngine.UI;         // Graphic
using UnityEngine.EventSystems;

public class ShowOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject hoverPrefab;
    [SerializeField] private Canvas parentCanvas;

    private GameObject _instance;
    private RectTransform _instRect;

    private void Awake()
    {
        if (parentCanvas == null)
            parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_instance != null || hoverPrefab == null) return;

        // 1) Instantiate
        _instance = Instantiate(hoverPrefab, parentCanvas.transform);
        _instRect = _instance.GetComponent<RectTransform>();

        // 2) CanvasGroup 확보
        var cg = _instance.GetComponent<CanvasGroup>();
        if (cg == null) cg = _instance.AddComponent<CanvasGroup>();
        // 이게 false 면 그래픽이 레이캐스트를 가로채지 않음
        cg.blocksRaycasts = false;

        // 3) 자식 Graphic(이미지/텍스트) RaycastTarget 끄기
        foreach (var gfx in _instance.GetComponentsInChildren<Graphic>())
            gfx.raycastTarget = false;

        // 4) 초기 위치 세팅
        _instRect.position = Input.mousePosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_instance != null)
        {
            Destroy(_instance);
            _instance = null;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // 클릭 시 인스턴스 제거
        if (_instance != null)
        {
            Destroy(_instance);
            _instance = null;
        }
    }
    private void Update()
    {
        if (_instance != null)
            _instRect.position = Input.mousePosition;
    }
}
