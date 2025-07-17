// ProgressManager.cs (수정)
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    // NPC ID 대신, 이미 본 '대화 ID'를 저장
    private HashSet<string> seenDialogueIds = new HashSet<string>();

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    /// <summary>
    /// 특정 대화 ID를 "이미 봄"으로 기록합니다.
    /// </summary>
    public void MarkDialogueAsSeen(string dialogueId)
    {
        if (!string.IsNullOrEmpty(dialogueId) && !seenDialogueIds.Contains(dialogueId))
        {
            seenDialogueIds.Add(dialogueId);
            Debug.Log($"<ProgressManager> Dialogue ID '{dialogueId}' marked as seen.");
        }
    }

    /// <summary>
    /// 특정 대화 ID를 이미 보았는지 확인합니다.
    /// </summary>
    public bool HasSeenDialogue(string dialogueId)
    {
        if (string.IsNullOrEmpty(dialogueId)) return true; // ID가 없는 경우는 본 것으로 간주하여 '새로운' 표시 안 함
        return seenDialogueIds.Contains(dialogueId);
    }

    // 게임 저장/로드 시 이 seenDialogueIds 데이터를 저장하고 불러와야 함
}