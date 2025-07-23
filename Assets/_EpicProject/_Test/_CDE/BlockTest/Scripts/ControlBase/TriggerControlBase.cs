using UnityEngine;

public abstract class TriggerControlBase<TFeature> : EngineBlock where TFeature : class
{
    private TFeature _feature;

    public override void Activate(object feature)
    {
        _feature = feature as TFeature;
        if (_feature == null) return;
        
        // 기능 활성화
        EnableFeature(_feature);
        
        // 로그시스템
        string stageId = StageBaseManager.Instance.StageId;
        string sectionId = StageBaseManager.Instance.SectionId;
        string blockType = Type.ToString();
        string blockValue = "null";
        string targetObj = CurrentSlot.GetTargetClickable().name;
        GameManager.Instance.LogManager.LogBlockControl(stageId, sectionId, blockType, blockValue, targetObj);
        // Debug.Log($"@@DE ---> {stageId} / {sectionId} / {blockType} / {blockValue} / {targetObj}");
    }

    public override void Deactivate(object feature)
    {
        if (_feature == null) return;
        // 기능 비활성화
        DisableFeature(_feature);
        _feature = null;
    }

    protected abstract void EnableFeature(TFeature feature);
    protected abstract void DisableFeature(TFeature feature);
}
