// DialogueUI.cs
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Events;
using UnityEngine.UI; // Button 사용을 위해 추가

public class DialogueUI : MonoBehaviour
{
    private const string DIALOGUE_TEXT_UI_NAME = "DialogueText"; // 이 UI가 제어할 주 텍스트 오브젝트 이름
    private const string SPEAKER_NAME_TEXT_UI_NAME = "SpeakerNameText";
    private const string NEXT_BUTTON_NAME = "Next Button"; // 버튼 GameObject 이름

    private TextMeshProUGUI mainTextTMP;      // 주 텍스트 표시용 (일반 대사 또는 선택지 목록)
    private TypeEffect mainTextTypeEffect; // 주 텍스트에 연결된 TypeEffect (있을 수도, 없을 수도 있음)
    private TextMeshProUGUI speakerNameTextTMP;
    private Button nextButton; // 버튼 참조 추가

    // DialogueManager에게 "다음" 액션을 요청하기 위한 이벤트 (선택적이지만 좋은 방식)
    public UnityEvent onNextActionRequested = new UnityEvent();
    // 또는 public System.Action OnNextActionRequested;

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

        // Next Button 참조 및 리스너 연결
        Transform nextButtonObj = transform.Find(NEXT_BUTTON_NAME);
        if (nextButtonObj != null)
        {
            nextButton = nextButtonObj.GetComponent<Button>();
            if (nextButton != null)
            {
                // 버튼 클릭 시 호출될 함수 연결
                nextButton.onClick.AddListener(HandleNextButtonClick);
            }
            else
            {
                Debug.LogWarning($"DialogueUI on '{gameObject.name}': Button component not found on '{NEXT_BUTTON_NAME}'.");
            }
        }
        else
        {
            Debug.LogWarning($"DialogueUI on '{gameObject.name}': Child GameObject named '{NEXT_BUTTON_NAME}' not found.");
        }
    }

    private void HandleNextButtonClick()
    {
        Debug.Log($"DialogueUI on '{gameObject.name}': Next Button Clicked!");
        // DialogueManager에게 다음 액션 요청 (이벤트 방식)
        onNextActionRequested.Invoke();
        // 또는 직접 DialogueManager의 함수 호출 (결합도가 높아짐)
        // if (DialogueManager.Instance != null) DialogueManager.Instance.ProcessNextActionInput();
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
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(show); // 말풍선 보일 때 버튼도 보이게 (선택지 상황에서는 다를 수 있음)
        }
        if (!show)
        {
            CompleteTyping();
        }
    }

    /// <summary>
    /// 선택지 표시 중에는 Next 버튼을 숨길지 여부 등을 결정하는 함수
    /// </summary>
    public void SetNextButtonVisibilityForChoices(bool visible)
    {
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(visible);
        }
    }
}