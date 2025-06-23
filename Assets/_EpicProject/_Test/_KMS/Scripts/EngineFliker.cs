using UnityEngine;
using System.Collections.Generic;

public class EngineFliker : MonoBehaviour
{
    private List<Flik> _fliks;

    void Awake()
    {
        // 씬에 있는 모든 Flik 컴포넌트를 찾고,
        // 각 Flik 스크립트가 붙은 오브젝트를 비활성화
        _fliks = new List<Flik>(FindObjectsOfType<Flik>());
        foreach (var f in _fliks)
            f.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        DraggableBlock.OnAnyBlockBeginDrag += EnableFlikObjects;
        DraggableBlock.OnAnyBlockEndDrag += DisableFlikObjects;
    }

    void OnDisable()
    {
        DraggableBlock.OnAnyBlockBeginDrag -= EnableFlikObjects;
        DraggableBlock.OnAnyBlockEndDrag -= DisableFlikObjects;
    }

    private void EnableFlikObjects()
    {
        foreach (var f in _fliks)
            f.gameObject.SetActive(true);
    }

    private void DisableFlikObjects()
    {
        foreach (var f in _fliks)
            f.gameObject.SetActive(false);
    }
}
