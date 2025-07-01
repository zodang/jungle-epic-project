using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public string defaultDialogueId;
    private SpeechAnchor _speechAnchor;

    private void Awake()
    {
        _speechAnchor = transform.GetComponentInChildren<SpeechAnchor>();
    }

    public void TriggerDialogue()
    {
        // Default ID 대사 시작
        if (StageBaseManager.Instance.DialogueManager != null && !string.IsNullOrEmpty(defaultDialogueId) && _speechAnchor != null)
        {
            StageBaseManager.Instance.DialogueManager.StartDialogue(defaultDialogueId, _speechAnchor.transform);
        }
    }
    
    public void TriggerDialogue(string dialogueId)
    {
        // 특정 ID 대사 시작
        if (StageBaseManager.Instance.DialogueManager != null && !string.IsNullOrEmpty(dialogueId) && _speechAnchor != null)
        {
            StageBaseManager.Instance.DialogueManager.StartDialogue(defaultDialogueId, _speechAnchor.transform);
        }
    }
}
