// DialogueUI.cs
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class DialogueUI : MonoBehaviour
{
    private const string DIALOGUE_TEXT_UI_NAME = "DialogueText"; // 이 UI가 제어할 주 텍스트 오브젝트 이름
    private const string SPEAKER_NAME_TEXT_UI_NAME = "SpeakerNameText";

    private TextMeshProUGUI mainTextTMP;      // 주 텍스트 표시용 (일반 대사 또는 선택지 목록)
    private TypeEffect mainTextTypeEffect; // 주 텍스트에 연결된 TypeEffect (있을 수도, 없을 수도 있음)
    private TextMeshProUGUI speakerNameTextTMP;

    private StringBuilder choiceStringBuilder = new StringBuilder();

    void Awake()
    {
        Debug.Log($"DialogueUI Awake called on GameObject: {gameObject.name}");

        Transform mainTextObj = transform.Find(DIALOGUE_TEXT_UI_NAME);
        if (mainTextObj != null)
        {
            mainTextTMP = mainTextObj.GetComponent<TextMeshProUGUI>();
            mainTextTypeEffect = mainTextObj.GetComponent<TypeEffect>(); // TypeEffect는 없을 수도 있음

            if (mainTextTMP == null)
            {
                Debug.LogError($"DialogueUI ERROR: Child '{DIALOGUE_TEXT_UI_NAME}' on '{gameObject.name}' does NOT have a TextMeshProUGUI component.");
                enabled = false; return;
            }
            // TypeEffect는 선택 사항이므로, 없어도 오류는 아님 (경고는 TypeEffect.Awake에서)
        }
        else
        {
            Debug.LogError($"DialogueUI ERROR: Child GameObject named '{DIALOGUE_TEXT_UI_NAME}' NOT FOUND under '{gameObject.name}'.");
            enabled = false; return;
        }

        Transform speakerNameTextObj = transform.Find(SPEAKER_NAME_TEXT_UI_NAME);
        if (speakerNameTextObj != null)
        {
            speakerNameTextTMP = speakerNameTextObj.GetComponent<TextMeshProUGUI>();
        }
        if (speakerNameTextTMP == null && !gameObject.name.Contains("ChoiceBubble")) // 선택지 버블에는 화자 이름이 없을 수 있음
        {
            Debug.LogWarning($"DialogueUI WARNING: Child '{SPEAKER_NAME_TEXT_UI_NAME}' not found on '{gameObject.name}'. Speaker name won't be shown.");
        }
    }

    /// <summary>
    /// 주 텍스트 영역에 텍스트를 설정합니다. TypeEffect가 있다면 사용합니다.
    /// </summary>
    public void SetMainText(string text, bool useTypingEffect)
    {
        if (mainTextTMP == null) return;

        if (useTypingEffect && mainTextTypeEffect != null)
        {
            mainTextTypeEffect.SetMsg(text);
        }
        else // 타이핑 효과를 사용하지 않거나 TypeEffect 컴포넌트가 없는 경우
        {
            if (mainTextTypeEffect != null && mainTextTypeEffect.IsPlaying) // 진행중인 효과가 있다면 강제종료
            {
                mainTextTypeEffect.FinishEffect();
            }
            mainTextTMP.text = text;
        }
    }

    public void SetSpeakerName(string name)
    {
        if (speakerNameTextTMP != null)
        {
            if (!string.IsNullOrEmpty(name))
            {
                speakerNameTextTMP.text = name;
                speakerNameTextTMP.gameObject.SetActive(true);
            }
            else
            {
                speakerNameTextTMP.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 선택지 목록을 주 텍스트 영역에 표시합니다. (TypeEffect 없이 즉시 표시)
    /// </summary>
    public void DisplayChoicesInMainText(List<DialogueChoice> choices, int selectedIndex)
    {
        if (mainTextTMP == null) return;

        if (choices == null || choices.Count == 0)
        {
            mainTextTMP.text = "";
            return;
        }

        // 선택지 표시 전, 혹시 진행 중인 타이핑 효과가 있다면 완료시킴
        if (mainTextTypeEffect != null && mainTextTypeEffect.IsPlaying)
        {
            mainTextTypeEffect.FinishEffect();
        }

        choiceStringBuilder.Clear();
        for (int i = 0; i < choices.Count; i++)
        {
            choiceStringBuilder.Append(i == selectedIndex ? "> " : "  ");
            choiceStringBuilder.AppendLine(choices[i].text);
        }
        if (choiceStringBuilder.Length > 0 && choiceStringBuilder[choiceStringBuilder.Length - 1] == '\n')
        {
            choiceStringBuilder.Length--;
        }
        mainTextTMP.text = choiceStringBuilder.ToString();
    }

    public bool IsTyping()
    {
        return mainTextTypeEffect != null && mainTextTypeEffect.IsPlaying;
    }

    public void CompleteTyping()
    {
        if (mainTextTypeEffect != null && mainTextTypeEffect.IsPlaying)
        {
            mainTextTypeEffect.FinishEffect();
        }
    }

    public void SetBubblePosition(Vector3 screenPosition)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null) rectTransform.position = screenPosition;
    }

    public void Show(bool show)
    {
        gameObject.SetActive(show);
        if (!show)
        {
            CompleteTyping(); // 숨겨질 때 타이핑 완료
        }
    }
}