using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    InputAction moveAction;
    InputAction attackAction;
    InputAction interactAction;
    InputAction jumpAction;
    InputAction sprintAction;

    float _speed = 5f;
    GameObject _interactObject;

    public LayerMask InteractLayer;


    private void Awake()
    {
    }

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        interactAction = InputSystem.actions.FindAction("Interact");
        //jumpAction = InputSystem.actions.FindAction("Jump");
        //sprintAction = InputSystem.actions.FindAction("Sprint");
    }

    // Update is called once per frame
    void Update()
    {
        OnMove(moveAction.ReadValue<Vector2>());

        // Check Interaction
        if (FindInteract(out _interactObject, 1.5f, InteractLayer))
        {
            //Debug.Log("FindInteract: " + _interactObject.name);
            // UI 표시
        }

        if(interactAction.WasPressedThisFrame())
        {
            OnInteractAction();
        }
    }

    
    // 상호작용 가능한 함수 찾기
    bool FindInteract(out GameObject interactObject, float radius, int layerMask)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
        
        List<Interact> list = new List<Interact>();
        for (int i = 0; i < colliders.Length; i++)
        {
            Interact obj;
            if (colliders[i].TryGetComponent<Interact>(out obj))
            {
                list.Add(obj);
            }
        }

        if (list.Count > 0)
        {
            if (list.Count > 1)
            {
                float minDist = Vector2.Distance(transform.position, list[0].transform.position);
                int minIdx = 0;
                for (int i = 1; i < list.Count; i++)
                {
                    float dist = Vector2.Distance(transform.position, list[i].transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        minIdx = i;
                    }
                }
                interactObject = list[minIdx].gameObject;
            }
            else
            {
                interactObject = list[0].gameObject;
            }
            return true;
        }
        else
        {
            interactObject = null;
            return false;
        }

    }

    void OnMove(Vector2 move)
    {
        //Debug.Log("OnMove: " + move);
        transform.Translate(move * _speed * Time.deltaTime);
    }

    void OnInteractAction()
    {
        if (_interactObject == null) return;
        Debug.Log("InteractAction: " + _interactObject.name);

        BlockBehaviour blockBehaviour;
        if(_interactObject.TryGetComponent<BlockBehaviour>(out blockBehaviour))
        {
            Debug.Log("Get Block: " + _interactObject.name);

            if(GetComponent<BlockInventory>().TryAddBlock(blockBehaviour))
            {
                _interactObject.SetActive(false);
                _interactObject = null;
            }
        }

        
    }
}
