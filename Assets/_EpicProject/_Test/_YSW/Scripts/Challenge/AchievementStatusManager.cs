// AchievementStatusManager.cs

using System.Collections.Generic;
using UnityEngine;

public static class AchievementStatusManager
{
    // 각 도전과제의 해금 여부를 저장하는 static 변수들
    public static bool _isStartAchievementUnlocked = false;
    public static bool _isCliffAchievementUnlocked = false;
    public static bool _isAllHighAchievementUnlocked = false;
    public static bool _isEnteranceAchievementUnlocked = false;
    public static bool _isMaxSpeedAchievementUnlocked = false;
    public static bool _isbaldHeadAchievementUnlocked = false;
    public static bool _isAntManAchievementUnlocked = false;
    public static bool _isPosterAchievementUnlocked = false;
    public static bool _isBowlingAchievementUnlocked = false;
    public static bool _isHappyEndingAchievementUnlocked = false;
    public static bool _isFairyAchievementUnlocked = false;
    public static bool _isSortingAchievementUnlocked = false;
    public static bool _isHandsomeAchievementUnlocked = false;
    public static bool _isTitanAchievementUnlocked = false;
    public static bool _isSweepAchievementUnlocked = false;
    public static bool _isTinkerbellAchievementUnlocked = false;
    public static bool _isBeautyGuardAchievementUnlocked = false;
    public static bool _isShadowPuzzleAchievementUnlocked = false; 


    public static int fallCount = 0; // 낙하 횟수 카운터
    public static bool isFallAchievementUnlocked = false; // '손이 미끄러졌네' 도전과제 해금 여부

    private const int TOTAL_NPC_COUNT = 11; // << 게임에 있는 전체 NPC 수를 여기에 입력하세요.
    public static List<string> talkedToNpcIds = new List<string>(); // 대화한 NPC ID 목록
    public static bool isChatterboxAchievementUnlocked = false; // '수다쟁이' 도전과제 해금 여부

    // ... 다른 도전과제 변수들도 여기에 추가 ...

    /// <summary>
    /// 게임을 새로 시작하거나 할 때 모든 상태를 리셋하는 함수
    /// </summary>
    public static void ResetAllAchievementFlags()
    {
        _isStartAchievementUnlocked = false;
        _isCliffAchievementUnlocked = false;
        _isAllHighAchievementUnlocked = false;
        _isEnteranceAchievementUnlocked = false;
        _isMaxSpeedAchievementUnlocked = false;
        _isbaldHeadAchievementUnlocked = false;
        _isAntManAchievementUnlocked = false;
        _isPosterAchievementUnlocked = false;
        _isBowlingAchievementUnlocked = false;
        _isHappyEndingAchievementUnlocked = false;
        _isFairyAchievementUnlocked = false;
        _isSortingAchievementUnlocked = false;
        _isHandsomeAchievementUnlocked = false;
        _isTitanAchievementUnlocked = false;
        _isSweepAchievementUnlocked = false;
        _isTinkerbellAchievementUnlocked = false;
        _isBeautyGuardAchievementUnlocked = false;
        _isShadowPuzzleAchievementUnlocked = false;

        // << 아래 두 줄을 새로 추가하세요. >>
        fallCount = 0;
        isFallAchievementUnlocked = false;
        // ...

        // << 아래 내용을 새로 추가하세요. >>
        talkedToNpcIds.Clear(); // NPC 대화 목록 비우기
        isChatterboxAchievementUnlocked = false;

        Debug.Log("All local achievement flags have been reset.");
    }


    /// <summary>
    /// NPC와 대화했을 때 호출되는 함수
    /// </summary>
    public static void RegisterNpcTalk(string npcId)
    {
        // 유효한 ID가 아니거나, 이미 도전과제가 해금되었으면 아무것도 하지 않음
        if (string.IsNullOrEmpty(npcId) || isChatterboxAchievementUnlocked)
        {
            return;
        }

        // 저장된 데이터 기준으로 중복 체크
        talkedToNpcIds = GameManager.Instance.SaveManager.LoadTalkedNpcData();
        if (talkedToNpcIds.Contains(npcId)) return;

        talkedToNpcIds.Add(npcId);
        GameManager.Instance.SaveManager.SaveTalkedNpcData(npcId);
        Debug.Log($"새로운 NPC와 대화: {npcId}. 현재 대화한 NPC 수: {talkedToNpcIds.Count}/{TOTAL_NPC_COUNT}");

        // 대화한 NPC 수가 목표치에 도달했는지 확인
        if (talkedToNpcIds.Count >= TOTAL_NPC_COUNT)
        {
            isChatterboxAchievementUnlocked = true;
            SteamAchievementManager.Instance.UnlockAchievement("ACH_SECRET_CHATTERBOX"); // '수다쟁이' API 이름
            Debug.Log("도전과제 '수다쟁이'가 완료되었습니다.");
        }
    }
}