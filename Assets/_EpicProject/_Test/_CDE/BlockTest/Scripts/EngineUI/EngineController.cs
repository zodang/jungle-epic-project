using Define;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EngineController : MonoBehaviour
{
    public bool IsActivate { get; private set; } // 창 활성화 여부 체크
    
    private Clickable _currentTarget;
    private EngineUIController _engineUIController;
    private DraggableUI _draggableUI;
    
    public List<EngineSlot> EngineSlotList = new List<EngineSlot>();
    
    private void Awake()
    {
        _engineUIController = GetComponent<EngineUIController>();
        EngineSlotList = new List<EngineSlot>(GetComponentsInChildren<EngineSlot>());

        for (int i = 0; i < EngineSlotList.Count; i++)
        {
            EngineSlotList[i].Init(i);
        }
    }

    private void Start()
    {
        // Button 기능 연결
        _engineUIController.OnClickCloseBtn += Deactivate;
        _engineUIController.OnResetBtnClicked += ResetFeature;
        _engineUIController.OnClearBtnClicked += ClearBlock;
        
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _engineUIController.OnClickCloseBtn -= Deactivate;
        _engineUIController.OnResetBtnClicked -= ResetFeature;
        _engineUIController.OnClearBtnClicked -= ClearBlock;
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
        
        // Clickable의 기본 블록 세팅
        _currentTarget.InitClickable(this);

        // UI 세팅
        _engineUIController.SetProfile(target.GetProfile());
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
        if (!gameObject.activeSelf) return;
        
        _engineUIController.DeactivateEffect(_currentTarget);
        GameManager.Instance.AudioManager.PlaySfx(SfxType.Close);
        ClearBlock();

    }

    public void DeactivateSilently()
    {
        IsActivate = false;
        if (!gameObject.activeSelf) return;
        
        _engineUIController.DeactivateEffect(_currentTarget);
        ClearBlock();
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
            block.Value.DropToInventorySlot();
        }
    }
}
