using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public SettingUI SettingUI { get; private set; }
    public PopupUI PopupUI { get; private set; }

    private void Awake()
    {
        PopupUI = GetComponentInChildren<PopupUI>(true);
        SettingUI = GetComponentInChildren<SettingUI>();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        if (PopupUI.IsActive())
        {
            PopupUI.HidePopup();
        }
        else
        {
            // Loading 시 ESC 비활성화
            if (GameManager.Instance.FadeManager.IsLoading) return;
            
            GameManager.Instance.SettingManager.ToggleSetting();
        }
    }
}
