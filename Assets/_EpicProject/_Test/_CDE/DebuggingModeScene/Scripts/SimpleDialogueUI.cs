using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleDialogueUI : MonoBehaviour
{
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button nextBtn;

    private SimpleDialogueLoader _dialogueLoader;
    private TypeEffect _typeEffect;
    
    private string _currentId;
    private int _currentLineIndex;
    private int _lineCount = 1;

    private void Awake()
    {
        _dialogueLoader = FindAnyObjectByType<SimpleDialogueLoader>();
        _typeEffect = dialogueText.GetComponent<TypeEffect>();
    }

    private void Start()
    {
        nextBtn.onClick.AddListener(SkipDialogue);
    }
    
    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;
        SkipDialogue();
    }

    public void ChangeSpeechBubbleUI(string id)
    {
        _currentId = id;
        _currentLineIndex = 0;
        _lineCount = _dialogueLoader.GetLineCount(id);
        
        StartDialogue();
    }

    private void StartDialogue()
    {
        // 데이터 변경
        string targetSpeaker = _dialogueLoader.GetSpeaker(_currentId, _currentLineIndex);
        string targetText = _dialogueLoader.GetDialogue(_currentId, _currentLineIndex);
        
        // 대사 진행
        speakerText.text = targetSpeaker;
        _typeEffect.SetMsg(targetText);
        
        // 버튼 비활성화
        nextBtn.gameObject.SetActive(_currentLineIndex + 1 < _lineCount);
    }

    private void SkipDialogue()
    {
        if (_typeEffect.IsPlaying)
        {
            _typeEffect.FinishEffect();
        }
        else
        {
            if (_currentLineIndex + 1 >= _lineCount) return;

            // 다음 대사 실행
            _currentLineIndex++;
            StartDialogue();
        }
    }
}
