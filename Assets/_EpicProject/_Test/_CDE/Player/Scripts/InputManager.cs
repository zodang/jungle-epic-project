using System;
using System.Linq;
using Define;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public event Action OnInteract;
    public event Action OnEscPressed;
    public event Action OnTabPressed;
    
    public Vector2 MoveInput { get; private set; }

    private InputActionAsset _inputActionAsset;
    private InputActionMap _actionMap;
    
    private InputAction _moveAction;
    private InputAction _interactionAction;
    private InputAction _talkAction;
    private InputAction _clickAction;
    private InputAction _OffEngineAction;
    private InputAction _toggleInventoryAction;

    private bool _isClicked;
    
    private bool _isPlayerInputActive = true;

    public void Awake()
    {
        var originalInputActionAsset = Resources.Load<InputActionAsset>("InputAction");
        _inputActionAsset = Instantiate(originalInputActionAsset);
        
        _actionMap = _inputActionAsset.FindActionMap("Player");

        _moveAction = _actionMap.FindAction("Move");
        _interactionAction = _actionMap.FindAction("Interact");
        _talkAction = _actionMap.FindAction("Talk");
        _clickAction = _actionMap.FindAction("Click");
        _OffEngineAction = _actionMap.FindAction("OffEngine");
        _toggleInventoryAction = _actionMap.FindAction("ToggleInventory");

        _moveAction.performed += OnMovePerformed;
        _moveAction.canceled += OnMoveCanceled;
        _interactionAction.performed += OnInteractionPerformed;
        _clickAction.performed += OnCLickPerformed;
        _OffEngineAction.performed += OnOffEnginePerformed;
        _toggleInventoryAction.performed += OnToggleInventoryPerformed;

        _actionMap.Enable();
    }
    
    public void ActivatePlayerInput(bool isActive)
    {
        // 플레이어 입력 활성화
        _isPlayerInputActive = isActive;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        if (!_isPlayerInputActive) return;
        
        var input = context.ReadValue<Vector2>();
        MoveInput = input;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        MoveInput = Vector2.zero;
    }

    private void OnInteractionPerformed(InputAction.CallbackContext context)
    {
        if (!_isPlayerInputActive) return;
        
        OnInteract?.Invoke();
    }
    
    private void OnCLickPerformed(InputAction.CallbackContext context)
    {
        if (!_isPlayerInputActive) return;
        _isClicked = true;
    }
    
    private void OnOffEnginePerformed(InputAction.CallbackContext context)
    {
        if (!_isPlayerInputActive) return;
        OnEscPressed?.Invoke();
    }

    private void OnToggleInventoryPerformed(InputAction.CallbackContext context)
    {
        if (!_isPlayerInputActive) return;
        OnTabPressed?.Invoke();
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
        OnEscPressed = null;
        OnTabPressed = null;
        
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

            // RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, float.PositiveInfinity, LayerMask.GetMask("Clickable"));
            RaycastHit2D[] hits = Physics2D.RaycastAll(
                    worldPos,
                    Vector2.zero,
                    float.PositiveInfinity,
                    LayerMask.GetMask("Clickable"))
                .OrderBy(h => h.collider.transform.position.z)
                .ToArray();

            bool isMaskBypass = false;

            for (int i = 0; i < hits.Length; i++)
            {
                Collider2D coll = hits[i].collider;
                
                ClickableMask clickableMask = coll.GetComponent<ClickableMask>();
                ClickableMaskBypass clickableMaskBypass = coll.GetComponent<ClickableMaskBypass>();
                if (!clickableMaskBypass.IsUnityNull())
                {
                    isMaskBypass = true;
                    continue;
                }
                else if (!clickableMask.IsUnityNull() && !isMaskBypass)
                {
                    break;
                }

                IClickable clickable = coll.GetComponentInParent<IClickable>();
                if (!clickable.IsUnityNull())
                {
                    clickable.OnClicked();
                    break;
                }
            }
            //RaycastHit2D hit = hits.OrderBy(h => h.transform.position.z).FirstOrDefault();

            //var clickable = hit.collider != null
            //    ? hit.collider.GetComponentInParent<IClickable>()
            //    : null;

            //if (clickable != null)
            //{
            //    // Clickable 오브젝트 클릭 시 작동
            //    clickable.OnClicked();
            //}
        }
        
        // BlockTest();
    }

    private void BlockTest()
    {
        // 테스트용 코드
        if (Input.GetKeyDown(KeyCode.F1))
        {
            FindAnyObjectByType<Inventory>().Collect(BlockType.PlayerControl);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            FindAnyObjectByType<Inventory>().Collect(BlockType.Scale);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            FindAnyObjectByType<Inventory>().Collect(BlockType.Rotate);
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            FindAnyObjectByType<Inventory>().Collect(BlockType.Light);
        }
        
        if (Input.GetKeyDown(KeyCode.F5))
        {
            FindAnyObjectByType<Inventory>().Collect(BlockType.Graphic);
        }
        
        if (Input.GetKeyDown(KeyCode.F6))
        {
            FindAnyObjectByType<Inventory>().Collect(BlockType.Speed);
        }
    }
}
