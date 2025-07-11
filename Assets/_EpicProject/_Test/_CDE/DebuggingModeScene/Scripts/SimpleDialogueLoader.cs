using System.Linq;
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
        var entry = _dialogueCollection.dialogues.FirstOrDefault(d => d.id == id);
        return entry.lines.Count;
    }

    public string GetSpeaker(string id, int index = 0)
    {
        string speaker = "";

        // 현재 언어 코드
        string lang = UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale.Identifier.Code;
        Debug.Log(lang);

        var entry = _dialogueCollection.dialogues.FirstOrDefault(d => d.id == id);
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

        var entry = _dialogueCollection.dialogues.FirstOrDefault(d => d.id == id);
        if (entry.lines[index].text.TryGetValue(lang, out var txt))
        {
            dialogue = txt;
        }

        return dialogue;
    }
}
