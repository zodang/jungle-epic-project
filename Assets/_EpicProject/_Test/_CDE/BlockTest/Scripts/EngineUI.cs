using Define;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EngineUI : MonoBehaviour
{
    public Clickable CurrentTarget { get; private set; }

    private BlockFactory _blockFactory;

    private EngineSlotGroup _engineSlotGroup;
    private List<InspectorSlot> _slotList;
    private Transform[] _slotTransforms;
    
    private EngineUICloseBtn _closeBtn;
    private EngineUIOpacitySlider _opacitySlider;
    
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    
    private Vector2 _offset = new Vector2(150, 0);
    private float _minOpacity = 0.4f;

    private void Awake()
    {
        _blockFactory = FindAnyObjectByType<BlockFactory>();
        _engineSlotGroup = FindAnyObjectByType<EngineSlotGroup>();

        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        
        _slotList = new List<InspectorSlot>(_engineSlotGroup.GetComponentsInChildren<InspectorSlot>());
        _slotTransforms = new Transform[_slotList.Count];
        for (int i = 0; i < _slotList.Count; i++)
        {
            _slotTransforms[i] = _slotList[i].transform;
        }

        // close button 기능 연결
        _closeBtn = transform.GetComponentInChildren<EngineUICloseBtn>();
        _closeBtn.GetComponent<Button>().onClick.AddListener(CloseInspector);

        // opacity slider 기능 연결
        _opacitySlider = transform.GetComponentInChildren<EngineUIOpacitySlider>();
        _opacitySlider.GetComponent<Slider>().onValueChanged.AddListener(OnSliderValueChanged);
        _opacitySlider.GetComponent<Slider>().minValue = _minOpacity;
    }

    private void Start()
    {
        gameObject.AddComponent<DraggableUI>();
        gameObject.SetActive(false);
    }

    public void OpenInspector(Clickable target)
    {
        CurrentTarget = target;
        
        // Inspector Slot의 기존 블록 제거
        foreach (var slot in _slotList)
        {
            var existing = slot.GetChildBlock();
            if (existing != null)
            {
                Destroy(existing.gameObject);
                slot.OnBlockRemoved();
            }
        }
        
        // Inspector Slot에 새 Block 추가
        BlockManager.ApplyBlockToTarget(target, _blockFactory, _slotTransforms);
        
        SetUIPosition(target);
        
        AudioManager.instance.playSfx(SfxType.Open);
        gameObject.SetActive(true);
    }

    private void CloseInspector()
    {
        CurrentTarget = null;
        
        AudioManager.instance.playSfx(SfxType.Close);
        gameObject.SetActive(false);
    }

    public void RefreshSlot(Clickable target)
    {
        CurrentTarget = target;
        
        BlockManager.RemoveBlockFromTarget(target, _slotTransforms);
        BlockManager.ApplyBlockToTarget(target, _blockFactory, _slotTransforms);
    }

    private void OnSliderValueChanged(float value)
    {
        _canvasGroup.alpha = value;
    }
    
    private void SetUIPosition(Clickable clickable)
    {
        // 스크린 좌표로 변환
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, clickable.transform.position);

        // 우측에 UI 위치
        Vector2 targetPos = screenPos + _offset;

        // 팝업 UI 크기/캔버스 크기 가져오기
        Vector2 uiSize = _rectTransform.sizeDelta * _canvas.scaleFactor;
        float halfWidth = uiSize.x * 0.5f;
        float halfHeight = uiSize.y * 0.5f;

        // 화면 끝 계산 (스크린 좌표)
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // 짤림 검사
        // (1) 오른쪽 끝 넘침 → 왼쪽에 붙임
        if (targetPos.x + halfWidth > screenWidth)
            targetPos.x = screenPos.x - _offset.x - uiSize.x;

        // (2) 왼쪽 끝 넘침 → 오른쪽에 붙임
        if (targetPos.x - halfWidth < 0)
            targetPos.x = screenPos.x + _offset.x;

        // (3) 위쪽 끝 넘침 → 아래로 내림
        if (targetPos.y + halfHeight > screenHeight)
            targetPos.y = screenHeight - halfHeight - 10;

        // (4) 아래쪽 끝 넘침 → 위로 올림
        if (targetPos.y - halfHeight < 0)
            targetPos.y = halfHeight + 10;

        // UI 위치 변경
        _rectTransform.position = targetPos;
    }
}
