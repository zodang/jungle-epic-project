using Define;
using System;

public class EmotionBlock : TriggerControlBase<IEmotionAvailable>
{
    public override BlockType Type => BlockType.Emotion;
    public override Type RequiredFeatureType => typeof(IEmotionAvailable);

    protected override void EnableFeature(IEmotionAvailable feature)
    {
        feature.EnableEmotion();
    }

    protected override void DisableFeature(IEmotionAvailable feature)
    {
        feature.DisableEmotion();
    }
}