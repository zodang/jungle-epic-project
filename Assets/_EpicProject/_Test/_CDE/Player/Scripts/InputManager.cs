using System;
using System.Linq;
using Define;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public event Action OnInteract;
    public event Action OnEscPressed;
    public event Action OnTabPressed;
    public event Action OnQPressed;
    
    public Vector2 MoveInput { get; private set; }

    private InputActionAsset _inputActionAsset;
    private InputActionMap _actionMap;
    
    private InputAction _moveAction;
    private InputAction _interactionAction;
    private InputAction _talkAction;
    private InputAction _clickAction;
    private InputAction _toggleSettingAction;
    private InputAction _toggleInventoryAction;
    private InputAction _glitchVisionAction;
    
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
        _toggleSettingAction = _actionMap.FindAction("ToggleSetting");
        _toggleInventoryAction = _actionMap.FindAction("ToggleInventory");
        _glitchVisionAction = _actionMap.FindAction("GlitchVision");
        

        _moveAction.performed += OnMovePerformed;
        _moveAction.canceled += OnMoveCanceled;
        _interactionAction.performed += OnInteractionPerformed;
        _clickAction.performed += OnClickPerformed;
        _toggleSettingAction.performed += OnToggleSettingPerformed;
        _toggleInventoryAction.performed += OnToggleInventoryPerformed;
        _glitchVisionAction.performed += OnGlitchVisioPerformed;

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
        if (Time.timeScale <= 0) return;
        
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
        if (Time.timeScale <= 0) return;
        
        OnInteract?.Invoke();
    }
    
    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        if (!_isPlayerInputActive) return;
        if (Time.timeScale <= 0) return;
        
        _isClicked = true;
    }
    
    private void OnToggleSettingPerformed(InputAction.CallbackContext context)
    {
        OnEscPressed?.Invoke();
    }

    private void OnToggleInventoryPerformed(InputAction.CallbackContext context)
    {
        if (!_isPlayerInputActive) return;
        if (Time.timeScale <= 0) return;
        
        OnTabPressed?.Invoke();
    }

    private void OnGlitchVisioPerformed(InputAction.CallbackContext context)
    {
        if (!_isPlayerInputActive) return;
        if (Time.timeScale <= 0) return;
        
        OnQPressed?.Invoke();
    }
    
    public void OnDestroy()
    {
        _actionMap.Disable();

        // InputAction 이벤트 연결 해제
        _moveAction.performed -= OnMovePerformed;
        _moveAction.canceled -= OnMoveCanceled;
        _interactionAction.performed -= OnInteractionPerformed;
        _clickAction.performed -= OnClickPerformed;

        OnInteract = null;
        OnEscPressed = null;
        OnTabPressed = null;
        OnQPressed = null;
        
        _inputActionAsset = null;
        _actionMap = null;
    }

    private bool IsPointOverPassUI()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        if (results.Count == 0) return false;

        // 가장 위에 있는(리스트 첫번째) UI가 tagName인지 확인
        var topUI = results[0].gameObject;
        return topUI.CompareTag("ClickPassUI");
    }

    private void Update()
    {
        if (_isClicked)
        {
            _isClicked = false;
            
            // UI 감지 시 Click Pass UI인지 검사 후 return
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (!IsPointOverPassUI())
                {
                    return;
                }
            }
            
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
            bool isMask = false;
            ClickableMask mask = null;
            ClickableMaskSortOrder clickableMaskSortOrder = ClickableMaskSortOrder.ForePlayer;
            
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
                    if (clickableMask.SortOrder == ClickableMaskSortOrder.ForePlayer)
                    {
                        break;
                    }
                    else
                    {
                        mask = clickableMask;
                        isMask = true;
                        clickableMaskSortOrder = clickableMask.SortOrder;
                        continue;
                    }
                }

                IClickable clickable = coll.GetComponentInParent<IClickable>();
                if (!clickable.IsUnityNull())
                {
                    if (isMask)
                    {
                        if (clickableMaskSortOrder == ClickableMaskSortOrder.PlayerAndObject)
                        {
                            if (coll.transform.position.y >= mask.transform.position.y)
                            {
                                if (!coll.TryGetComponent<ClickableYAnchor>(out ClickableYAnchor clickableYAnchor) ||
                                    clickableYAnchor.YAnchor.IsUnityNull() ||
                                    clickableYAnchor.YAnchor.transform.position.y >= mask.transform.position.y)
                                {
                                    continue;
                                }
                            }
                        }
                        else if (clickableMaskSortOrder == ClickableMaskSortOrder.MidGround)
                        {
                            if (!coll.GetComponent<ClickableUnterTag>().IsUnityNull())
                            {
                                continue;
                            }
                        }
                    }

                    clickable.OnClicked();
                    break;
                }
            }
        }

        // 테스트 커맨드
         StageTest();
         BlockTest();
    }

    private void StageTest()
    {
        bool condition1 = Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Tab);
        bool condition2 = Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKey(KeyCode.Tab);
        
        if (condition1 || condition2)
        {
            FindAnyObjectByType<TestModeUI>().ActivateStageCommandUI();
        }
    }

    private void BlockTest()
    {
        // 테스트용 코드
        if (Input.GetKeyDown(KeyCode.F1))
        {
            FindAnyObjectByType<Inventory>().SpawnBlock(BlockType.PlayerControl);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            FindAnyObjectByType<Inventory>().SpawnBlock(BlockType.Scale);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            FindAnyObjectByType<Inventory>().SpawnBlock(BlockType.Rotate);
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            FindAnyObjectByType<Inventory>().SpawnBlock(BlockType.Light);
        }
        
        if (Input.GetKeyDown(KeyCode.F5))
        {
            FindAnyObjectByType<Inventory>().SpawnBlock(BlockType.Graphic);
        }
        
        if (Input.GetKeyDown(KeyCode.F6))
        {
            FindAnyObjectByType<Inventory>().SpawnBlock(BlockType.Speed);
        }
    }
}
