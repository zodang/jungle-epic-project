// TypeEffect.cs (원래의 스킵 기능만 있는 코루틴 버전)
using UnityEngine;
using TMPro;
using System.Collections;
using System.Text;
using Define;

public class TypeEffect : MonoBehaviour
{
    [Header("Typing Speed")]
    [Tooltip("초당 표시할 기본 글자 수")]
    public float charsPerSecond = 15f;

    private TextMeshProUGUI msgText;
    private AudioManager audioManager;
    private string targetMsg;

    public bool IsPlaying { get; private set; }
    private Coroutine typingCoroutine;


    private int charCountForSound = 0; // 효과음 재생 간격 카운터

    private bool _isPaused = false;

    private void Awake()
    {
        msgText = GetComponent<TextMeshProUGUI>();
        if (msgText == null) { enabled = false; Debug.LogError($"TypeEffect CRITICAL ERROR: TextMeshProUGUI not found on '{gameObject.name}'.", gameObject); }

        if (msgText != null)
        {
            msgText.richText = true;
        }
    }

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.AudioManager != null)
        {
            audioManager = GameManager.Instance.AudioManager;
        }
    }

    public void SetMsg(string msg)
    {
        if (!enabled || msgText == null) return;
        targetMsg = msg ?? "";
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        EffectStart();
    }

    private void EffectStart()
    {
        msgText.text = "";
        IsPlaying = true;
        if (charsPerSecond <= 0 || string.IsNullOrEmpty(targetMsg))
        {
            msgText.text = targetMsg;
            CompleteEffect();
            return;
        }
        typingCoroutine = StartCoroutine(EffectRoutine());
    }

    private IEnumerator EffectRoutine()
    {
        StringBuilder stringBuilder = new StringBuilder();
        int currentIndex = 0;

        while (currentIndex < targetMsg.Length)
        {
            // 일시 정지 기능
            while (_isPaused) yield return null;
            
            if (targetMsg[currentIndex] == '<')
            {
                int endIndex = targetMsg.IndexOf('>', currentIndex);
                if (endIndex != -1)
                {
                    string tag = targetMsg.Substring(currentIndex, endIndex - currentIndex + 1);
                    stringBuilder.Append(tag);
                    currentIndex = endIndex + 1;
                    msgText.text = stringBuilder.ToString();
                    continue;
                }
            }

            stringBuilder.Append(targetMsg[currentIndex]);
            msgText.text = stringBuilder.ToString();

            if (audioManager != null && targetMsg[currentIndex] != ' ')
            {
                audioManager.PlaySfx(SfxType.Text);
            }

            currentIndex++;

            if (charsPerSecond > 0)
            {
                yield return new WaitForSecondsRealtime(1.0f / charsPerSecond);
            }
            else
            {
                yield return null;
            }
        }
        CompleteEffect();
    }

    private void CompleteEffect()
    {
        IsPlaying = false;
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }

    /// <summary>
    /// 타이핑 효과를 즉시 완료하고 전체 텍스트를 표시합니다.
    /// </summary>
    public void FinishEffect()
    {
        if (!enabled || msgText == null) return;

        msgText.text = targetMsg;
        CompleteEffect();
    }

    private void OnEnable()
    {
        GameManager.Instance.SettingManager.OnSettingOpen += Pause;
        GameManager.Instance.SettingManager.OnSettingClose += Resume;
    }

    private void OnDisable()
    {
        GameManager.Instance.SettingManager.OnSettingOpen -= Pause;
        GameManager.Instance.SettingManager.OnSettingClose -= Resume;
    }

    private void Pause()
    {
        _isPaused = true;
    }

    private void Resume()
    {
        _isPaused = false;
    }
}