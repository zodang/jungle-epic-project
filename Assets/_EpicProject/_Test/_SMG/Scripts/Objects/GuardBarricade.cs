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
        StageBaseManager.Instance.ChangeStageSection("pass_guard");
    }
}
