// DialogueLoader.cs
using UnityEngine;
using System.Linq; // FirstOrDefault 사용 시 (선택적 헬퍼 함수용)

public class DialogueLoader : MonoBehaviour
{
    
    public DialogueCollection LoadDialogueDataFromFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            Debug.LogError("<DialogueLoader> File name is null or empty. Cannot load dialogue data.");
            return null;
        }

        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);
        if (jsonFile != null)
        {
            Debug.Log($"<DialogueLoader> Successfully loaded JSON file: {fileName}.json from Resources.");
            try
            {
                DialogueCollection collection = JsonUtility.FromJson<DialogueCollection>(jsonFile.text);
                if (collection?.dialogues == null) // JsonUtility는 파싱 실패 시 null 또는 빈 객체를 반환할 수 있음
                {
                    Debug.LogError($"<DialogueLoader> Failed to parse JSON data from '{fileName}'. The 'dialogues' list might be missing or the JSON structure is incorrect. JSON Content: \n{jsonFile.text}");
                    return null;
                }
                Debug.Log($"<DialogueLoader> Successfully parsed JSON. Dialogues count: {collection.dialogues.Count}");
                return collection;
            }
            catch (System.Exception ex) // JsonUtility가 파싱 중 예외를 던질 경우 대비
            {
                Debug.LogError($"<DialogueLoader> Exception during JSON parsing from '{fileName}': {ex.Message}\nJSON Content: \n{jsonFile.text}");
                return null;
            }
        }
        else
        {
            Debug.LogError($"<DialogueLoader> Failed to load 'Resources/{fileName}.json'. File not found or file name is incorrect.");
            return null;
        }
    }

    /// <summary>
    /// (선택적 헬퍼 함수) DialogueCollection 내에서 특정 ID의 DialogueEntry를 찾습니다.
    /// </summary>
    public DialogueEntry GetDialogueEntryById(DialogueCollection collection, string dialogueId)
    {
        if (collection?.dialogues == null || string.IsNullOrEmpty(dialogueId))
        {
            return null;
        }
        return collection.dialogues.FirstOrDefault(d => d.id == dialogueId);
    }
}