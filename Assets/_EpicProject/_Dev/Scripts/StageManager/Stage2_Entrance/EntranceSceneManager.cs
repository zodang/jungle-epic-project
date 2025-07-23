using Define;
using System.Collections.Generic;
using UnityEngine;

public class EntranceSceneManager : StageBaseManager
{
    [Header("그래픽 도전과제")]
    [Tooltip("상태를 확인할 GraphicHandler 오브젝트들을 모두 등록하세요.")]
    [SerializeField] private List<GraphicHandler> graphicHandlersToCheck; // << 추가

    

    private TriggerArea _goalTrigger;
    
    protected override void Awake()
    {
        // 스테이지 정보 불러오기
        stageFilePath = "StageInfos/EntranceStage";
        ChangeStageId("40_entrance_1");
        
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        // 대화 불러오기
        DialogueManager.LoadDialogue(stageFilePath + "/Dialogues");
        
        _goalTrigger = GameObject.FindWithTag(Tags.Goal)?.GetComponent<TriggerArea>();
        _goalTrigger.OnTrigger.AddListener(OnGoalTriggered);

        GameManager.Instance.AudioManager.PlayBgm(BgmType.Stage3);
        
        // 로그 시스템
        GameManager.Instance.LogManager.LogStageEnter(StageId);
        ChangeStageSection("40_0_enter_entrance");

        // 리스트에 있는 모든 GraphicHandler의 OnSetValue 이벤트에 CheckAllGraphicsAreHigh 함수를 연결(구독)
        if (graphicHandlersToCheck != null && graphicHandlersToCheck.Count > 0)
        {
            foreach (var handler in graphicHandlersToCheck)
            {
                // handler의 SetValue가 호출될 때마다 CheckAllGraphicsAreHigh 함수도 같이 호출됨
                handler.OnSetValue += CheckAllGraphicsAreHigh;
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy(); // StageBaseManager의 OnDestroy가 있다면 호출

        // 구독했던 이벤트를 모두 해지
        if (graphicHandlersToCheck != null && graphicHandlersToCheck.Count > 0)
        {
            foreach (var handler in graphicHandlersToCheck)
            {
                if (handler != null) // 오브젝트가 먼저 파괴되었을 경우를 대비
                {
                    handler.OnSetValue -= CheckAllGraphicsAreHigh;
                }
            }
        }
    }

    private void CheckAllGraphicsAreHigh(int changedIndex)
    {
        // 이미 도전과제가 해금되었다면 아무것도 하지 않음
        if (AchievementStatusManager._isAllHighAchievementUnlocked) return;

        // 리스트에 있는 모든 핸들러를 순회
        foreach (var handler in graphicHandlersToCheck)
        {
            // 단 하나라도 그래픽 타입 인덱스가 2(High)가 아니라면,
            // 조건을 만족하지 않으므로 즉시 함수를 종료.
            if ((int)handler.GetCurrentValue() != 2)
            {
                return;
            }
        }

        // 위 반복문이 중간에 종료되지 않고 끝까지 실행되었다면,
        // 모든 오브젝트의 그래픽이 High(인덱스 2)라는 의미입니다.

        AchievementStatusManager._isAllHighAchievementUnlocked = true;
        SteamAchievementManager.Instance.UnlockAchievement("ACH_SECRET_ALL_HIGH_GRAPHICS"); // API 이름은 예시입니다.
        Debug.Log("도전과제 달성: 모든 오브젝트가 High 그래픽입니다!");
    }

    private void OnGoalTriggered()
    {
        // 충돌 감지 제거
        foreach (var detectionRange in FindObjectsByType<DetectionRange>(FindObjectsSortMode.None))
        {
            Destroy(detectionRange.gameObject);  
        }
        
        // 로그 시스템
        ChangeStageSection("40_2_exit_entrance");
        GameManager.Instance.LogManager.LogStageExit(StageId, SectionId,"clear", Time.realtimeSinceStartup - StageStartTime);

        GameManager.Instance.AudioManager.FadeOutAudio(1.0f);
        GameManager.Instance.FadeManager.LoadNextScene();

        if (!AchievementStatusManager._isEnteranceAchievementUnlocked)
        {
            SteamAchievementManager.Instance.UnlockAchievement("ACH_STORY_GATE_CLEAR");
            AchievementStatusManager._isEnteranceAchievementUnlocked = true;
            Debug.Log("도전과제 '입구 클리어'가 완료되었습니다.");
        }
    }
}
