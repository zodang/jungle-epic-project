using System;
using System.Collections.Generic;
using UnityEngine;

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
    public string note;
    public string imagePath;

    public Sprite sprite;
}

public class TestStageManager : MonoBehaviour
{
    
    public string StageFileName = "Stages/0_Stage/0_StageData";

    private Dictionary<string, ClickableProfile> _profileDic = new();

    private void Awake()
    {
        LoadStage();
    }

    private void Start()
    {
        AudioManager.instance.PlayBgm(true);
    }

    public void LoadStage()
    {
        TextAsset json = Resources.Load<TextAsset>(StageFileName);
        if (json == null)
        {
            Debug.LogError($"Stage JSON 파일을 찾을 수 없습니다: {StageFileName}");
            return;
        }
        
        // Json 파싱
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
}
