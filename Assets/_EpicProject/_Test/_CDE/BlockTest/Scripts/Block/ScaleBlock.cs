using UnityEngine;
using UnityEngine.UI;

public class ScaleBlock : Block
{
    public float MaxScale = 2f;
    public float MinScale = 0.5f;
    
    private Slider _slider;
    private ClickableController _target;

    private void Awake()
    {
        _slider = GetComponentInChildren<Slider>();
        _slider.gameObject.SetActive(false);
    }
    
    public override void Activate(ClickableController target)
    {
        _target = target;
        
        _slider.maxValue = MaxScale;
        _slider.minValue = MinScale;
        _slider.value = target.transform.localScale.x;
        
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
        _slider.gameObject.SetActive(true);
    }

    public override void Deactivate(ClickableController target)
    {
        _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        _slider.gameObject.SetActive(false);
        _target = null;
    }

    private void OnSliderValueChanged(float value)
    {
        Vector3 newScale = new Vector3(value, value, 1f);
        _target.transform.localScale = newScale;
    }
}
