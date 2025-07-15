using System.Collections;
using UnityEngine;

public class DetectionGuard : MonoBehaviour
{
    private Animator _animator;
    private GuardDetectionHandler _detectionHandler;

    private void Awake()
    {
        _animator = transform.GetComponentInChildren<Animator>();
        _detectionHandler = GetComponent<GuardDetectionHandler>();
    }

    private void Start()
    {
        _animator.enabled = false;
        _detectionHandler.OnPlayerPass += EndDetect;
    }

    private void EndDetect()
    {
        StartCoroutine(EndDetectCo());
    }

    private IEnumerator EndDetectCo()
    {
        yield return new WaitForSeconds(0.1f);
        _animator.enabled = true;
        
        // 로그 시스템
        StageBaseManager.Instance.ChangeStageSection("pass_guard");
    }
}
