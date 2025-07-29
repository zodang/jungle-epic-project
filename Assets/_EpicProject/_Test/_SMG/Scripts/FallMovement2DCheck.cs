using Unity.VisualScripting;
using UnityEngine;

public class FallMovement2DCheck : MonoBehaviour
{
    public Transform ReaspwanPoint;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Movement2D movement2D;
        movement2D = collision.GetComponentInParent<Movement2D>();
        if (!movement2D.IsUnityNull())
        {
            movement2D.Respawn(ReaspwanPoint.position);

            // --- 아래 도전과제 로직을 추가하세요 ---

            // 1. 아직 도전과제가 해금되지 않았을 때만 카운트를 셉니다.
            if (!AchievementStatusManager.isFallAchievementUnlocked)
            {
                // 2. 중앙 관리자의 카운터를 1 증가시킵니다.
                AchievementStatusManager.fallCount++;
                Debug.Log($"낙하 감지! 현재 카운트: {AchievementStatusManager.fallCount}");

                // 3. 카운트가 5 이상이 되면 도전과제를 해금합니다.
                if (AchievementStatusManager.fallCount >= 5)
                {
                    AchievementStatusManager.isFallAchievementUnlocked = true;
                    SteamAchievementManager.Instance.UnlockAchievement("ACH_SECRET_FALL");
                    Debug.Log("도전과제 '손이 미끄러졌네'가 완료되었습니다.");
                }
            }
        }
        //if(collision.transform.root.TryGetComponent<Movement2D>(out Movement2D movement))
        //{
        //    Debug.Log(name + ".OnTriggerEnter2D: " + collision.name);
        //    movement.Reaspawn(ReaspwanPoint.position);
        //}
        
    }
}
