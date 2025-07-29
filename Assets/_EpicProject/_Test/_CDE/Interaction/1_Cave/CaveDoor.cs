using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using Define;
using Unity.VisualScripting;

public class CaveDoor : MonoBehaviour
{
    [Header("Sprite")]
    [SerializeField] private SpriteRenderer door;

    private CaveSwitch _caveSwitch;
    private ShadowPuzzleSystem _shadowPuzzleSystem;

    [Header("DOTween")] 
    private Tween _doorTween;
    private readonly float _openHeight = 1f;
    private readonly float _closeHeight = 0f;
    private readonly float _duration = 0.5f;

    public UnityEvent<bool> OnChangeDoorState;

    private void Awake()
    {
        _caveSwitch = FindAnyObjectByType<CaveSwitch>();
        _shadowPuzzleSystem = FindAnyObjectByType<ShadowPuzzleSystem>();
    }

    private void Start()
    {
        if (!_caveSwitch.IsUnityNull())
        {
            _caveSwitch.OnSwitchPressed += ChangeDoorState;
        }
        if(!_shadowPuzzleSystem.IsUnityNull())
        {
            _shadowPuzzleSystem.OnAlignmentChanged.AddListener(ChangeDoorStateInvert);
        }
    }

    private void ChangeDoorState(bool isOpen)
    {
        // 문 여닫음 효과
        float endHeight = !isOpen ? _openHeight : _closeHeight;
        
        _doorTween?.Kill();
        _doorTween = door.transform.DOLocalMoveY(endHeight, _duration).SetEase(Ease.OutQuad);

        OnChangeDoorState?.Invoke(!isOpen);
        
    }

    private void ChangeDoorStateInvert(bool isOpen)
    {
        ChangeDoorState(!isOpen);
    }
}
