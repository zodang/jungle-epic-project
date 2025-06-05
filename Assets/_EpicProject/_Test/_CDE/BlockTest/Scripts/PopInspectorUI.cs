using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopInspectorUI : MonoBehaviour
{
    public Clickable CurrentTarget { get; private set; }

    [SerializeField] private Button closeBtn;

    private BlockFactory _blockFactory;

    private InspectorSlotGroup _inspectorSlotGroup;
    private List<InspectorSlot> _slotList;
    
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private Vector2 _offset = new Vector2(150, 0);

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
        _blockFactory = FindAnyObjectByType<BlockFactory>();
        
        _inspectorSlotGroup = FindAnyObjectByType<InspectorSlotGroup>();
        _slotList = new List<InspectorSlot>(_inspectorSlotGroup.GetComponentsInChildren<InspectorSlot>());


        closeBtn.onClick.AddListener(CloseInspector);
    }

    private void Start()
    {
        CloseInspector();
    }

    public void OpenInspector(Clickable target)
    {
        CurrentTarget = target;
        
        // 슬롯의 기존 블록 제거
        foreach (var slot in _slotList)
        {
            var existing = slot.GetChildBlock();
            if (existing != null)
            {
                Destroy(existing.gameObject);
                slot.OnBlockRemoved();
            }
        }
        
        // 현재 clickable의 block type대로 feature block 생성
        for (int i = 0; i < target.BlockTypeList.Count && i < _slotList.Count; i++)
        {
            var type = target.BlockTypeList[i];
            var slot = _slotList[i];

            var featureBlock = _blockFactory.CreateFeatureBlock(type, slot.transform);
            slot.OnBlockDrop(featureBlock);

            var feature = target.GetComponent(featureBlock.RequiredFeatureType);
            featureBlock.Activate(feature);
        }
        
        SetPosition(target);
        gameObject.SetActive(true);
    }

    public void CloseInspector()
    {
        CurrentTarget = null;
        gameObject.SetActive(false);
    }

    public void RefreshSlot(Clickable target)
    {
        CurrentTarget = target;
        
        foreach (var slot in _slotList)
        {
            var existing = slot.GetChildBlock();
            if (existing is FeatureBlock featureBlock)
            {
                var feature = target.GetComponent(featureBlock.RequiredFeatureType);
                featureBlock.Deactivate(feature);
                Destroy(featureBlock.gameObject);
            }

            slot.OnBlockRemoved();
        }

        for (int i = 0; i < target.BlockTypeList.Count && i < _slotList.Count; i++)
        {
            var blockType = target.BlockTypeList[i];
            var slot = _slotList[i];

            var featureBlock = _blockFactory.CreateFeatureBlock(blockType, slot.transform);
            slot.OnBlockDrop(featureBlock);

            var feature = target.GetComponent(featureBlock.RequiredFeatureType);
            featureBlock.Activate(feature);
        }
    }
    
    private void SetPosition(Clickable clickable)
    {
        // 1. 월드 → 스크린 좌표로 변환
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, clickable.transform.position);

        // 2. 우측에 UI 위치
        Vector2 targetPos = screenPos + _offset;

        // 3. 팝업 UI 크기/캔버스 크기 가져오기
        Vector2 uiSize = _rectTransform.sizeDelta * _canvas.scaleFactor;
        float halfWidth = uiSize.x * 0.5f;
        float halfHeight = uiSize.y * 0.5f;

        // 4. 화면 끝 계산 (스크린 좌표)
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // 5. 짤림 검사
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

        // 6. UI 실제 위치 반영
        _rectTransform.position = targetPos;
    }
}
