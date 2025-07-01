using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public string defaultDialogueId;
    [SerializeField] private Transform speechAnchor;
    public List<NPCInteraction.ConditionalDialogueEntry> conditionalDialogues = new List<NPCInteraction.ConditionalDialogueEntry>();

    private void Awake()
    {
        conditionalDialogues.Sort((a, b) => a.priority.CompareTo(b.priority));
    }

    public void TriggerDialogue()
    {
        string dialogueIdToStart = GetDialogueIdBasedOnConditions();

        if (StageBaseManager.Instance.DialogueManager != null && !string.IsNullOrEmpty(dialogueIdToStart) && speechAnchor != null)
        {
            StageBaseManager.Instance.DialogueManager.StartDialogue(dialogueIdToStart, speechAnchor);
        }
    }
    
    public void TriggerDialogue(string dialogueId)
    {
        // 특정 ID 대사 시작
        if (StageBaseManager.Instance.DialogueManager != null && !string.IsNullOrEmpty(dialogueId) && speechAnchor != null)
        {
            StageBaseManager.Instance.DialogueManager.StartDialogue(dialogueId, speechAnchor);
        }
    }
    
    private string GetDialogueIdBasedOnConditions()
    {
        if (StageBaseManager.Instance.FlagManager == null)
        {
            return defaultDialogueId;
        }

        // 설정된 조건부 대화들을 순서대로 확인 (리스트의 순서가 우선순위)
        foreach (var conditionEntry in conditionalDialogues)
        {
            if (string.IsNullOrEmpty(conditionEntry.requiredFlagName) || string.IsNullOrEmpty(conditionEntry.dialogueId))
            {
                // 필수 정보 누락 시 이 조건은 건너뜀
                continue;
            }

            // FlagManager를 통해 플래그 상태 확인
            bool flagState = StageBaseManager.Instance.FlagManager.IsFlagSet(conditionEntry.requiredFlagName);

            // 플래그 상태가 요구되는 값과 일치하는지 확인
            if (flagState == conditionEntry.requiredFlagValue)
            {
                return conditionEntry.dialogueId;
            }
        }

        return defaultDialogueId;
    }
}
