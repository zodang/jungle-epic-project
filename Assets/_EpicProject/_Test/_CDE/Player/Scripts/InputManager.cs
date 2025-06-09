using System;
using Define;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    public event Action OnInteract;
    public Vector2 MoveInput { get; private set; }

    private InputActionAsset _inputActionAsset;
    private InputActionMap _actionMap;
    
    private InputAction _moveAction;
    private InputAction _interactionAction;
    private InputAction _talkAction;
    private InputAction _clickAction;
    
    private bool _isClicked;

    public override void Awake()
    {
        base.Awake();
        
        var originalInputActionAsset = Resources.Load<InputActionAsset>("InputAction");
        _inputActionAsset = Instantiate(originalInputActionAsset);
        
        _actionMap = _inputActionAsset.FindActionMap("Player");

        _moveAction = _actionMap.FindAction("Move");
        _interactionAction = _actionMap.FindAction("Interact");
        _talkAction = _actionMap.FindAction("Talk");
        _clickAction = _actionMap.FindAction("Click");

        _moveAction.performed += OnMovePerformed;
        _moveAction.canceled += OnMoveCanceled;
        _interactionAction.performed += OnInteractionPerformed;
        _clickAction.performed += OnCLickPerformed;

        _actionMap.Enable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        if (IsInputFieldFocused())  return;
        
        var input = context.ReadValue<Vector2>();
        MoveInput = input;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        MoveInput = Vector2.zero;
    }

    private void OnInteractionPerformed(InputAction.CallbackContext context)
    {
        if (IsInputFieldFocused()) return;
        
        OnInteract?.Invoke();
    }
    
    private void OnCLickPerformed(InputAction.CallbackContext context)
    {
        _isClicked = true;
    }

    private bool IsInputFieldFocused()
    {
        // InputField 입력 중 여부 반환
        GameObject selectedObj = EventSystem.current.currentSelectedGameObject;
        if (selectedObj == null) return false;
        
        return selectedObj.GetComponent<TMP_InputField>() != null;
    }
    
    public void OnDestroy()
    {
        _actionMap.Disable();

        // InputAction 이벤트 연결 해제
        _moveAction.performed -= OnMovePerformed;
        _moveAction.canceled -= OnMoveCanceled;
        _interactionAction.performed -= OnInteractionPerformed;
        _clickAction.performed -= OnCLickPerformed;

        OnInteract = null;
        _inputActionAsset = null;
        _actionMap = null;
    }

    private void Update()
    {
        if (_isClicked)
        {
            _isClicked = false;
            
            // UI 감지 시 return
            if (EventSystem.current.IsPointerOverGameObject()) return;
            
            Vector2 screenPos = Mouse.current.position.ReadValue();
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, float.PositiveInfinity, LayerMask.GetMask("Clickable"));
            var clickable = hit.collider != null
                ? hit.collider.GetComponentInParent<IClickable>()
                : null;

            if (clickable != null)
            {
                // Clickable 오브젝트 클릭 시 작동
                clickable.OnClicked();
            }
        }
        
        
        // 테스트용 코드
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            FindAnyObjectByType<Inventory>().Collect(BlockType.PlayerControl);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            FindAnyObjectByType<Inventory>().Collect(BlockType.Scale);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            FindAnyObjectByType<Inventory>().Collect(BlockType.Rotate);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            FindAnyObjectByType<Inventory>().Collect(BlockType.Light);
        }
    }
}
