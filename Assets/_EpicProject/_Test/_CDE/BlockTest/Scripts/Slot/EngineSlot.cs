using Define;
using UnityEngine;
using UnityEngine.UI;

public class EngineSlot : MonoBehaviour, ISlotType
{
    #region ISlotType

    private Clickable _targetClickable;
    
    public SlotType GetSlotType()
    {
        return SlotType.EngineSlot;
    }

    public Clickable GetTargetClickable()
    {
        return _targetClickable;
    }

    public void SetTargetClickable(Clickable clickable)
    {
        _targetClickable = clickable;
    }
    #endregion
    
    private BlockContainer _blockContainer;
    private EngineSlotIcon _slotIcon;
    private Image _slotIconImg;

    public int Index { get; private set; } = -1;
    private EngineBlock _currentBlock;

    private void Awake()
    {
        _blockContainer = GetComponentInChildren<BlockContainer>();
        _slotIcon = GetComponentInChildren<EngineSlotIcon>();
        _slotIconImg = _slotIcon?.GetComponent<Image>();
    }

    public void Init(int index)
    {
        Index = index;
    }

    public void SetBlock(EngineBlock block)
    {
        _currentBlock = block;
        SetBlockParent(block);
        SetIcon(block);
    }

    private void SetBlockParent(EngineBlock block)
    {
        // 해당 block의 부모를 해당 Slot으로 변경
        block.transform.SetParent(_blockContainer.transform, false);

        RectTransform rectTransform = block.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.one * 0.5f;
        rectTransform.anchorMax = Vector2.one * 0.5f;
        rectTransform.anchoredPosition = Vector2.zero;
    }

    private void SetIcon(EngineBlock block)
    {
        Sprite iconSprite = StageManager.Instance.BlockFactory.GetIcon(block.Type);
        _slotIconImg.sprite = iconSprite;
    }
}
