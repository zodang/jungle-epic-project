using UnityEngine;
using DG.Tweening;

public class CaveDoor : MonoBehaviour
{
    [Header("Sprite")]
    [SerializeField] private SpriteRenderer door;

    private CaveSwitch _caveSwitch;

    [Header("DOTween")] 
    private readonly float _openHeight = 1f;
    private readonly float _closeHeight = 0f;
    private readonly float _duration = 0.5f;

    private void Awake()
    {
        _caveSwitch = FindAnyObjectByType<CaveSwitch>();
    }

    private void Start()
    {
        _caveSwitch.OnSwitchPressed += ChangeDoorState;
    }

    private void ChangeDoorState(bool isOpen)
    {
        // 문 여닫음 효과
        float endHeight = !isOpen ? _openHeight : _closeHeight;
        door.transform.DOLocalMoveY(endHeight, _duration).SetEase(Ease.OutQuad);
    }
}
