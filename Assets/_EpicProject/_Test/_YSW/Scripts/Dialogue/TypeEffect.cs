// TypeEffect.cs (빨리 감기 기능 추가 버전)
using UnityEngine;
using TMPro;
using System.Collections;
using System.Text;
using Define; // AudioManager에서 사용하는 enum이 있다면

public class TypeEffect : MonoBehaviour
{
    [Header("Typing Speed")]
    [Tooltip("초당 표시할 기본 글자 수")]
    public float charsPerSecond = 15f;
    [Tooltip("키를 꾹 누를 때의 초당 글자 수. 기본 속도보다 커야 합니다.")]
    public float fastForwardSpeed = 50f;

    [Header("Sound Settings")]
    [Tooltip("빨리 감기 시 몇 글자마다 소리를 낼지 결정합니다. (0이면 매번, 3이면 3글자마다)")]
    public int fastForwardSoundInterval = 3; // 3글자마다 한 번 소리

    private TextMeshProUGUI msgText;
    private AudioManager audioManager;
    private string targetMsg;

    public bool IsPlaying { get; private set; }
    private bool isFastForwarding = false; // 빨리 감기 상태 플래그

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
        // GameManager가 싱글톤이고 AudioManager를 가지고 있다고 가정
        if (GameManager.Instance != null && GameManager.Instance.AudioManager != null)
        {
            audioManager = GameManager.Instance.AudioManager;
        }
        else
        {
            // AudioManager를 못 찾았을 경우 경고. (필수는 아니므로)
            // Debug.LogWarning($"TypeEffect on {gameObject.name}: AudioManager not found. Typing sound will not play.");
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
        isFastForwarding = false; // 효과 시작 시 빨리 감기 모드 해제
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
                bool shouldPlaySound = false;
                if (!isFastForwarding) // 빨리 감기가 아니면 항상 소리 재생
                {
                    shouldPlaySound = true;
                }
                else // 빨리 감기 중일 때
                {
                    charCountForSound++;
                    // fastForwardSoundInterval 값마다 소리를 재생 (0이면 매번 재생)
                    if (fastForwardSoundInterval <= 0 || charCountForSound % fastForwardSoundInterval == 0)
                    {
                        shouldPlaySound = true;
                    }
                }

                if (shouldPlaySound)
                {
                    audioManager.PlaySfx(SfxType.Text);
                }
            }

            currentIndex++;

            // 현재 속도 결정
            float currentSpeed = isFastForwarding ? fastForwardSpeed : charsPerSecond;
            if (currentSpeed > 0)
            {
                yield return new WaitForSecondsRealtime(1.0f / currentSpeed);
            }
            else
            {
                yield return null; // 0 이하일 경우 한 프레임에 한 글자씩
            }
        }
        CompleteEffect();
    }

    private void CompleteEffect()
    {
        IsPlaying = false;
        isFastForwarding = false; // 효과 종료 시 확실하게 해제
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }

    /// <summary>
    /// 타이핑 효과를 강제로 즉시 완료합니다. (Next 버튼 등에 사용)
    /// </summary>
    public void FinishEffect()
    {
        if (!enabled || msgText == null) return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        msgText.text = targetMsg;
        CompleteEffect(); // IsPlaying = false 및 기타 정리 작업
    }



    /// <summary>
    /// 타이핑 속도를 빨리 감기 모드로 전환하거나 해제합니다.
    /// </summary>
    public void SetFastForward(bool fastForward)
    {
        isFastForwarding = fastForward;
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