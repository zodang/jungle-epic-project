public abstract class TriggerControlBase<TFeature> : EngineBlock where TFeature : class
{
    private TFeature _feature;

    public override void Activate(object feature)
    {
        _feature = feature as TFeature;
        if (_feature == null) return;
        
        // 기능 활성화
        EnableFeature(_feature);
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
