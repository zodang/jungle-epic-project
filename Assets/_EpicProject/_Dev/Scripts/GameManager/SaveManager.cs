using System;
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
        SaveData(data);
    }

    public SettingData LoadSettingData()
    {
        return LoadData().SettingData;
    }

    public void SaveStageData(StageData stage)
    {
        GameSaveData data = LoadData();
        data.StageData = stage;
        SaveData(data);
    }

    public StageData LoadStageData()
    {
        return LoadData().StageData;
    }
    
    private void SaveData(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"저장 완료: {SavePath}");
    }

    private GameSaveData LoadData()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("새 데이터 생성");
            return SetNewGameData();
        }

        string json = File.ReadAllText(SavePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
        Debug.Log($"로드 완료 : {SavePath}");
        return data;
    }

    private void DeleteAllData()
    {
        SaveData(new GameSaveData());
    }

    public void DeleteData()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("삭제 실패");
            return;
        }

        File.Delete(SavePath);
        Debug.Log("삭제 완료");
    }

    private GameSaveData SetNewGameData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.SettingData = new SettingData();
        saveData.StageData = new StageData();
        
        // 기본값 세팅
        saveData.SettingData.ResolutionIndex =
            GameManager.Instance.SettingManager.ResolutionSetting.GetOptimalResolutionIndex();
        saveData.SettingData.IsFullScreen = true;

        SaveData(saveData);
        return saveData;
    }
}

[Serializable]
public class GameSaveData
{
    public SettingData SettingData = new SettingData();
    public StageData StageData = new StageData();
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