using UnityEngine;

public class EntrancePatrolGuard : MonoBehaviour, IFeatureResetable, IControllable
{
    // IControllable
    private Rigidbody2D _rigidbody2D;
    private Movement2D _movement2D;
    private bool _enableMove;
    
    // IGraphicChangeable
    private GraphicHandler _graphicHandler;
    
    private PlayerAnimation _animation;
    
    private void Awake()
    {
        _graphicHandler = GetComponent<GraphicHandler>();
        
        _movement2D = GetComponent<Movement2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.gravityScale = 0;

        _animation = GetComponentInChildren<PlayerAnimation>();
        
        DisableControl();
        ResetFeature(); 
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
        _animation.ActivateAnimation(true);
    }

    public void DisableControl()
    {
        _enableMove = false;
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _movement2D.MoveDir = Vector3.zero;
        _animation.ActivateAnimation(false);
    }
    
    public void ResetFeature()
    {
        // Todo: 리셋 기능
    }

    #endregion FeatureSetting
}
