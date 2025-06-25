using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public ResolutionSetting ResolutionSetting { get; private set; }
    public LanguageSetting LanguageSetting { get; private set; }
    public AudioSetting AudioSetting { get; private set; }

    private SettingUI _settingUI;

    private void Awake()
    {
        ResolutionSetting = transform.GetComponentInChildren<ResolutionSetting>();
        LanguageSetting = transform.GetComponentInChildren<LanguageSetting>();
        AudioSetting = transform.GetComponentInChildren<AudioSetting>();
        
        _settingUI = FindAnyObjectByType<SettingUI>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            OpenSetting();
        }
    }

    public void OpenSetting()
    {
        _settingUI.OpenSettingUI();
    }
}
