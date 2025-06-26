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

    // [새로 추가/변경할 변수들]
    [Header("Choice Settings")]
    [SerializeField] private GameObject choiceItemPrefab; // 1단계에서 만든 ChoiceItemPrefab을 연결
    [SerializeField] private Transform choicesContainer;  // 2단계에서 만든 ChoicesContainer를 연결
    [SerializeField] private Color selectedChoiceColor = new Color(1, 1, 0.5f, 1); // 선택됐을 때 배경색 (노란색)
    [SerializeField] private Color defaultChoiceColor = new Color(1, 1, 1, 0.5f);   // 기본 배경색 (반투명 흰색)

    private List<GameObject> instantiatedChoiceItems = new List<GameObject>(); // 생성된 선택지 UI 오브젝트들을 관리할 리스트

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
        // if (StageBaseManager.Instance.DialogueManager != null) StageBaseManager.Instance.DialogueManager.ProcessNextActionInput();
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
    /// 이전에 생성된 선택지 UI들을 모두 파괴합니다.
    /// </summary>
    private void ClearChoices()
    {
        foreach (GameObject item in instantiatedChoiceItems)
        {
            Destroy(item);
        }
        instantiatedChoiceItems.Clear();
    }

    /// <summary>
    /// 선택지 목록을 UI에 표시합니다. (완전히 새로 작성된 메서드)
    /// </summary>
    public void DisplayChoicesInMainText(List<DialogueChoice> choices, int selectedIndex)
    {
        // 0. 기존에 있던 선택지 UI들을 깨끗하게 지웁니다.
        ClearChoices();

        if (choices == null || choices.Count == 0 || choiceItemPrefab == null || choicesContainer == null)
        {
            return;
        }
        
        // Localization: 현재 설정 언어
        string lang = UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale.Identifier.Code;

        // 1. 모든 선택지에 대해 루프를 돕니다.
        for (int i = 0; i < choices.Count; i++)
        {
            // 2. 프리팹으로부터 새로운 선택지 아이템 UI를 생성합니다.
            GameObject choiceInstance = Instantiate(choiceItemPrefab, choicesContainer);

            // 3. 자식 오브젝트에서 Image와 TextMeshProUGUI 컴포넌트를 찾습니다.
            Image background = choiceInstance.GetComponent<Image>();
            TextMeshProUGUI choiceText = choiceInstance.GetComponentInChildren<TextMeshProUGUI>();
            
            // Localization: 현재 설정 언어에 따른 DialogueChoice의 text 데이터 추출
            string choiceTextValue = "";
            if (choices[i].text != null && choices[i].text.TryGetValue(lang, out var val))
                choiceTextValue = val;
            
            if (choiceText != null)
            {
                // 4. 선택지 텍스트를 설정합니다.
                choiceText.text = choiceTextValue;
            }

            if (background != null)
            {
                // 5. 현재 인덱스(i)가 선택된 인덱스(selectedIndex)와 같은지 확인합니다.
                if (i == selectedIndex)
                {
                    // 선택된 항목: 지정된 색상으로 변경하고 텍스트를 볼드체로 만듭니다.
                    background.color = selectedChoiceColor;
                    if (choiceText != null) choiceText.fontStyle = FontStyles.Bold;
                }
                else
                {
                    // 선택되지 않은 항목: 기본 색상과 일반 텍스트 스타일로 설정합니다.
                    background.color = defaultChoiceColor;
                    if (choiceText != null) choiceText.fontStyle = FontStyles.Normal;
                }
            }

            // 관리 리스트에 추가합니다.
            instantiatedChoiceItems.Add(choiceInstance);
        }
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

    // Show 메서드도 수정이 필요합니다.
    public void Show(bool show)
    {
        gameObject.SetActive(show);
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(show);
        }

        if (!show)
        {
            // UI가 숨겨질 때, 생성했던 선택지 아이템들을 모두 제거합니다.
            ClearChoices();
            // 타이핑 효과가 있다면 중단
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