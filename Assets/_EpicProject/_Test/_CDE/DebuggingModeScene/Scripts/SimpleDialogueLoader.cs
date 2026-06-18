using UnityEngine;

public class SimpleDialogueLoader : MonoBehaviour
{
    private DialogueLoader _dialogueLoader;
    private DialogueCollection _dialogueCollection;

    private void Awake()
    {
        _dialogueLoader = FindAnyObjectByType<DialogueLoader>();
        _dialogueCollection = _dialogueLoader.LoadDialogueDataFromFile("StageInfos/DebuggingModeStage/Dialogues");
    }

    public int GetLineCount(string id)
    {
        var entry = GetDialogueEntry(id);
        return entry.lines.Count;
    }

    public string GetSpeaker(string id, int index = 0)
    {
        string speaker = "";

        // 현재 언어 코드
        string lang = UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale.Identifier.Code;

        var entry = GetDialogueEntry(id);
        if (entry.lines[index].speaker.TryGetValue(lang, out var spk))
        {
            speaker = spk;
        }

        return speaker;
    }

    public string GetDialogue(string id, int index = 0)
    {
        string dialogue = "";

        // 현재 언어 코드
        string lang = UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale.Identifier.Code;

        var entry = GetDialogueEntry(id);
        if (entry.lines[index].text.TryGetValue(lang, out var txt))
        {
            dialogue = txt;
        }

        return dialogue;
    }

    private DialogueEntry GetDialogueEntry(string id)
    {
        // 전체 dialogues 중 특정 대화 묶음(id)에서 특정 줄(index) 반환
        for (int i = 0; i < _dialogueCollection.dialogues.Count; i++)
        {
            DialogueEntry entry = _dialogueCollection.dialogues[i];
            if (entry.id == id)
            {
                return entry;
            }
        }

        return null;
    }
}
