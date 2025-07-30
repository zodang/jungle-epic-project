using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private readonly string _saveFileName = "SaveFile.json";
    private string SavePath => Path.Combine(Application.persistentDataPath, _saveFileName);

    public void SaveSettingData(SettingData setting)
    {
        GameSaveData data = LoadData();
        data.SettingData = setting;
        SaveAllData(data);
    }
    
    public void SaveStageData(StageData stage)
    {
        GameSaveData data = LoadData();
        data.StageData = stage;
        SaveAllData(data);
    }

    public void SavePrivacyData(bool isConfirm)
    {
        GameSaveData data = LoadData();
        data.IsPrivacyConfirmation = isConfirm;
        SaveAllData(data);
    }

    public void SaveTalkedNpcData(string id)
    {
        GameSaveData data = LoadData();
        
        if (data.TalkedNpcHashSet.Contains(id)) return;

        data.TalkedNpcHashSet.Add(id);
        SaveAllData(data);
    }

    public SettingData LoadSettingData()
    {
        return LoadData().SettingData;
    }
    
    public StageData LoadStageData()
    {
        return LoadData().StageData;
    }

    public bool LoadPrivacyConfirmData()
    {
        return LoadData().IsPrivacyConfirmation;
    }

    public List<string> LoadTalkedNpcData()
    {
        return LoadData().TalkedNpcHashSet;
    }
    
    public void DeleteSettingData()
    {
        SettingData settingData = new SettingData();
        settingData.ResolutionIndex = GameManager.Instance.SettingManager.ResolutionSetting.GetOptimalResolutionIndex();
        settingData.IsFullScreen = true;
        SaveSettingData(settingData);
    }

    public void DeleteStageData()
    {
        StageData stageData = new StageData();
        stageData.ClearStageIndex = 0;
        SaveStageData(stageData);
    }
    
    private void SaveAllData(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    private GameSaveData LoadData()
    {
        if (!File.Exists(SavePath))
        {
            return SetNewGameData();
        }

        string json = File.ReadAllText(SavePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
        return data;
    }

    private void DeleteData()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("@@DE: 삭제 실패");
            return;
        }

        File.Delete(SavePath);
        Debug.Log("@@DE: 삭제 완료");
    }

    private GameSaveData SetNewGameData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.SettingData = new SettingData();
        saveData.StageData = new StageData();
        saveData.IsPrivacyConfirmation = false;
        saveData.TalkedNpcHashSet = new List<string>();
        
        // 기본값 세팅
        saveData.SettingData.LanguageIndex = 
            GameManager.Instance.SettingManager.LanguageSetting.GetOptimalLanguage();
        saveData.SettingData.IsFullScreen = true;
        saveData.SettingData.ResolutionIndex =
            GameManager.Instance.SettingManager.ResolutionSetting.GetOptimalResolutionIndex();
        saveData.SettingData.BgmVolume =
            GameManager.Instance.SettingManager.AudioSetting.GetOptimalBgmVolume();
        saveData.SettingData.SfxVolume =
            GameManager.Instance.SettingManager.AudioSetting.GetOptimalSfxVolume();

        saveData.StageData.ClearStageIndex = 0;

        SaveAllData(saveData);
        return saveData;
    }
}

[Serializable]
public class GameSaveData
{
    public SettingData SettingData = new SettingData(); // 설정 데이터
    public StageData StageData = new StageData(); // 스테이지 데이터
    public bool IsPrivacyConfirmation; // 개인정보 처리방침 확인 여부
    public List<string> TalkedNpcHashSet = new List<string>(); // 대화한 Npc ID
}

[Serializable]
public class SettingData
{
    public int LanguageIndex;
    public int ResolutionIndex;
    public bool IsFullScreen;
    public float BgmVolume;
    public float SfxVolume;
}

[Serializable]
public class StageData
{
    public int ClearStageIndex;
}