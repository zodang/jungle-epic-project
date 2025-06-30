using UnityEngine;

public class WantedPoster : MonoBehaviour
{
    // IGraphicChangeable
    private GraphicHandler _graphicHandler;

    private void Awake()
    {
        _graphicHandler = GetComponent<GraphicHandler>();
        _graphicHandler.OnSetValue += ChangeGraphic;
    }

    private void ChangeGraphic(int index)
    {
        // 스프라이트 변경 외 추가 사항 있을 시 사용
    }
}
