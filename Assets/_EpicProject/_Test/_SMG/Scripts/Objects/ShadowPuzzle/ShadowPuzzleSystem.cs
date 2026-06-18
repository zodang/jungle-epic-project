using Define;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShadowPuzzleSystem : MonoBehaviour
{
    private List<TargetAlignmentChecker> _targetAlignmentCheckers;
    private Dictionary<TargetAlignmentChecker, int> _checkerIndexes;
    private bool[] _checkerReported;
    private bool[] _checkerAlignedStates;
    private int _reportedCheckerCount;
    private int _alignedCheckerCount;

    public bool IsAllAligned => _reportedCheckerCount == _targetAlignmentCheckers.Count && _alignedCheckerCount == _targetAlignmentCheckers.Count;
    private bool _prevAllAligned;
    private bool _hasAllAlignedState;

    public UnityEvent<bool> OnAlignmentChanged;

    private bool _isInitialAlignChanged = false;

    private void Awake()
    {
        _targetAlignmentCheckers = new List<TargetAlignmentChecker>();
        _checkerIndexes = new Dictionary<TargetAlignmentChecker, int>();
        TargetAlignmentChecker[] targetAlignmentCheckers = GetComponentsInChildren<TargetAlignmentChecker>();
        
        _checkerReported = new bool[targetAlignmentCheckers.Length];
        _checkerAlignedStates = new bool[targetAlignmentCheckers.Length];

        for(int i = 0; i < targetAlignmentCheckers.Length; i++)
        {
            TargetAlignmentChecker checker = targetAlignmentCheckers[i];

            _targetAlignmentCheckers.Add(checker);
            _checkerIndexes.Add(checker, i);
            
            if (checker.OnIsAligend == null)
            {
                checker.OnIsAligend = new UnityEvent<TargetAlignmentChecker, bool>();
            }
            checker.OnIsAligend.AddListener(HandleCheckerAlignmentChanged);
        }
    }

    private void Start()
    {
        TryNotifyAlignmentChanged();
    }

    private void OnDestroy()
    {
        if (_targetAlignmentCheckers == null) return;

        for (int i = 0; i < _targetAlignmentCheckers.Count; i++)
        {
            if (_targetAlignmentCheckers[i] != null && _targetAlignmentCheckers[i].OnIsAligend != null)
            {
                _targetAlignmentCheckers[i].OnIsAligend.RemoveListener(HandleCheckerAlignmentChanged);
            }
        }
    }

    private void HandleCheckerAlignmentChanged(TargetAlignmentChecker checker, bool isAligned)
    {
        if (!_checkerIndexes.TryGetValue(checker, out int checkerIndex)) return;

        if (!_checkerReported[checkerIndex])
        {
            _checkerReported[checkerIndex] = true;
            _reportedCheckerCount++;

            if (isAligned)
            {
                _alignedCheckerCount++;
            }
        }
        else if (_checkerAlignedStates[checkerIndex] != isAligned)
        {
            // 전체 정렬 여부는 개수로 비교
            _alignedCheckerCount += isAligned ? 1 : -1;
        }
        else
        {
            return;
        }

        _checkerAlignedStates[checkerIndex] = isAligned;
        TryNotifyAlignmentChanged();
    }

    private void TryNotifyAlignmentChanged()
    {
        if (!_hasAllAlignedState && _reportedCheckerCount < _targetAlignmentCheckers.Count) return;

        bool isAllAligned = IsAllAligned;
        if (!_hasAllAlignedState)
        {
            _hasAllAlignedState = true;
            _prevAllAligned = !isAllAligned;
        }

        if (_prevAllAligned == isAllAligned) return;

        _prevAllAligned = isAllAligned;
        Debug.Log("isAllAligned: " + isAllAligned);
        OnAlignmentChanged?.Invoke(isAllAligned);
            
        if (_isInitialAlignChanged)
        {
            // 문 효과음 재생
            GameManager.Instance.AudioManager.PlaySfx(SfxType.CaveButton);
        }
        else
        {
            // 초기 1회 효과음 무시
            _isInitialAlignChanged = true;
        }
    }
}
