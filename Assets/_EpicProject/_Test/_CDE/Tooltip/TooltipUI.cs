using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance { get; private set; }
    private CanvasGroup _canvasGroup;

    [SerializeField] private TMP_Text label;
    [SerializeField] private TMP_Text description;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        _canvasGroup.alpha = 0;
    }

    public void ShowTooltip(TooltipData data, Vector2 pos)
    {
        label.text = LocalizationSettings.StringDatabase.GetLocalizedString("ToolTip Table", data.LabelKey);
        description.text = LocalizationSettings.StringDatabase.GetLocalizedString("ToolTip Table", data.DescriptionKey);
        
        _canvasGroup.alpha = 1;
        transform.position = pos;
    }

    public void SetTooltipPosition()
    {
        transform.position = Input.mousePosition;
    }

    public void HideTooltip()
    {
        _canvasGroup.alpha = 0;
    }
}
