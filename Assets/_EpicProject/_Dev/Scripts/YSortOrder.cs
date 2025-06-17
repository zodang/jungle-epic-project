using UnityEngine;

public class YSortOrder : MonoBehaviour
{
    private SpriteRenderer[] _spriteRenderers;
    private int _yPos;

    // --- [새로 추가된 부분 1] ---
    [Tooltip("이 옵션을 켜면 Y축과 상관없이 항상 플레이어 뒤에 정렬됩니다.")]
    public bool alwaysBehindPlayer = false;

    // 플레이어의 SpriteRenderer를 저장할 변수
    private SpriteRenderer _playerRenderer;
    // --- [추가 끝] ---


    private void Awake()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void Start()
    {
        // --- [새로 추가된 부분 2] ---
        // 'Player' 태그를 가진 오브젝트를 찾아서 렌더러를 미리 저장해둡니다.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // 플레이어의 스프라이트가 자식 오브젝트에 있을 수 있으므로 GetComponentInChildren 사용
            _playerRenderer = player.GetComponentInChildren<SpriteRenderer>();
        }
        else
        {
            Debug.LogWarning("YSortOrder: 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }
        // --- [추가 끝] ---

        // 기존의 Start 로직은 Update에서 처리되므로 생략하거나, 초기화 코드를 한번 실행할 수 있습니다.
        // 여기서는 Update에서 모두 처리하도록 코드를 옮겼습니다.
    }

    // 캐릭터 움직임 계산이 모두 끝난 후에 실행되는 LateUpdate를 사용하는 것이
    // 미세한 떨림(Jitter) 현상을 방지하는 데 더 좋습니다.
    void LateUpdate()
    {
        if (_spriteRenderers.Length <= 0) return;

        // --- [수정된 핵심 로직] ---
        if (alwaysBehindPlayer && _playerRenderer != null)
        {
            // '플레이어 뒤에 있기' 옵션이 켜져 있다면,
            // 플레이어의 현재 순서를 가져와서 모든 자식 스프라이트들의 순서를 그보다 1 낮게 설정합니다.
            int playerSortingOrder = _playerRenderer.sortingOrder;
            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                // 플레이어의 순서가 계속 바뀔 수 있으므로, 매 프레임 업데이트합니다.
                _spriteRenderers[i].sortingOrder = playerSortingOrder - 1;
            }
        }
        else
        {
            // '플레이어 뒤에 있기' 옵션이 꺼져 있다면, 원래 하시던 Y-Sort 로직을 그대로 수행합니다.
            int newYPos = (int)(transform.position.y * 100f);
            if (_yPos == newYPos) return; // Y위치가 그대로면 업데이트 안함 (기존 최적화 유지)

            _yPos = newYPos;
            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                _spriteRenderers[i].sortingOrder = -_yPos;
            }
        }
        // --- [수정 끝] ---
    }
}