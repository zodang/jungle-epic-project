using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class GameBtnGroup : MonoBehaviour
{
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button menuBtn;
    [SerializeField] private Button quitBtn;
    
    private void Awake()
    {
        restartBtn.onClick.AddListener(OnClickRestartBtn);
        menuBtn.onClick.AddListener(OnClickMenuBtn);
        quitBtn.onClick.AddListener(OnClickQuitBtn);
    }

    private void OnClickRestartBtn()
    {
        GameManager.Instance.SettingManager.CloseSetting();
        GameManager.Instance.FadeManager.LoadCurrentScene();
    }

    private void OnClickMenuBtn()
    {
        string title = LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "Menu_Title");
        string message =  LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "Menu_Message");
        string okLabel =  LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "Menu_Confirm");
        string cancelLabel =  LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "Menu_Cancel");
        
        GameManager.Instance.UIManager.PopupUI.ShowPopup
        (
            title,
            message,
            onOk: () =>
            {
                GameManager.Instance.SettingManager.CloseSetting();
                GameManager.Instance.FadeManager.LoadScene(1);
            },
            onCancel: GameManager.Instance.UIManager.PopupUI.HidePopup,
            okLabel,
            cancelLabel
        );
    }

    private void OnClickQuitBtn()
    {
        string title = LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "Quit_Title");
        string message =  LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "Quit_Message");
        string okLabel =  LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "Quit_Confirm");
        string cancelLabel =  LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "Quit_Cancel");

        GameManager.Instance.UIManager.PopupUI.ShowPopup
        (
            title,
            message,
            onOk: () =>
            {
                GameManager.Instance.SettingManager.CloseSetting();
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
            },
            onCancel: GameManager.Instance.UIManager.PopupUI.HidePopup,
            okLabel,
            cancelLabel
        );
    }
}
