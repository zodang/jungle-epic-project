using UnityEngine;

public class TimelineDialogue : MonoBehaviour
{
    private int _index = 0;
    private SpeechAnchor _speechAnchor;
    [SerializeField] private string[] dialogues;

    private void Awake()
    {
        _speechAnchor = GetComponentInChildren<SpeechAnchor>();
    }

    public void StartDialogue()
    {
        if (_index < dialogues.Length)
        {
            StageBaseManager.Instance.DialogueManager.StartDialogue(dialogues[_index], _speechAnchor.transform);
            _index++;
        }
    }

    public void FinishDialogue()
    {
        StageBaseManager.Instance.DialogueManager.FinalizeDialogue();
    }

    /// <summary>
    /// 타임라인 스킵 시, 건너뛴 대사의 수만큼 인덱스를 강제로 증가시킵니다.
    /// </summary>
    public void AdvanceDialogueIndex(int amount)
    {
        _index += amount;
    }
}
