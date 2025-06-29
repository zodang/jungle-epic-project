using Define;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EngineController : MonoBehaviour
{
    public bool IsActivate { get; private set; } // 창 활성화 여부 체크
    public bool IsInHome; // 정렬 여부 체크
    public List<Numpad> NumpadList { get; private set; }
    public int SelectedIndex { get; private set; }

    private Clickable _currentTarget;
    private EngineUIController _engineUIController;
    private DraggableUI _draggableUI;

    private void Awake()
    {
        _engineUIController = GetComponent<EngineUIController>();
        _draggableUI = GetComponent<DraggableUI>();
        
        NumpadList = new List<Numpad>(transform.GetComponentsInChildren<Numpad>());
    }

    private void Start()
    {
        // Button 기능 연결
        _engineUIController.OnClickCloseBtn += Deactivate;
        _engineUIController.OnResetBtnClicked += ResetFeature;
        _engineUIController.OnClearBtnClicked += ClearBlock;
        _currentTarget.OnBlockChanged += ChangeAllNumpadVisual;
        _draggableUI.OnDragEndedInHome += HandleDragEndedInHome;
        
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _engineUIController.OnClickCloseBtn -= Deactivate;
        _engineUIController.OnResetBtnClicked -= ResetFeature;
        _engineUIController.OnClearBtnClicked -= ClearBlock;
        _currentTarget.OnBlockChanged -= ChangeAllNumpadVisual;
        _draggableUI.OnDragEndedInHome -= HandleDragEndedInHome;
    }

    public void InitEngineController(Clickable target)
    {
        _currentTarget = target;
        
        // Slot의 Target Clickable 설정
        List<ISlotType> slots = new(transform.GetComponentsInChildren<ISlotType>());
        foreach (var slot in slots)
        {
            slot.SetTargetClickable(_currentTarget);
        }
        
        // Numpad 기능 세팅
        for (int i = 0; i < NumpadList.Count; i++)
        {
            NumpadList[i].Init(i);
            NumpadList[i].OnClickNumpad += ShowBlock;
        }

        // UI 세팅
        _engineUIController.SetProfile(target.GetProfile());
        _engineUIController.SetPosition(_currentTarget);
    }
    
    public void ShowBlock(int index)
    {
        SelectedIndex = index;
        
        List<(int, EngineBlock, bool)> blocksToShow = new List<(int slotIndex, EngineBlock block, bool isActive)>();
        foreach (var engineBlock in _currentTarget.BlockDictionary)
        {
            blocksToShow.Add((engineBlock.Key,engineBlock.Value, engineBlock.Key == index));
        }
        
        _engineUIController.ChangeBlockContainer(index);
        foreach (var blockData in blocksToShow)
        {
            if (blockData.Item2 != null)
            {
                // Block의 Visual 변경
                blockData.Item2.ShowBlockVisual(blockData.Item3);
                _engineUIController.SetBlockPositionToEngine(blockData.Item2);
            }
        }
    }
    
    public void Activate()
    {
        if (IsActivate) return;
        IsActivate = true;

        _engineUIController.SetPosition(_currentTarget);
        _engineUIController.ActivateEffect();
        GameManager.Instance.AudioManager.PlaySfx(SfxType.Open);
    }
    
    private void Deactivate()
    {
        IsActivate = false;
        IsInHome = false;
        if (!gameObject.activeSelf) return;
        
        _draggableUI.SetToOriginalParent();
        _engineUIController.DeactivateEffect(_currentTarget);
        GameManager.Instance.AudioManager.PlaySfx(SfxType.Close);
    }

    public void DeactivateSilently()
    {
        IsActivate = false;
        IsInHome = false;
        if (!gameObject.activeSelf) return;
        
        _draggableUI.SetToOriginalParent();
        _engineUIController.DeactivateEffect(_currentTarget);
    }
    
    private void ResetFeature()
    {
        if (_currentTarget == null) return;

        // Clickable의 기능 초기화
        IFeatureResetable resettable = _currentTarget.GetComponent<IFeatureResetable>();
        resettable?.ResetFeature();
        
        // Block의 UI 초기화
        EngineBlock[] blocks = GetComponentsInChildren<EngineBlock>(includeInactive: true);
        foreach (EngineBlock block in blocks)
        {
            block.ResetUI();
        }
    }

    private void ClearBlock()
    {
        foreach (var block in _currentTarget.BlockDictionary.ToList())
        {
            block.Value.DropToInventorySlot(block.Key);
        }
        
        ShowBlock(0);
    }

    public void ChangeAllNumpadVisual()
    {
        foreach (var numpad in NumpadList)
        {
            numpad.ChangeVisual();
        }
    }
    
    private void HandleDragEndedInHome(bool isInHome)
    {
        IsInHome = isInHome;
        if (!isInHome) _draggableUI.SetToOriginalParent();
    }
}
