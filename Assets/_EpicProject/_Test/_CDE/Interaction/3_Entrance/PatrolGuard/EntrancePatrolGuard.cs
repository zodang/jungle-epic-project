using Define;
using System;
using System.Collections;
using UnityEngine;

public class EntrancePatrolGuard : MonoBehaviour, IFeatureResetable, IControllable
{
    // IControllable
    private Rigidbody2D _rigidbody2D;
    private Movement2D _movement2D;
    private bool _enableMove;
    
    // ISpeedChangeable
    private SpeedHandler _speedHandler;
    private readonly int _defaultSpeedStep = 2;
    
    // IGraphicChangeable
    private GraphicHandler _graphicHandler;
    private readonly GraphicType _defaultGraphicType = GraphicType.Middle;
    
    private PlayerAnimation _animation;
    
    public Action OnControlEnabled;
    public Action OnControlDisabled;
    
    private RespawnPointEntrance _respawnPoint;
    private DetectionRange _range;
    private DialogueTrigger _dialogueTrigger;
    
    private void Awake()
    {
        ComponentHelper.TryGetOrAddComponent<SpeedHandler>(ref _speedHandler, gameObject);
        _graphicHandler = GetComponent<GraphicHandler>();
        
        _speedHandler.Init(1);
        _speedHandler.OnSetValue += ChangeSpeed;
        
        _graphicHandler.Init(_defaultGraphicType);
        
        _movement2D = GetComponent<Movement2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.gravityScale = 0;
        
        _animation = GetComponentInChildren<PlayerAnimation>();

        _respawnPoint = FindAnyObjectByType<RespawnPointEntrance>();
        _range = GetComponentInChildren<DetectionRange>();
        _dialogueTrigger = GetComponent<DialogueTrigger>();
    }

    private void Start()
    {
        _range.OnPlayerDetected += WhenPlayerDetected;
        
        DisableControl();
        ResetFeature();
        
        _animation.ActivateAnimation(true);
    }

    public void SetMoveDirection(Vector2 dir)
    {
        _movement2D.MoveDir = dir;
    }

    private void WhenPlayerDetected(GameObject playerObj)
    {
        _dialogueTrigger.TriggerDialogue();
        StartCoroutine(WaitCo(playerObj));
    }
    
    private IEnumerator WaitCo(GameObject player)
    {
        yield return new WaitForSeconds(0.1f);
        player.GetComponent<Movement2D>().Respawn(_respawnPoint.transform.position);
    }

    #region FeatureSetting

    private void Update()
    {
        if (!_enableMove) return;

        _movement2D.MoveDir = StageManager.Instance.InputManager.MoveInput;
        _graphicHandler?.SetSpriteDirection(StageManager.Instance.InputManager.MoveInput);
    }

    public void EnableControl()
    {
        _enableMove = true;
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _movement2D.MoveDir = Vector3.zero;
        
        OnControlEnabled?.Invoke();
    }

    public void DisableControl()
    {
        _enableMove = false;
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _movement2D.MoveDir = Vector3.zero;
        
        OnControlDisabled?.Invoke();
    }

    private void ChangeSpeed(int step)
    {
        float multiple = 0.5f + 0.5f * step;
        _movement2D.MultiplySpeed(multiple);
    }
    
    public void ResetFeature()
    {
        _speedHandler.SetValue(_defaultSpeedStep);
        _graphicHandler.SetValue(_defaultGraphicType);
    }

    #endregion FeatureSetting
}
