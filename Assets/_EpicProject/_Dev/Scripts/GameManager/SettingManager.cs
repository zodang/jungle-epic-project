using System;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance { get; private set; } 
    public ResolutionSetting ResolutionSetting { get; private set; }
    public LanguageSetting LanguageSetting { get; private set; }
    public AudioSetting AudioSetting { get; private set; }

    private SettingUI _settingUI;

    private void Awake()
    {
        // 싱글턴 초기화 KMS
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        ResolutionSetting = transform.GetComponentInChildren<ResolutionSetting>();
        LanguageSetting = transform.GetComponentInChildren<LanguageSetting>();
        AudioSetting = transform.GetComponentInChildren<AudioSetting>();
        
        _settingUI = FindAnyObjectByType<SettingUI>();
    }

    public void Update()
    {
        // ESC 키를 눌렀을 때 설정 UI 열기
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenSetting();
        }
    }

    public void OpenSetting()
    {
        _settingUI.OpenSettingUI();
    }
}
