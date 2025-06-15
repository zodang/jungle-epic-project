using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class StageBaseManager : MonoBehaviour
{
    public static StageBaseManager Instance { get; private set; }
    public EngineManager EngineManager { get; private set; }
    public PlayerManager PlayerManager { get; private set; }
    public DialogueManager DialogueManager { get; private set; }

    [SerializeField] protected string stageFilePath = "Stages/0_Stage/0_StageData";
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
        DialogueManager = GetComponentInChildren<DialogueManager>();
        
        // Clickable의 프로필 데이터 로드
        LoadClickableProfile();
    }
    
    private void LoadClickableProfile()
    {
        // stage json 파일 경로 체크
        if (string.IsNullOrEmpty(stageFilePath))
        {
            Debug.LogWarning("Stage 파일 경로가 지정되지 않았습니다.");
            return;
        }

        TextAsset json = Resources.Load<TextAsset>(stageFilePath);
        if (json == null)
        {
            Debug.LogError($"Stage JSON 파일을 찾을 수 없습니다: {stageFilePath}");
            return;
        }

        // stage json 파싱
        ClickableProfileList wrapper = JsonUtility.FromJson<ClickableProfileList>(json.text);

        foreach (var profile in wrapper.items)
        {
            profile.sprite = Resources.Load<Sprite>(profile.imagePath);
            _profileDic[profile.id] = profile;
        }

        foreach (var clickable in FindObjectsByType<Clickable>(FindObjectsSortMode.None))
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
    public string serialNumber;
    public string status;
    public string imagePath;

    public Sprite sprite;
}