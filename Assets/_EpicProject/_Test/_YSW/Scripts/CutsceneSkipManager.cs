using UnityEngine;
using UnityEngine.Playables; // PlayableDirector를 사용하기 위해 필요
using UnityEngine.Events;   // UnityEvent를 사용하기 위해 필요
// using UnityEngine.UI;    // UI.Image를 사용하려면 이 줄의 주석을 해제하세요.

/// <summary>
/// 컷신 타임라인의 스킵 기능을 관리하는 스크립트입니다.
/// 지정된 키를 꾹 누르고 있으면 타임라인을 중지하고, OnCutsceneFinished 이벤트를 호출합니다.
/// </summary>
public class CutsceneSkipManager : MonoBehaviour
{
    [Header("핵심 연결")]
    [Tooltip("이 씬에서 스킵할 대상인 PlayableDirector를 연결하세요.")]
    public PlayableDirector cutsceneTimeline;

    [Header("컷신 종료 시 실행할 작업들")]
    [Tooltip("컷신이 스킵되거나 정상 종료되었을 때 실행할 함수들을 여기에 연결하세요.")]
    public UnityEvent OnCutsceneFinished;

    /*
    // [UI 연결 - 선택 사항]
    // 스킵 안내 UI를 사용하고 싶을 때, 아래 변수들의 주석을 해제하고 인스펙터에서 연결하세요.
    [Header("UI 요소 (선택 사항)")]
    [Tooltip("스킵 안내 UI의 부모 오브젝트입니다.")]
    public GameObject skipPromptUI;
    [Tooltip("진행 상태를 보여줄 채워지는(Filled) 이미지입니다.")]
    public Image skipProgressImage;
    */

    [Header("스킵 설정")]
    public KeyCode skipKey = KeyCode.Escape;
    [Tooltip("스킵을 위해 키를 누르고 있어야 하는 시간(초)")]
    public float timeToSkip = 1.5f;

    // 내부 변수
    private float skipTimer = 0f;
    private bool isSkipped = false;

    void Start()
    {
        // UI가 연결되어 있다면, 시작 시 숨깁니다.
        /*
        if (skipPromptUI != null)
            skipPromptUI.SetActive(false);
        */

        // 필수 컴포넌트 확인
        if (cutsceneTimeline == null)
        {
            Debug.LogError("오류: 스킵할 타임라인(PlayableDirector)이 할당되지 않았습니다!", this.gameObject);
            this.enabled = false; // 스크립트 비활성화
        }
    }

    void Update()
    {
        // 스킵이 이미 완료되었거나, 타임라인이 재생 중이 아닐 때는 아무것도 하지 않습니다.
        if (isSkipped || (cutsceneTimeline != null && cutsceneTimeline.state != PlayState.Playing))
        {
            /*
            if (skipPromptUI != null && skipPromptUI.activeInHierarchy)
                skipPromptUI.SetActive(false);
            */
            return;
        }

        // 스킵 키를 누르고 있는 동안
        if (Input.GetKey(skipKey))
        {
            // Time.timeScale이 0이어도 시간을 측정하기 위해 unscaledDeltaTime 사용
            skipTimer += Time.unscaledDeltaTime;

            // UI 로직: 스킵 안내 UI를 보여주고 진행 바를 채웁니다.
            /*
            if (skipPromptUI != null && !skipPromptUI.activeInHierarchy)
                skipPromptUI.SetActive(true);
            
            if (skipProgressImage != null)
                skipProgressImage.fillAmount = skipTimer / timeToSkip;
            */

            // 타이머가 목표 시간을 채우면 스킵을 실행합니다.
            if (skipTimer >= timeToSkip)
            {
                Skip();
            }
        }

        // 스킵 키에서 손을 뗐을 때
        if (Input.GetKeyUp(skipKey))
        {
            // 타이머와 진행 바를 초기화합니다.
            skipTimer = 0f;

            // UI 로직: 진행 바를 초기화하고 스킵 안내 UI를 숨깁니다.
            /*
            if (skipProgressImage != null)
                skipProgressImage.fillAmount = 0f;
            
            if (skipPromptUI != null)
                skipPromptUI.SetActive(false);
            */
        }
    }

    /// <summary>
    /// 스킵 기능을 실행하여 타임라인을 정지시키고, 종료 이벤트를 즉시 호출합니다.
    /// </summary>
    public void Skip()
    {
        if (isSkipped) return; // 중복 실행 방지

        isSkipped = true;
        Debug.Log("컷신이 스킵되었습니다. OnCutsceneFinished 이벤트를 실행합니다.");

        // 타임라인 재생을 즉시 멈춥니다. (중간 시그널이 발동되지 않도록)
        if (cutsceneTimeline != null)
        {
            cutsceneTimeline.Stop();
        }

        // 인스펙터의 OnCutsceneFinished 이벤트에 연결된 모든 함수들을 실행합니다.
        OnCutsceneFinished.Invoke();

        // 스킵 UI를 비활성화합니다.
        /*
        if (skipPromptUI != null)
        {
            skipPromptUI.SetActive(false);
        }
        */

        // 이 스크립트의 역할은 끝났으므로 비활성화합니다.
        this.enabled = false;
    }
}
