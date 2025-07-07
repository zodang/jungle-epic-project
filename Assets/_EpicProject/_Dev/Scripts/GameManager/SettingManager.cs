using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance { get; private set; } 
    public ResolutionSetting ResolutionSetting { get; private set; }
    public LanguageSetting LanguageSetting { get; private set; }
    public AudioSetting AudioSetting { get; private set; }

    private SettingUI _settingUI;
    public SettingData CurrentSetting { get; private set; } = new SettingData();

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

    private void Start()
    {
        // LoadSetting();
        // ApplySetting();
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

    private void SaveSetting()
    {
        // 저장된 설정값 변경
        CurrentSetting.LanguageIndex = LanguageSetting.CurrentIndex;
        CurrentSetting.ResolutionIndex = ResolutionSetting.CurrentIndex;
        CurrentSetting.IsFullScreen = ResolutionSetting.IsFullScreen;
        CurrentSetting.BgmVolume = AudioSetting.CurrentBgmVolume;
        CurrentSetting.SfxVolume = AudioSetting.CurrentSfxVolume;

        // 변경된 설정값 저장
        GameSaveData gameSaveData = GameManager.Instance.SaveManager.LoadData();
        gameSaveData.SettingData = CurrentSetting;
        GameManager.Instance.SaveManager.SaveData(gameSaveData);
    }

    private void LoadSetting()
    {
        // 저장된 설정값 불러오기
        GameSaveData gameSaveData = GameManager.Instance.SaveManager.LoadData();
        CurrentSetting = gameSaveData.SettingData;
    }

    private void ApplySetting()
    {
        LanguageSetting.ChangeLanguage(CurrentSetting.LanguageIndex);
        ResolutionSetting.ChangeResolution(CurrentSetting.ResolutionIndex);
        ResolutionSetting.ChangeFullScreen(CurrentSetting.IsFullScreen);
    }
}
