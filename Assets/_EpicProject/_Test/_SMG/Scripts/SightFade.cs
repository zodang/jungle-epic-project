using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class SightFade : MonoBehaviour
{
    private List<Transform> _insider = new List<Transform>();
    private CinemachineTargetGroup _cinemachineTargetGroup;
    private SpriteRenderer _spriteRenderer;
    private bool _isHide;

    private void Awake()
    {
        _cinemachineTargetGroup = FindAnyObjectByType<CinemachineTargetGroup>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _insider.Clear();
        _isHide = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Clickable")) return;
        
        Transform target = collision.transform.root;
        if (_cinemachineTargetGroup.FindMember(target) >= 0)
        {
            if(!_insider.Contains(target))
            {
                _insider.Add(target);
                if (_insider.Count < 2)
                    SetFade(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Clickable")) return;

        Transform target = collision.transform.root;
        if (_insider.Contains(target))
        {
            _insider.Remove(target);
            if (_insider.Count < 1)
                SetFade(false);
        }

    }

    void SetFade(bool isHide)
    {
        if (_isHide == isHide) return;
        _isHide = isHide;
        StartCoroutine(SetFadeCoroutine(_isHide));
    }

    IEnumerator SetFadeCoroutine(bool isHide)
    {
        yield return null;
        Color color = _spriteRenderer.color;
        for (int i = 1; i <= 4; i++)
        {
            yield return new WaitForSeconds(0.05f);
            color.a = i * 0.25f;
            if(_isHide)
            {
                color.a = 1f - color.a;
            }
            _spriteRenderer.color = color;
        }
    }
}
