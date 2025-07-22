using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public SettingUI SettingUI { get; private set; }
    public PopupUI PopupUI { get; private set; }

    private bool _isActive;

    private void Awake()
    {
        PopupUI = GetComponentInChildren<PopupUI>(true);
        SettingUI = GetComponentInChildren<SettingUI>();

#if UNITY_EDITOR
        if (SceneManager.GetActiveScene().buildIndex != 0) _isActive = true;
#endif
    }

    public void ActivateGameUIManager(bool isActive)
    {
        // ESC 기능 활성화
        _isActive = isActive;
    }

    private void Update()
    {
        if (!_isActive) return;

        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (PopupUI.IsActive())
        {
            PopupUI.HidePopup();
        }
        else if (SettingUI.IsActive())
        {
            GameManager.Instance.SettingManager.CloseSetting(true);
        }
        else if (!SettingUI.IsActive())
        {
            if (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1)
            {
                string title = LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "Quit_Title");
                string message = LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "Quit_Message");
                string okLabel = LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "Quit_Confirm");
                string cancelLabel = LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "Quit_Cancel");

                GameManager.Instance.UIManager.PopupUI.ShowPopup
                (
                    title,
                    message,
                    onOk: () =>
                    {
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
                return;
            }
            
            // Loading 시 ESC 비활성화
            if (GameManager.Instance.FadeManager.IsLoading) return;
            GameManager.Instance.SettingManager.OpenSetting();
        }
    }
}
