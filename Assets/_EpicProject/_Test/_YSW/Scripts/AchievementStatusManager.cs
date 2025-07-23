// AchievementStatusManager.cs

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
        // ...

        Debug.Log("All local achievement flags have been reset.");
    }
}