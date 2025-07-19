using Define;
using System;
using TMPro;
using UnityEngine;


public class GraphicBlock : ButtonControlBase<IGraphicChangeable>
{
    public override BlockType Type => BlockType.Graphic;
    public override Type RequiredFeatureType => typeof(IGraphicChangeable);
    [SerializeField] public TMP_Text optionText;

    public override void Activate(object feature)
    {
        base.Activate(feature);
        ChangeOptionText(GetCurrentValue());
    }
    protected override void OnButtonClicked(int buttonIndex)
    {
        Feature.SetValue((GraphicType)buttonIndex);
        ChangeOptionText(buttonIndex);
    }

    protected override int GetCurrentValue()
    {
        return (int)Feature.GetCurrentValue();
    }
    
    public override void ResetUI()
    {
        if (optionText == null) return;
        ChangeOptionText(GetCurrentValue());
    }
    
    private void ChangeOptionText(int buttonIndex)
    {
        if (buttonIndex == 0)
        {
            optionText.text = "Low";
        }
        else if (buttonIndex == 1)
        {
            optionText.text = "Mid";
        }
        else if (buttonIndex == 2)
        {
            optionText.text = "High";
        }
    }
}
