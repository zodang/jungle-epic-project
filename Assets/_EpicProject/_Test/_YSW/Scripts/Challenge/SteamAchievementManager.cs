using UnityEngine;
using Steamworks; // Steamworks.NET 네임스페이스 추가

public class SteamAchievementManager : MonoBehaviour
{
    // 이 스크립트를 어디서든 쉽게 접근할 수 있도록 싱글톤으로 만듭니다.
    public static SteamAchievementManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 지정된 API 이름의 도전과제를 해금합니다.
    /// </summary>
    /// <param name="achievementID">Steamworks에 설정한 도전과제의 API 이름</param>
    public void UnlockAchievement(string achievementID)
    {
        // 스팀이 실행 중이고, 정상적으로 초기화되었는지 확인합니다.
        if (!SteamManager.Initialized)
        {
            Debug.LogWarning("Steam is not initialized. Cannot unlock achievement.");
            return;
        }

        bool success = SteamUserStats.SetAchievement(achievementID);

        if (success)
        {
            Debug.Log("Achievement [" + achievementID + "] unlocked!");
            // 변경된 도전과제 상태를 스팀 서버에 즉시 저장하고 업로드합니다.
            // 이 함수를 호출하지 않으면, 게임이 종료될 때까지 상태가 저장되지 않을 수 있습니다.
            SteamUserStats.StoreStats();
        }
        else
        {
            Debug.LogWarning("Failed to unlock achievement [" + achievementID + "]. It might be already unlocked or the ID is incorrect.");
        }
    }
}
