using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance { get; private set; } 
    public ResolutionSetting ResolutionSetting { get; private set; }
    public LanguageSetting LanguageSetting { get; private set; }
    public AudioSetting AudioSetting { get; private set; }
    public SettingData CurrentSetting { get; private set; } = new SettingData();
    
    private SettingUI _settingUI;
    private bool _isSettingUIOpen = false;
    
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

        ResolutionSetting = FindAnyObjectByType<ResolutionSetting>();
        LanguageSetting = FindAnyObjectByType<LanguageSetting>();
        AudioSetting = FindAnyObjectByType<AudioSetting>();
        
        _settingUI = FindAnyObjectByType<SettingUI>();
    }

    private void Start()
    {
        _settingUI.OnCloseBtnClicked += OpenSetting;
        
        // 저장된 설정값 불러오기
        CurrentSetting  = GameManager.Instance.SaveManager.LoadSettingData();

        StartCoroutine(ApplyLocalization());
        ApplyResolutionSetting();
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
        _isSettingUIOpen = !_isSettingUIOpen;
        _settingUI.OpenSettingUI(_isSettingUIOpen);
        
        if (!_isSettingUIOpen)
        {
            // Setting UI 닫을 때 설정 데이터 저장
            SaveSetting();
        }
    }

    private void SaveSetting()
    {
        // 저장된 설정값 변경
        CurrentSetting.LanguageIndex = LanguageSetting.CurrentIndex;
        CurrentSetting.ResolutionIndex = ResolutionSetting.CurrentIndex;
        CurrentSetting.IsFullScreen = ResolutionSetting.IsFullScreen;
        CurrentSetting.BgmVolume = AudioSetting.CurrentBgmVolume;
        CurrentSetting.SfxVolume = AudioSetting.CurrentSfxVolume;

        // 변경된 설정값 저장
        SettingData settingData = GameManager.Instance.SaveManager.LoadSettingData();
        settingData = CurrentSetting;
        GameManager.Instance.SaveManager.SaveSettingData(settingData);
    }
    
    private IEnumerator ApplyLocalization()
    {
        yield return LocalizationSettings.InitializationOperation;
        LanguageSetting .ChangeLanguage(CurrentSetting.LanguageIndex);
    }

    private void ApplyResolutionSetting()
    {
        ResolutionSetting.ChangeResolution(CurrentSetting.ResolutionIndex);
        ResolutionSetting.ChangeFullScreen(CurrentSetting.IsFullScreen);
    }
}
