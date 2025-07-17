using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public abstract class StageBaseManager : MonoBehaviour
{
    public static StageBaseManager Instance { get; private set; }
    public EngineManager EngineManager { get; private set; }
    public PlayerManager PlayerManager { get; private set; }
    public DialogueManager DialogueManager { get; private set; }
    public FlagManager FlagManager { get; private set; }

    public List<Clickable> ClickableList { get; private set; }

    protected string stageFilePath;
    private string _profileDataPath;
    protected Dictionary<string, ClickableProfile> _profileDic = new();
    
    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        // 테스트용 코드 추가
        gameObject.AddComponent<BootstrapManager>();
        
        // Manager 클래스 참조
        EngineManager = FindAnyObjectByType<EngineManager>();
        PlayerManager = FindAnyObjectByType<PlayerManager>();
        DialogueManager = FindAnyObjectByType<DialogueManager>();
        FlagManager = FindAnyObjectByType<FlagManager>();
        
        if(FlagManager.IsUnityNull())
        {
            FlagManager = transform.AddComponent<FlagManager>();
        }

        // Clickable의 프로필 데이터 로드
        ClickableList = new List<Clickable>(FindObjectsByType<Clickable>(FindObjectsSortMode.None));
        LoadClickableProfile();
    }

    private void Start()
    {
        // 씬 전환 시 초기화
        StageManager.Instance.InputManager.ActivatePlayerInput(true);
        GameManager.Instance.SettingManager.CloseSetting();

        StageStartTime = Time.realtimeSinceStartup;
    }

    private void LoadClickableProfile()
    {
        // stage json 파일 경로 체크
        if (string.IsNullOrEmpty(stageFilePath))
        {
            Debug.LogWarning("Stage 파일 경로가 지정되지 않았습니다.");
            return;
        }
        
        _profileDataPath = stageFilePath + "/ProfileData";

        TextAsset json = Resources.Load<TextAsset>(_profileDataPath);
        if (json == null)
        {
            Debug.LogError($"Profile 데이터 파일을 찾을 수 없습니다: {_profileDataPath}");
            return;
        }

        // stage json 파싱
        ClickableProfileList wrapper = JsonUtility.FromJson<ClickableProfileList>(json.text);

        foreach (var profile in wrapper.items)
        {
            profile.sprite = Resources.Load<Sprite>(profile.imagePath);
            _profileDic[profile.id] = profile;
        }

        foreach (var clickable in ClickableList)
        {
            if (_profileDic.TryGetValue(clickable.ID, out var profile))
            {
                clickable.InitProfile(profile);
            }
        }
    }
    
    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }


    #region LogSystem
    public string StageId { get; private set; }
    public string SectionId { get; private set; }
    public float StageStartTime { get; private set; }

    protected void ChangeStageId(string stageId)
    {
        StageId = stageId;
    }
    
    public void ChangeStageSection(string sectionIndex)
    {
        // 현재 구간 변경
        SectionId = sectionIndex;
        
        // 로그 시스템
        string stage = StageId;
        string section = SectionId;
        float elapsed = Time.realtimeSinceStartup - StageStartTime;
        GameManager.Instance.LogManager.LogSectionEnter(stage, section, elapsed);
    }
    #endregion
}


[Serializable]
public class ClickableProfileList
{
    public List<ClickableProfile> items;
}

[Serializable]
public class ClickableProfile
{
    public string id;
    public string name;
    public string imagePath;
    public Sprite sprite;
}