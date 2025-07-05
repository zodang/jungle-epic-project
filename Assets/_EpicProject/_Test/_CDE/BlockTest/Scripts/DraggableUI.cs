using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public Action<bool> OnDragEndedInHome;
    
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private Vector2 _offset;
    
    private Transform _originalParent;
    private Transform _prevParent;

    public static event Action OnDragBeginEngine;
    public static event Action OnDragEndEngine;
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _originalParent = transform.parent;
    }
    
    private void OnDestroy()
    {
        OnDragEndedInHome = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //이벤트 Invoke 할 예정 드래그가 시작되면 Tap이 flik 되는 기능 활성화
        OnDragBeginEngine?.Invoke();
        _prevParent = transform.parent;
        
        transform.SetParent(_originalParent);
        transform.SetAsLastSibling();
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform, eventData.position, eventData.pressEventCamera, out _offset);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 pointerPosition))
        {
            _rectTransform.anchoredPosition = pointerPosition - _offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //이벤트 끝나는대로 끝내는 이벤트 호출 예정
        OnDragEndEngine?.Invoke();
        TabHome targetTabHome = GetDropTabHome(eventData);

        if (targetTabHome == null) {
            OnDragEndedInHome?.Invoke(false);
            return;
        }

        EngineController myEngine = GetComponent<EngineController>();

        if (targetTabHome.transform.childCount == 0)
        {
            // 빈 TabHome으로 이동
            HandleDropToEmptyTabHome(targetTabHome, myEngine);
            
        }
        else if (IsSwap(myEngine, targetTabHome))
        {
            HandleSwap(targetTabHome, myEngine);
        }
        else
        {
            HandleReplace(targetTabHome, myEngine);
        }
        
        OnDragEndedInHome?.Invoke(true);

    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }

    public void SetToOriginalParent()
    {
        transform.SetParent(_originalParent);
        transform.SetAsLastSibling();
    }

    #region Swap/Replace
    private bool IsSwap(EngineController myEngine, TabHome targetTabHome)
    {
        // 원래 TabHome에 있고, drop한 곳이 다른 TabHome일 때 swap
        /*return myEngine.IsInHome
               && _prevParent != null
               && targetTabHome.transform != _prevParent;*/

        return false;
    }
    
    private TabHome GetDropTabHome(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (var result in results)
        {
            var tabHome = result.gameObject.GetComponent<TabHome>();
            if (tabHome != null) return tabHome;
        }
        return null;
    }
    
    private void HandleDropToEmptyTabHome(TabHome tabHome, EngineController myEngine)
    {
        transform.SetParent(tabHome.transform, false);
        _rectTransform.anchoredPosition = Vector3.zero;
        // myEngine.IsInHome = true;
    }
    
    private void HandleSwap(TabHome targetTabHome, EngineController myEngine)
    {
        EngineController prevEngine = targetTabHome.transform.GetChild(0).GetComponent<EngineController>();
        prevEngine.transform.SetParent(_prevParent, false);
        prevEngine.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        // prevEngine.IsInHome = true;

        transform.SetParent(targetTabHome.transform, false);
        _rectTransform.anchoredPosition = Vector3.zero;
        // myEngine.IsInHome = true;
    }

    private void HandleReplace(TabHome targetTabHome, EngineController myEngine)
    {
        EngineController prevEngine = targetTabHome.transform.GetChild(0).GetComponent<EngineController>();
        prevEngine.transform.SetParent(_originalParent);
        prevEngine.GetComponent<RectTransform>().anchoredPosition += new Vector2(300, 0);
        // prevEngine.IsInHome = false;

        transform.SetParent(targetTabHome.transform, false);
        _rectTransform.anchoredPosition = Vector3.zero;
        // myEngine.IsInHome = true;

    }
    #endregion
}
