using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupUI : MonoBehaviour
{
    public bool IsActive() => gameObject.activeSelf;
    
    private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button cancelBtn;
    [SerializeField] private Button confirmBtn;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        HidePopup();
    }

    public void ShowPopup(string title, string message, Action onOk = null, Action onCancel = null, string okLabel = "확인", string cancelLabel = "취소")
    {
        titleText.text = title;
        messageText.text = message;

        confirmBtn.gameObject.SetActive(onOk != null);
        cancelBtn.gameObject.SetActive(onCancel != null);

        confirmBtn.GetComponentInChildren<TMP_Text>().text = okLabel;
        cancelBtn.GetComponentInChildren<TMP_Text>().text = cancelLabel;

        confirmBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.RemoveAllListeners();

        if (onOk != null)
            confirmBtn.onClick.AddListener(() => { onOk(); HidePopup(); });
        if (onCancel != null)
            cancelBtn.onClick.AddListener(() => { onCancel(); HidePopup(); });

        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
        gameObject.SetActive(true);
    }

    public void HidePopup()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
        gameObject.SetActive(false);
    }
}
