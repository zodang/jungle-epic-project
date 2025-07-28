using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

[System.Serializable]
public class DialogueEventEntry
{
    public string DialogueId;
    public UnityEvent OnStartDialogue;
}

public class VisualNovelSystem : MonoBehaviour
{
    [Header("Content")]
    public List<string> DialogueIds = new List<string>();
    private int _dialogueIdIdx;
    private string _dialogueFileName = "StageInfos/Opening/Dialogues";
    private DialogueLoader _dialogueLoader;
    private DialogueCollection _dialogueCollection;
    private string _language = "ko";
    public List<DialogueEventEntry> DialogueEventEntries = new List<DialogueEventEntry>();

    [Header("UI")]
    public TextMeshProUGUI ContentText;
    public TextMeshProUGUI SpeakerText;
    private TypeEffect _typeEffect;

    [Header("Fonts")]
    public TMP_FontAsset Default_Font;
    public TMP_FontAsset ZhHans_Font;

    [Header("Illustration")]
    public Image IllustrationImage;
    public List<Sprite> Illustrations = new List<Sprite>();
    private int _illustrationsIdx;

    [Header("Credits Scroll")]
    public RectTransform CreditRoot;   // 에디터에서 Credit 오브젝트의 RectTransform 연결
    public float scrollSpeed = 5f;     // 기본 스크롤 속도
    public float fastMultiplier = 3f;  // 마우스 누를 때 배수
    public float targetY = 3400f;      // 최종 Y 위치


    private bool _isUsing;
    private bool _isEndAll;
    private float _delayDeltaTime;

    public Action OnFinish;

    void Start()
    {
        if (!ContentText.IsUnityNull())
        {
            _typeEffect = ContentText.GetComponent<TypeEffect>();
        }

        ComponentHelper.TryGetOrAddComponent<DialogueLoader>(ref _dialogueLoader, gameObject);

        LoadDialogue(_dialogueFileName);

        _dialogueIdIdx = 0;
        _illustrationsIdx = 0;

        _isUsing = false;
        _delayDeltaTime = 0f;
        _isEndAll = false;
        _language = LocalizationSettings.SelectedLocale.Identifier.Code;
    }

    void Update()
    {
        if (_isEndAll) return;

        if (!_isUsing && _delayDeltaTime <= 0f)
        {
            _delayDeltaTime = 2f;

            bool isEndIllustration = _illustrationsIdx >= Illustrations.Count;
            bool isEndDialogueId = _dialogueIdIdx >= DialogueIds.Count;

            if (isEndIllustration && isEndDialogueId)
            {
                _isEndAll = true;
                StartCoroutine(FadeThenCredits());
            }

            else
            {
                if (!isEndIllustration)
                {
                    SetImage(_illustrationsIdx);
                    _illustrationsIdx++;
                }
                if (!isEndDialogueId)
                {
                    StartDialogue(DialogueIds[_dialogueIdIdx]);
                    for (int i = 0; i < DialogueEventEntries.Count; i++)
                    {
                        if (DialogueEventEntries[i].DialogueId == DialogueIds[_dialogueIdIdx])
                        {
                            DialogueEventEntries[i].OnStartDialogue?.Invoke();
                            break;
                        }
                    }
                    _dialogueIdIdx++;
                }
                else
                {
                    SpeakerText.text = "";
                    _typeEffect.SetMsg("");
                }
            }
        }
        if (_delayDeltaTime > 0f)
        {
            _delayDeltaTime -= Time.deltaTime;
        }
    }

    public void LoadDialogue(string path)
    {
        _dialogueCollection = _dialogueLoader.LoadDialogueDataFromFile(path);
        if (_dialogueCollection == null)
        {
            Debug.LogError("DM: Failed to load dialogue collection. System disabled.");
            enabled = false; return;
        }
    }

    public void StartDialogue(string dialogueId)
    {
        if (_dialogueCollection == null) { Debug.LogError("DM: Dialogue collection not loaded."); return; }
        DialogueEntry entry = _dialogueLoader.GetDialogueEntryById(_dialogueCollection, dialogueId);
        if (entry == null) { Debug.LogWarning($"DM: Dialogue ID '{dialogueId}' not found."); return; }

        StartCoroutine(DialogueCoroutine(entry));
    }

    IEnumerator DialogueCoroutine(DialogueEntry entry)
    {
        float initialDelay = 0.1f;
        float postEffectDelay = 2f;
        int cnt = entry.lines.Count;

        _isUsing = true;
        for (int i = 0; i < cnt; i++)
        {
            _language = LocalizationSettings.SelectedLocale.Identifier.Code;
            switch (_language)
            {
                case "zh-Hans":
                    SpeakerText.font = ZhHans_Font;
                    ContentText.font = ZhHans_Font;
                    break;
                default:
                    SpeakerText.font = Default_Font;
                    ContentText.font = Default_Font;
                    break;
            }

            string speaker;
            if (entry.lines[i].speaker.TryGetValue(_language, out speaker))
            {
                SpeakerText.text = speaker;
            }

            string text;
            if (entry.lines[i].text.TryGetValue(_language, out text))
            {
                _typeEffect.SetMsg(text);
            }

            yield return new WaitForSeconds(initialDelay);
            while (_typeEffect.IsPlaying) yield return null;
            yield return new WaitForSeconds(postEffectDelay);
        }
        _isUsing = false;
    }

    void SetImage(int idx)
    {
        IllustrationImage.sprite = Illustrations[idx];
    }

    IEnumerator FadeCoroutine(bool isFadeIn, int fadeSteps, float stepDelay)
    {
        Color color = IllustrationImage.color;
        for (int i = 1; i <= fadeSteps; i++)
        {
            color.a = 1f / fadeSteps * i;
            if (!isFadeIn)
            {
                color.a = 1f - color.a;
            }
            IllustrationImage.color = color;
            yield return new WaitForSeconds(stepDelay);
        }
    }

    IEnumerator DelayFinishScene(float delay)
    {
        yield return new WaitForSeconds(delay);
        OnFinish?.Invoke();
    }

    IEnumerator FadeThenCredits()
    {
        // 1) 페이드 아웃이 끝날 때까지 대기
        yield return StartCoroutine(FadeCoroutine(false, 20, 0.15f));

        if (CreditRoot.IsUnityNull())
        {
            StartCoroutine(DelayFinishScene(1f));
            yield break;
        }
            

        // 2) 에디터에서 false로 꺼둔 크레딧 오브젝트 활성화
        CreditRoot.gameObject.SetActive(true);

        yield return new WaitForSeconds(4f);

        // 3) 활성화된 크레딧을 스크롤
        yield return StartCoroutine(CreditScrollCoroutine());
    }

    IEnumerator CreditScrollCoroutine()
    {
        // 크레딧 루트가 시작 Y = 0 이라고 가정
        Vector2 pos = CreditRoot.anchoredPosition;

        while (pos.y < targetY)
        {
            // 마우스(왼쪽 버튼) 누르고 있으면 속도 3배
            float speed = scrollSpeed * (Input.GetMouseButton(0) ? fastMultiplier : 1f);
            pos.y += speed * Time.deltaTime;
            CreditRoot.anchoredPosition = pos;
            yield return null;
        }

        // 목표 도달하면 즉시 씬 종료 콜
        StartCoroutine(DelayFinishScene(4f));
    }
}
