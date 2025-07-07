using System;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private readonly string _saveFileName = "SaveFile.json";
    private string SavePath => Path.Combine(Application.persistentDataPath, _saveFileName);

    public void SaveData(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"저장 완료: {SavePath}");
    }

    public GameSaveData LoadData()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("로드 실패");
            return null;
        }
        
        string json = File.ReadAllText(SavePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
        Debug.Log("로드 완료");
        return data;
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