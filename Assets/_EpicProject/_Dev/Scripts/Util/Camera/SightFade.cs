using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SightFade : MonoBehaviour
{
    private enum FadeTargetType
    {
        SpriteType,
        TilemapType,
    }
    [SerializeField] private FadeTargetType fadeTargetType;
    
    private List<Transform> _insider = new List<Transform>();
    private CinemachineTargetGroup _cinemachineTargetGroup;
    private bool _isHide;
    
    private SpriteRenderer _spriteRenderer;
    private Tilemap _tilemap;

    private Coroutine _fadeCo;

    private ClickableMask _clickableMask;
    private bool _isClickableMask;

    private void Awake()
    {
        _isClickableMask = TryGetComponent<ClickableMask>(out _clickableMask);

        _cinemachineTargetGroup = FindAnyObjectByType<CinemachineTargetGroup>();

        switch (fadeTargetType)
        {
            case FadeTargetType.SpriteType:
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
                break;
            case FadeTargetType.TilemapType:
                _tilemap = GetComponentInChildren<Tilemap>();
                break;
        }

        _insider.Clear();
        _isHide = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.layer == LayerMask.NameToLayer("Clickable")) return;

        if (!collision.TryGetComponent<FootTag>(out FootTag footTag)) return;
        
        Transform target = collision.GetComponentInParent<Rigidbody2D>().transform;
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
        //if (collision.gameObject.layer == LayerMask.NameToLayer("Clickable")) return;

        if (!collision.TryGetComponent<FootTag>(out FootTag footTag)) return;

        Transform target = collision.GetComponentInParent<Rigidbody2D>().transform;
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

        if (_isClickableMask) gameObject.layer = (_isHide) ? 0 : Define.Layers.Clickable;

        // 실행중인 Fade 코루틴 있으면 중단 후 실행
        if (_fadeCo != null) StopCoroutine(_fadeCo);
        
        StartCoroutine(fadeTargetType == FadeTargetType.SpriteType
            ? SetSpriteFadeCoroutine(_isHide)
            : SetTilemapFadeCoroutine(_isHide));
    }

    IEnumerator SetSpriteFadeCoroutine(bool isHide)
    {
        if (_spriteRenderer == null)
        {
            Debug.Log("Sprite가 없습니다.");
            yield break;
        }
        
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

    IEnumerator SetTilemapFadeCoroutine(bool isHide)
    {
        if (_tilemap == null)
        {
            Debug.Log("Tilemap이 없습니다.");
            yield break;
        }
        
        Color color = _tilemap.color;
        for (int i = 1; i <= 4; i++)
        {
            yield return new WaitForSeconds(0.05f);
            color.a = i * 0.25f;
            if(_isHide)
            {
                color.a = 1f - color.a;
            }
            _tilemap.color = color;
        }
    }
}
