using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class SettingManager : MonoBehaviour
{
    public Action OnSettingOpen;
    public Action OnSettingClose;
    
    public ResolutionSetting ResolutionSetting { get; private set; }
    public LanguageSetting LanguageSetting { get; private set; }
    public AudioSetting AudioSetting { get; private set; }
    
    private SettingUI _settingUI;
    private bool _isSettingUIOpen = true;
    
    private SettingData _currentSetting = new SettingData();
    
    private void Awake()
    {
        ResolutionSetting = FindAnyObjectByType<ResolutionSetting>();
        LanguageSetting = FindAnyObjectByType<LanguageSetting>();
        AudioSetting = FindAnyObjectByType<AudioSetting>();
        
        _settingUI = FindAnyObjectByType<SettingUI>();
    }

    private void Start()
    {
        _settingUI.OnCloseBtnClicked += ToggleSetting;
        
        // 저장된 설정값 불러오기
        _currentSetting = GameManager.Instance.SaveManager.LoadSettingData();

        StartCoroutine(ApplyLocalization());
        ApplyResolutionSetting();
        ApplyAudioSetting();
        
        _isSettingUIOpen = false;
        _settingUI.OpenSettingUI(_isSettingUIOpen);
    }
    
    public void ToggleSetting()
    {
        _isSettingUIOpen = !_isSettingUIOpen;
        
        if (_isSettingUIOpen)
        {
            if (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1) return;
            OpenSetting();
        }
        else if (!_isSettingUIOpen)
        {
            CloseSetting();
        }
    }

    public void OpenSetting()
    {
        _isSettingUIOpen = true;
        _settingUI.OpenSettingUI(_isSettingUIOpen);
        
        OnSettingOpen?.Invoke();
        
        GameManager.Instance.TimeScaleManager.RequestPauseGame();
    }

    public void CloseSetting()
    {
        _isSettingUIOpen = false;
        _settingUI.OpenSettingUI(_isSettingUIOpen);
        
        SaveSetting();
        
        OnSettingClose?.Invoke();
        GameManager.Instance.TimeScaleManager.RequestResumeGame();
    }

    private void SaveSetting()
    {
        // 저장된 설정값 변경
        _currentSetting.LanguageIndex = LanguageSetting.CurrentIndex;
        _currentSetting.ResolutionIndex = ResolutionSetting.CurrentIndex;
        _currentSetting.IsFullScreen = ResolutionSetting.IsFullScreen;
        _currentSetting.BgmVolume = AudioSetting.CurrentBgmVolume;
        _currentSetting.SfxVolume = AudioSetting.CurrentSfxVolume;

        // 변경된 설정값 저장
        SettingData settingData = GameManager.Instance.SaveManager.LoadSettingData();
        settingData = _currentSetting;
        GameManager.Instance.SaveManager.SaveSettingData(settingData);
    }
    
    private IEnumerator ApplyLocalization()
    {
        yield return LocalizationSettings.InitializationOperation;
        LanguageSetting .ChangeLanguage(_currentSetting.LanguageIndex);
    }

    private void ApplyResolutionSetting()
    {
        ResolutionSetting.ChangeResolution(_currentSetting.ResolutionIndex);
        ResolutionSetting.ChangeFullScreen(_currentSetting.IsFullScreen);
    }

    private void ApplyAudioSetting()
    {
        AudioSetting.ChangeBgmVolume(_currentSetting.BgmVolume);
        AudioSetting.ChangeSfxVolume(_currentSetting.SfxVolume);
    }
}
