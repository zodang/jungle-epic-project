using System;
using UnityEngine;

public class ClickableController : MonoBehaviour, IClickable
{
    // private BlockDataManager _blockData;
    private PopInspectorUI _popInspector;

    private Action OnObjectClicked;

    private void Awake()
    {
        // _blockData = GetComponent<BlockDataManager>();
        _popInspector = FindAnyObjectByType<PopInspectorUI>();

        OnObjectClicked += WhenClicked;
    }
    
    private void OnDestroy()
    {
        OnObjectClicked -= WhenClicked;
    }

    public void OnClicked()
    {
        OnObjectClicked?.Invoke();
    }

    private void WhenClicked()
    {
        // 팝업 띄우기
        _popInspector.Show(this);
    }
}
