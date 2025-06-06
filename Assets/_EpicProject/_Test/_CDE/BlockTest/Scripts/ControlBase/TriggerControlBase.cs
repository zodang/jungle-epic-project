public abstract class TriggerControlBase<TFeature> : FeatureBlock where TFeature : class
{
    private TFeature _feature;

    public override void Activate(object feature)
    {
        _feature = feature as TFeature;
        
        // 기능 활성화
        if (_feature != null) EnableFeature(_feature);
    }

    public override void Deactivate(object feature)
    {
        // 기능 비활성화
        DisableFeature(_feature);
        _feature = null;
    }

    protected abstract void EnableFeature(TFeature feature);
    protected abstract void DisableFeature(TFeature feature);
}
