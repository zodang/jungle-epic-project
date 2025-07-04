// ESCManager.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ESCManager : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        // SettingManager.Instance가 null이 아닐 때만 리스너 등록
        _button.onClick.AddListener(() =>
        {
            if (SettingManager.Instance != null)
                SettingManager.Instance.OpenSetting();
            else
                Debug.LogWarning("SettingManager.Instance is null!");
        });
    }
}
