using UnityEngine;
using System.Collections.Generic;

public class EngineFliker : MonoBehaviour
{
    private List<Flik> _fliks;

    private void Awake()
    {
        // 씬에 있는 모든 Flik 컴포넌트를 찾고,
        // 각 Flik 스크립트가 붙은 오브젝트를 비활성화
        _fliks = new List<Flik>(FindObjectsByType<Flik>(FindObjectsSortMode.None));
        foreach (var f in _fliks)
            f.gameObject.SetActive(false);
    }

    private void Start()
    {
        BlockVisual.OnAnyBlockBeginDrag += EnableFlikObjects;
        BlockVisual.OnAnyBlockEndDrag += DisableFlikObjects;
    }

    private void OnDestroy()
    {
        BlockVisual.OnAnyBlockBeginDrag -= EnableFlikObjects;
        BlockVisual.OnAnyBlockEndDrag -= DisableFlikObjects;
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
