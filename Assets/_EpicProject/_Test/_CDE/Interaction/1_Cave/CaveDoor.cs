using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using Define;
using Unity.VisualScripting;

public class CaveDoor : MonoBehaviour
{
    [Tooltip("이 문이 그림자 퍼즐과 연결되어 도전과제를 해금하는 문인지 체크합니다.")]
    [SerializeField] private bool isShadowPuzzleDoor = false;

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

        // !isOpen이 true일 때가 문이 "열리는" 조건입니다.
        bool isDoorOpening = !isOpen;

        // 문이 열리고, 아직 도전과제가 해금되지 않았다면
        if (isShadowPuzzleDoor && isDoorOpening && !AchievementStatusManager._isShadowPuzzleAchievementUnlocked)
        {
            AchievementStatusManager._isShadowPuzzleAchievementUnlocked = true;
            // API 이름은 '빛과 그림자' 도전과제에 해당하는 "ACH_PUZZLE_SHADOW"를 사용했습니다.
            SteamAchievementManager.Instance.UnlockAchievement("ACH_PUZZLE_SHADOW");
            Debug.Log("도전과제 '빛과 그림자'가 완료되었습니다.");
        }

       

    }

    private void ChangeDoorStateInvert(bool isOpen)
    {
        ChangeDoorState(!isOpen);
    }
}
