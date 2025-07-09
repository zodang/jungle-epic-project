using UnityEngine;

public class TimelineDialogue : MonoBehaviour
{
    private int _index = 0;
    private SpeechAnchor _speechAnchor;
    public string[] dialogues;

    private void Awake()
    {
        _speechAnchor = GetComponentInChildren<SpeechAnchor>();
    }

    public void StartDialogue()
    {
        StageBaseManager.Instance.DialogueManager.StartDialogue(dialogues[_index], _speechAnchor.transform);
        _index++;
    }
}
