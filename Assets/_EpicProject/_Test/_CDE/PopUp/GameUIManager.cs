using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public SettingUI SettingUI { get; private set; }
    public PopupUI PopupUI { get; private set; }

    private void Awake()
    {
        PopupUI = GetComponentInChildren<PopupUI>();
    }
}
