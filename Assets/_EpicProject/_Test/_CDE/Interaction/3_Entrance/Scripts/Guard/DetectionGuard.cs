using System.Collections;
using UnityEngine;

public class DetectionGuard : MonoBehaviour
{
    [SerializeField] private RuntimeAnimatorController RightIdleController; 
    [SerializeField] private AnimatorOverrideController IdleController; 
    
    private GuardDetectionHandler _detectionHandler;
    private Animator _animator;

    private void Awake()
    {
        _animator = transform.GetComponentInChildren<Animator>();
        _detectionHandler = GetComponent<GuardDetectionHandler>();
    }

    private void Start()
    {
        _animator.enabled = true;
        _animator.runtimeAnimatorController = RightIdleController;
        _detectionHandler.OnPlayerPass += EndDetect;
    }

    private void EndDetect()
    {
        StartCoroutine(EndDetectCo());
    }

    private IEnumerator EndDetectCo()
    {
        yield return new WaitForSeconds(0.1f);
        _animator.runtimeAnimatorController = IdleController;
        
        // 로그 시스템
        StageBaseManager.Instance.ChangeStageSection("40_1_pass_guard");
    }
}
