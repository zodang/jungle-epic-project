// MeetingZone.cs

using UnityEngine;

public class MeetingZone : MonoBehaviour
{
    // OnTriggerEnter2D는 Is Trigger가 체크된 콜라이더에 다른 콜라이더가 들어왔을 때 호출됩니다.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. 이미 도전과제가 해금되었다면 아무것도 하지 않음
        if (AchievementStatusManager._isBeautyGuardAchievementUnlocked)
        {
            return;
        }

        // 2. 들어온 오브젝트의 태그가 'GuardToMeet'인지 확인합니다.
        if (other.CompareTag("GuardToMeet"))
        {
            // 3. 조건이 맞으면 도전과제를 해금합니다.
            AchievementStatusManager._isBeautyGuardAchievementUnlocked = true;
            SteamAchievementManager.Instance.UnlockAchievement("ACH_SECRET_BEAUTY_GUARD");
            Debug.Log("도전과제 '미녀와 경비병'이 완료되었습니다.");

            // 도전과제가 해금되었으므로, 이 스크립트는 더 이상 필요 없습니다.
            // 선택 사항: 이 컴포넌트를 비활성화하여 더 이상 OnTriggerEnter2D가 호출되지 않도록 합니다.
            this.enabled = false;
        }
    }
}