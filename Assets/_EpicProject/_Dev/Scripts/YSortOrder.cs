using UnityEngine;

public class YSortOrder : MonoBehaviour
{
    [Tooltip("숫자가 높을수록 같은 Y위치에 있을 때 더 앞에 보입니다.")]
    public int sortingOrderBias = 0;
    private SpriteRenderer[] _spriteRenderers;
    private int _yPos;
    public bool alwaysBehindPlayer = false;
    private SpriteRenderer _playerRenderer;

    // OnEnable, Awake, Start 함수는 이전과 동일하게 그대로 둡니다.
    private void OnEnable()
    {
        _yPos = int.MinValue;
    }

    private void Awake()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerRenderer = player.GetComponentInChildren<SpriteRenderer>();
        }
    }

    void LateUpdate()
    {
        if (_spriteRenderers.Length <= 0) return;

        // --- [ 여기가 핵심 수정사항 ] ---
        if (alwaysBehindPlayer && _playerRenderer != null)
        {
            // 플레이어의 Y위치를 가져와서 기본 순서를 계산합니다.
            // 이렇게 하면 도끼는 플레이어와 같은 Y레벨에 있는 것처럼 취급됩니다.
            int baseOrderByPlayerY = (int)(_playerRenderer.transform.position.y * 100f);

            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                // 플레이어의 Y위치 기반 순서에, 도끼 자신의 우선순위(Bias)를 더해줍니다.
                _spriteRenderers[i].sortingOrder = -baseOrderByPlayerY + sortingOrderBias;
            }
        }
        else
        {
            // '플레이어 뒤에 있기' 옵션이 꺼져 있다면, 원래의 Y-Sort 로직을 그대로 수행합니다.
            int newYPos = (int)(transform.position.y * 100f);
            if (_yPos == newYPos) return;

            _yPos = newYPos;
            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                _spriteRenderers[i].sortingOrder = -_yPos + sortingOrderBias;
            }
        }
        // --- [ 수정 끝 ] ---
    }
}