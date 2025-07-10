using System.Collections.Generic;
using UnityEngine;

public class ShadowPuzzleSystem : MonoBehaviour
{
    private List<TargetAlignmentChecker> _targetAlignmentCheckers = new List<TargetAlignmentChecker>();

    private bool _isOk;
    private bool _prevIsOk;


    private void Awake()
    {
        TargetAlignmentChecker[] targetAlignmentCheckers = GetComponentsInChildren<TargetAlignmentChecker>();

        for(int i = 0; i < targetAlignmentCheckers.Length; i++)
        {
            _targetAlignmentCheckers.Add(targetAlignmentCheckers[i]);
        }
    }

    private void Update()
    {
        bool isAllCheck = true;
        for(int i = 0; i < _targetAlignmentCheckers.Count; i++)
        {
            if (!_targetAlignmentCheckers[i].IsAligend)
            {
                isAllCheck = false;
                break;
            }
        }

        if(isAllCheck != _prevIsOk)
        {

        }

    }


}
