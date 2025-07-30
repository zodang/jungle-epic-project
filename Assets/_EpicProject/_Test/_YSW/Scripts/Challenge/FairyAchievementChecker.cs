// FairyAchievementChecker.cs
using System.Collections.Generic;
using UnityEngine;

public class FairyAchievementChecker : MonoBehaviour
{
    [Tooltip("도전과제 조건을 확인할 요정들을 모두 여기에 등록하세요.")]
    [SerializeField] private List<CaveFairy> fairiesToCheck; // FairyState 대신 CaveFairy

    private void Start()
    {
        // CaveFairy의 상태 변경 이벤트를 구독
        CaveFairy.OnAnyFairyStateChanged += CheckFairyStates;
    }

    private void OnDestroy()
    {
        // 구독 해지
        CaveFairy.OnAnyFairyStateChanged -= CheckFairyStates;
    }

    private void CheckFairyStates()
    {
        if (AchievementStatusManager._isTinkerbellAchievementUnlocked) return;
        if (fairiesToCheck == null || fairiesToCheck.Count == 0) return;

        foreach (var fairy in fairiesToCheck)
        {
            // 각 요정의 IsSmallAndBright() 함수를 호출하여 상태 확인
            if (!fairy.IsSmallAndBright())
            {
                return; // 한 마리라도 조건 미달이면 즉시 종료
            }
        }

        UnlockTinkerbellAchievement();
    }

    private void UnlockTinkerbellAchievement()
    {
        AchievementStatusManager._isTinkerbellAchievementUnlocked = true;
        SteamAchievementManager.Instance.UnlockAchievement("ACH_SECRET_FAIRY"); // '팅커벨' API 이름
        Debug.Log("도전과제 '팅커벨'이 완료되었습니다: 모든 요정이 작고 빛납니다!");
    }
}