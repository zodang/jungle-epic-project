using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class CaveDoor : MonoBehaviour
{
    [Header("Sprite")]
    [SerializeField] private SpriteRenderer door;

    private CaveSwitch _caveSwitch;

    [Header("DOTween")] 
    private Tween _doorTween;
    private readonly float _openHeight = 1f;
    private readonly float _closeHeight = 0f;
    private readonly float _duration = 0.5f;

    public UnityEvent<bool> OnChangeDoorState;

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
        
        _doorTween?.Kill();
        _doorTween = door.transform.DOLocalMoveY(endHeight, _duration).SetEase(Ease.OutQuad);

        OnChangeDoorState?.Invoke(!isOpen);
    }
}
