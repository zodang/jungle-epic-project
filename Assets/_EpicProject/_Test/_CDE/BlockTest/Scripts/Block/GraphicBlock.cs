using Define;
using System;
using TMPro;

public class GraphicBlock : ButtonControlBase<IGraphicChangeable>
{
    public TMP_Text GraphicText;

    public override BlockType Type => BlockType.Graphic;
    public override Type RequiredFeatureType => typeof(IGraphicChangeable);

    protected override void OnButtonClicked(int buttonIndex)
    {
        Feature.SetValue((GraphicType)buttonIndex);

        if(buttonIndex == 0)
        {
            GraphicText.text = "Low";
        }
        else if (buttonIndex == 1)
        {
            GraphicText.text = "Mid";
        }
        else if (buttonIndex == 2)
        {
            GraphicText.text = "High";
        }
    }




}
