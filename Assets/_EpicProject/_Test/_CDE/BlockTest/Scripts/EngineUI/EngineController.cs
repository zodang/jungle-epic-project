using Define;
using System.Collections.Generic;
using UnityEngine;

public class EngineController : MonoBehaviour
{
    public Clickable CurrentTarget { get; private set; }
    public bool IsActivate { get; private set; } // 창 활성화 여부 체크
    public bool IsInHome { get; private set; } // Home 정렬 여부 체크

    public List<Numpad> NumpadList { get; private set; }
    public int SelectedIndex { get; private set; }

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
        CurrentTarget.OnBlockChanged += ChangeAllNumpadVisual;
        _draggableUI.OnParentChangedToHome += ChangeIsHome;
        
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _engineUIController.OnClickCloseBtn -= Deactivate;
        _engineUIController.OnResetBtnClicked -= ResetFeature;
        CurrentTarget.OnBlockChanged -= ChangeAllNumpadVisual;
        _draggableUI.OnParentChangedToHome -= ChangeIsHome;
    }

    public void InitEngineController(Clickable target)
    {
        CurrentTarget = target;
        
        // Slot의 Target Clickable 설정
        List<ISlotType> slots = new List<ISlotType>(transform.GetComponentsInChildren<ISlotType>());
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].SetTargetClickable(CurrentTarget);
        }
        
        // Numpad 기능 세팅
        for (int i = 0; i < NumpadList.Count; i++)
        {
            NumpadList[i].Init(i);
            NumpadList[i].OnClickNumpad += ShowBlock;
        }

        // UI 세팅
        _engineUIController.SetProfile(target.GetProfile());
        _engineUIController.SetPosition(CurrentTarget);
    }
    
    public void ShowBlock(int index)
    {
        SelectedIndex = index;
        _engineUIController.ChangeBlockContainer(index);
        
        foreach (var blockPair in CurrentTarget.BlockDictionary)
        {
            int slotIndex = blockPair.Key;
            EngineBlock block = blockPair.Value;

            if (block == null) continue;

            bool isActive = (slotIndex == index);

            block.ShowBlockVisual(isActive);
            _engineUIController.SetBlockPositionToEngine(block);
        }
    }
    
    public void Activate()
    {
        if (IsActivate) return;
        
        IsActivate = true;

        _engineUIController.ActivateEffect();
        GameManager.Instance.AudioManager.PlaySfx(SfxType.Open);
    }
    
    private void Deactivate()
    {
        IsActivate = false;
        IsInHome = false;
        
        if (!gameObject.activeSelf) return;
        _draggableUI.SetParentToOriginal(false);
        _engineUIController.DeactivateEffect(CurrentTarget);
        
        GameManager.Instance.AudioManager.PlaySfx(SfxType.Close);
    }

    public void DeactivateSilently()
    {
        IsActivate = false;
        IsInHome = false;
        
        if (!gameObject.activeSelf) return;
        _draggableUI.SetParentToOriginal(false);
        _engineUIController.DeactivateEffect(CurrentTarget);
    }
    
    private void ResetFeature()
    {
        if (CurrentTarget == null) return;

        // Clickable의 기능 초기화
        IFeatureResetable resettable = CurrentTarget.GetComponent<IFeatureResetable>();
        resettable?.ResetFeature();
        
        // Block의 UI 초기화
        EngineBlock[] blocks = GetComponentsInChildren<EngineBlock>(includeInactive: true);
        foreach (EngineBlock block in blocks)
        {
            block.ResetUI();
        }
    }

    public void ChangeAllNumpadVisual()
    {
        foreach (var numpad in NumpadList)
        {
            numpad.ChangeVisual();
        }
    }
    
    private void ChangeIsHome(bool isInHome)
    {
        IsInHome = isInHome;
    }
}
