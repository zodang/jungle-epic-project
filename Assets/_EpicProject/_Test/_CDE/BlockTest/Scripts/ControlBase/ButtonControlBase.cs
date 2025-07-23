using Define;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ButtonControlBase<TFeature> : EngineBlock where TFeature : class
{
    protected TFeature Feature;
    
    [SerializeField] private List<Button> _btnList;
    
    public event Action OnControlStarted;
    
    public override void Activate(object feature)
    {
        Feature = feature as TFeature;
        if (Feature == null) return;

        // 버튼 리스너 연결
        for (int i = 0; i < _btnList.Count; i++)
        {
            // 중복 연결 방지
            _btnList[i].onClick.RemoveAllListeners();
            
            int index = i;
            _btnList[index].onClick.AddListener(()=> OnButtonClicked(index));
        }
    }

    public override void Deactivate(object feature)
    {
        if (Feature == null) return;
        
        // 버튼 리스너 해제
        for (int i = 0; i < _btnList.Count; i++)
        {
            _btnList[i].onClick.RemoveAllListeners();
        }
        
        Feature = null;
    }

    protected virtual void OnButtonClicked(int buttonIndex)
    {
        OnControlStarted?.Invoke();
        
        // 로그시스템
        string stageId = StageBaseManager.Instance.StageId;
        string sectionId = StageBaseManager.Instance.SectionId;
        string blockType = Type.ToString();
        string blockValue = ((GraphicType)buttonIndex).ToString();
        string targetObj = CurrentSlot.GetTargetClickable().name;
        GameManager.Instance.LogManager.LogBlockControl(stageId, sectionId, blockType, blockValue, targetObj);
        Debug.Log($"@@DE ---> {stageId} / {sectionId} / {blockType} / {blockValue} / {targetObj}");
    }
    
    protected abstract int GetCurrentValue();
}
