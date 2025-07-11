using System;
using UnityEngine;
using UnityEngine.UI;

public class EngineSlot : MonoBehaviour, ISlot
{
    #region ISlotType

    private Clickable _targetClickable;
    
    public Define.SlotType GetSlotType()
    {
        return Define.SlotType.EngineSlot;
    }

    public Clickable GetTargetClickable()
    {
        return _targetClickable;
    }
    
    public Transform GetTransform()
    {
        return transform;
    }

    public void SetTargetClickable(Clickable clickable)
    {
        _targetClickable = clickable;
    }
    #endregion
    
    private BlockContainer _blockContainer;
    private EngineSlotIcon _slotIcon;
    private Image _slotIconImg;
    private Sprite _defaultSprite;

    public int Index { get; private set; } = -1;
    private EngineBlock _currentBlock;

    // KMS 0708 / Todo : 블록 놔뒀을때 애니메이션 실행 구간
    public static event Action OnBlockPlaced;

    private void Awake()
    {
        _blockContainer = GetComponentInChildren<BlockContainer>();
    }

    public void Init(int index)
    {
        Index = index;
    }

    public void SetBlockPosition(EngineBlock block)
    {
        _currentBlock = block;
        SetBlockParent(_currentBlock);
    }

    private void SetBlockParent(EngineBlock block)
    {
        if (block == null) return;
        // 해당 block의 부모를 해당 Slot으로 변경
        block.transform.SetParent(_blockContainer.transform, false);

        RectTransform rectTransform = block.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.one * 0.5f;
        rectTransform.anchorMax = Vector2.one * 0.5f;
        rectTransform.anchoredPosition = Vector2.zero;
        // KMS 0708 / Todo : 블록 놔뒀을때 애니메이션 실행 구간 
        OnBlockPlaced?.Invoke();
        print("애니메이션 실행"); // 디버그용
    }

    private void SetIcon(EngineBlock block)
    {
        if (block == null)
        {
            _slotIconImg.sprite = _defaultSprite;
            return;
        }
        
        Sprite iconSprite = StageManager.Instance.BlockFactory.GetIcon(block.Type);
        _slotIconImg.sprite = iconSprite;
    }
}
