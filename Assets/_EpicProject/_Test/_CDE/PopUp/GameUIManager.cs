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
            GameManager.Instance.SettingManager.ToggleSetting();
        }
    }
}
