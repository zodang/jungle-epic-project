using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ButtonControlBase<TFeature> : EngineBlock where TFeature : class
{
    protected TFeature Feature;
    
    [SerializeField] private List<Button> _btnList;
    
    public override void Activate(object feature)
    {
        Feature = feature as TFeature;
        if (Feature == null) return;

        // 버튼 리스너 연결
        for (int i = 0; i < _btnList.Count; i++)
        {
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

    protected abstract void OnButtonClicked(int buttonIndex);
    protected abstract int GetCurrentValue();
}
