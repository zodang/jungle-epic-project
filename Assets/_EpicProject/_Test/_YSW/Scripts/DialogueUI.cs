// DialogueUI.cs
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class DialogueUI : MonoBehaviour
{
    // UI 오브젝트 이름 상수 (프리팹 내부 자식 GameObject 이름과 일치해야 함)
    private const string DIALOGUE_TEXT_UI_NAME = "DialogueText";
    private const string SPEAKER_NAME_TEXT_UI_NAME = "SpeakerNameText";

    private TextMeshProUGUI dialogueTextUI;
    private TextMeshProUGUI speakerNameTextUI;

    private StringBuilder choiceStringBuilder = new StringBuilder();

    // 이 스크립트가 활성화될 때 자동으로 호출됨
    void Awake()
    {
        // 자식 GameObject에서 이름으로 TextMeshProUGUI 컴포넌트 찾기
        Transform dialogueTextObj = transform.Find(DIALOGUE_TEXT_UI_NAME);
        if (dialogueTextObj != null)
        {
            dialogueTextUI = dialogueTextObj.GetComponent<TextMeshProUGUI>();
        }

        Transform speakerNameTextObj = transform.Find(SPEAKER_NAME_TEXT_UI_NAME);
        if (speakerNameTextObj != null)
        {
            speakerNameTextUI = speakerNameTextObj.GetComponent<TextMeshProUGUI>();
        }

        // 필수 UI 요소 확인
        if (dialogueTextUI == null)
        {
            Debug.LogError($"DialogueUI on '{gameObject.name}': Child GameObject named '{DIALOGUE_TEXT_UI_NAME}' with TextMeshProUGUI component not found. This UI will not function correctly.");
            enabled = false; // 스크립트 비활성화
            return;
        }

        if (speakerNameTextUI == null)
        {
            Debug.LogWarning($"DialogueUI on '{gameObject.name}': Child GameObject named '{SPEAKER_NAME_TEXT_UI_NAME}' with TextMeshProUGUI component not found. Speaker name will not be displayed.");
        }
    }

    public void SetDialogueText(string text)
    {
        if (dialogueTextUI != null)
        {
            dialogueTextUI.text = text;
        }
        // Awake에서 dialogueTextUI가 null이면 스크립트가 비활성화되므로,
        // 여기서 추가적인 null 체크는 필수는 아닐 수 있지만, 안전을 위해 둘 수 있음.
    }

    public void SetSpeakerName(string name)
    {
        if (speakerNameTextUI != null) // speakerNameTextUI는 선택 사항일 수 있음
        {
            if (!string.IsNullOrEmpty(name))
            {
                speakerNameTextUI.text = name;
                speakerNameTextUI.gameObject.SetActive(true);
            }
            else
            {
                speakerNameTextUI.gameObject.SetActive(false);
            }
        }
    }

    public void UpdateChoicesVisual(List<DialogueChoice> choices, int selectedIndex)
    {
        if (dialogueTextUI == null || choices == null || choices.Count == 0)
        {
            if (dialogueTextUI != null) dialogueTextUI.text = ""; // 선택지 표시 불가 시 텍스트 비움
            return;
        }

        choiceStringBuilder.Clear();
        for (int i = 0; i < choices.Count; i++)
        {
            choiceStringBuilder.Append(i == selectedIndex ? "> " : "  ");
            choiceStringBuilder.AppendLine(choices[i].text);
        }
        // 마지막 줄바꿈 제거 (AppendLine은 항상 줄바꿈을 추가하므로)
        if (choiceStringBuilder.Length > 0 && choiceStringBuilder[choiceStringBuilder.Length - 1] == '\n')
        {
            choiceStringBuilder.Length--;
        }
        dialogueTextUI.text = choiceStringBuilder.ToString();
    }

    public void SetBubblePosition(Vector3 screenPosition)
    {
        RectTransform rectTransform = GetComponent<RectTransform>(); // 이 UI의 RectTransform
        if (rectTransform != null)
        {
            rectTransform.position = screenPosition;
        }
    }

    public void Show(bool show)
    {
        gameObject.SetActive(show); // 이 UI GameObject 자체를 활성화/비활성화
    }

    // 만약 선택지 UI가 개별 버튼 등으로 복잡하게 구성된다면,
    // 선택지 UI를 초기화/정리하는 함수도 필요할 수 있음
    // public void ClearChoiceDisplayElements() { /* ... */ }
}