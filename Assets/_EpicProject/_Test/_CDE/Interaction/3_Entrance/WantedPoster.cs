using System.Collections.Generic;
using UnityEngine;

public class WantedPoster : MonoBehaviour
{
    // IGraphicChangeable
    private GraphicHandler _graphicHandler;
    [SerializeField] private List<Sprite> graphicList;
    
    private void Awake()
    {
        ComponentHelper.TryGetOrAddComponent<GraphicHandler>(ref _graphicHandler, gameObject);
        _graphicHandler.Init(graphicList);
    }
}
