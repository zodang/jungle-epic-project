using UnityEngine;

public class GuardBarricade : MonoBehaviour
{

    private DetectionRange _detectionRange;
    private BowlingPinHitHandler[] bowlingPinHitHandlers;

    bool isAllHit;

    private void Awake()
    {
        _detectionRange = GetComponentInChildren<DetectionRange>();
        bowlingPinHitHandlers = GetComponentsInChildren<BowlingPinHitHandler>();
    }


    // Update is called once per frame
    void Update()
    {
        if (isAllHit)
        {
            return;
        }
        
        for (int i = 0; i < bowlingPinHitHandlers.Length; i++)
        {
            if (!bowlingPinHitHandlers[i].IsHit) return;
        }
        isAllHit = true;
        _detectionRange.gameObject.SetActive(false);

        // 로그 시스템
        StageBaseManager.Instance.ChangeStageSection("51_1_pass_guard");

        if (!AchievementStatusManager._isBowlingAchievementUnlocked)
        {
            SteamAchievementManager.Instance.UnlockAchievement("ACH_PUZZLE_BOWLING");
            AchievementStatusManager._isBowlingAchievementUnlocked = true;
            Debug.Log("도전과제 '볼링 클리어'가 완료되었습니다.");
        }
        
    }
}
