using Define;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ShadowPuzzleSystem : MonoBehaviour
{
    private List<TargetAlignmentChecker> _targetAlignmentCheckers;

    private bool _init = false;

    public bool IsAllAligned => _init ? _targetAlignmentCheckers.All(x => x.IsAligned) : false;
    private bool _prevAllAligned;

    public UnityEvent<bool> OnAlignmentChanged;

    private bool _isInitialAlignChanged = false;

    private void Awake()
    {
        _init = false;
        _targetAlignmentCheckers = new List<TargetAlignmentChecker>();
        TargetAlignmentChecker[] targetAlignmentCheckers = GetComponentsInChildren<TargetAlignmentChecker>();

        for(int i = 0; i < targetAlignmentCheckers.Length; i++)
        {
            _targetAlignmentCheckers.Add(targetAlignmentCheckers[i]);
        }
        _init = true;
    }

    private void Start()
    {
        _prevAllAligned = !IsAllAligned;
    }


    private void Update()
    {
        bool isAllAligned = IsAllAligned;
        if(_prevAllAligned != isAllAligned)
        {
            _prevAllAligned = isAllAligned;
            Debug.Log("isAllAligned: " + isAllAligned);
            OnAlignmentChanged?.Invoke(isAllAligned);
            
            // 문 효과음 재생
            if (_isInitialAlignChanged)
            {
                GameManager.Instance.AudioManager.PlaySfx(SfxType.CaveButton);
            }
            else
            {
                // 초기 1회 효과음 무시
                _isInitialAlignChanged = true;
            }
        }
        
    }


}
