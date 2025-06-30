using Define;
using System;

public class GraphicBlock : ButtonControlBase<IGraphicChangeable>
{
    public override BlockType Type => BlockType.Graphic;
    public override Type RequiredFeatureType => typeof(IGraphicChangeable);

    protected override void OnButtonClicked(int buttonIndex)
    {
        Feature.SetGraphic(buttonIndex);
    }
}
