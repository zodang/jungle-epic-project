using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.Tilemaps.TilemapRenderer;

// 이 두 컴포넌트가 반드시 필요함을 명시합니다.
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(YSortOrder))] // Y-Sort 스크립트 이름이 다르다면 바꿔주세요.

public class ObjectPropertyController : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private YSortOrder _ySortOrder;

    // 스크립트가 처음 시작될 때 필요한 컴포넌트들을 미리 찾아둡니다.
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _ySortOrder = GetComponent<YSortOrder>();
    }

    // --- [새로 추가할 함수 1] ---
    /// <summary>
    /// 오브젝트가 물에 잠겼을 때 호출합니다.
    /// </summary>
    public void Submerge()
    {
        Debug.Log(gameObject.name + "가 물에 잠깁니다.");

        // 1. YSort 기능을 비활성화합니다.
        if (_ySortOrder != null) _ySortOrder.enabled = false;

        // 2. Sorting Layer를 물(Ground)보다 낮은 'Background'로 변경합니다.
        _spriteRenderer.sortingLayerName = "Background";

        // 3. Background 레이어의 다른 스프라이트(예: 배경 타일)보다 앞에 보이도록 
        //    Order in Layer를 적당히 높은 값으로 줍니다.
        _spriteRenderer.sortingOrder = 10;
    }

    // --- [새로 추가할 함수 2] ---
    /// <summary>
    /// 물이 빠져서 오브젝트가 드러났을 때 호출합니다.
    /// </summary>
    public void Emerge()
    {
        Debug.Log(gameObject.name + "가 물 밖으로 드러납니다.");

        // 1. Sorting Layer를 다시 'PlayerAndObjects'로 되돌립니다.
        _spriteRenderer.sortingLayerName = "PlayerAndObjects";

        // 2. YSort 기능을 다시 활성화해서 다른 오브젝트와 섞일 준비를 합니다.
        if (_ySortOrder != null) _ySortOrder.enabled = true;
    }


    /// <summary>
    /// 컨트롤러 블럭이 '붙었을 때' 호출할 함수
    /// </summary>
    public void AttachController()
    {
        Debug.Log(gameObject.name + "에 컨트롤러가 붙었습니다. ForePlayer 레이어로 변경합니다.");

        // 1. 스스로 순서를 정하는 YSort 스크립트를 잠시 끕니다.
        if (_ySortOrder != null)
        {
            _ySortOrder.enabled = false;
        }

        // --- [수정된 부분] ---
        // 컨트롤러가 붙으면 '플레이어 뒤에 있기' 옵션을 끕니다.
        _ySortOrder.alwaysBehindPlayer = false;

        // 2. 소팅 레이어를 'ForePlayer'로 바꿔서 무조건 앞에 보이게 합니다.
        _spriteRenderer.sortingLayerName = "ForePlayer";
        // Order in Layer는 0으로 초기화해도 좋습니다. ForePlayer 레이어 자체가 높기 때문입니다.
        _spriteRenderer.sortingOrder = 0;
    }

    /// <summary>
    /// 컨트롤러 블럭이 '떨어졌을 때' 호출할 함수
    /// </summary>
    public void DetachController()
    {
        Debug.Log(gameObject.name + "에서 컨트롤러가 떨어졌습니다. PlayerAndObjects 레이어로 복귀합니다.");

        // 1. 소팅 레이어를 원래의 'PlayerAndObjects'로 되돌립니다.
        _spriteRenderer.sortingLayerName = "PlayerAndObjects";

        // 2. 꺼뒀던 YSort 스크립트를 다시 켜서, 자신의 Y위치에 맞게 순서가 정해지도록 합니다.
        if (_ySortOrder != null)
        {
            _ySortOrder.enabled = true;
        }

        // --- [수정된 부분] ---
        // 컨트롤러를 떼면 '플레이어 뒤에 있기' 옵션을 켭니다.
        _ySortOrder.alwaysBehindPlayer = true;
    }

    // --- [테스트를 위한 임시 코드] ---
    // 이 부분은 테스트가 끝나면 지우거나 주석 처리해주세요.
    private void Update()
    {
        // 키보드 위쪽의 숫자 '1' 키를 눌렀을 때
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            // Submerge 함수를 호출해서 도끼를 물에 잠기게 합니다.
            Submerge();
        }

        // 키보드 위쪽의 숫자 '2' 키를 눌렀을 때
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            // Emerge 함수를 호출해서 도끼를 물 밖으로 꺼냅니다.
            Emerge();
        }
    }
}