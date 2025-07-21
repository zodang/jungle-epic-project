using System.Collections.Generic;
using Define;
using System;
using UnityEngine;
using Unity.VisualScripting;

public class Clickable : MonoBehaviour, IClickable
{
    public Action OnClickAction;
    
    public string ID;
    private ClickableProfile _profile;
    [SerializeField] private List<BlockType> DefaultBlockList = new();

    public BlockContainerBase BlockContainerBase { get; private set; }
    public EngineController EngineController { get; private set; }

    public void OnClicked()
    {
        OnClickAction?.Invoke();
        
        // 클릭 시 Engine UI 활성화
        if (GetComponent<PlayerManager>() != null) return;
        
        StageBaseManager.Instance.EngineManager.ActivateEngineUI(this);
    }
    public void InitProfile(ClickableProfile profile)
    {
        _profile = profile;
    }
    
    public ClickableProfile GetProfile()
    {
        return _profile;
    }
    
    public void InitBlockContainerBase(BlockContainerBase engineController)
    {
        BlockContainerBase = engineController;
    }

    public void InitEngineController(EngineController engineController)
    {
        EngineController = engineController;
        EngineController.InitEngineController(this, DefaultBlockList);
    }

    private void OnDisable()
    {
        if(!EngineController.IsUnityNull() && EngineController.IsActivate)
        {
            EngineController.Deactivate();
        }
    }
}
