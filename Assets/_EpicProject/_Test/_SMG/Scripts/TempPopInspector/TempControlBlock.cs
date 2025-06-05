using UnityEngine;
using SMG;
using Unity.VisualScripting;


public class TempControlBlock : MonoBehaviour
{
    [SerializeField, ReadOnly] private bool _hasIControllable;
    public bool CanControl;
    private IControllable _controllable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _hasIControllable = TryGetComponent<IControllable>(out _controllable);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_controllable.IsUnityNull())
        {
            if (CanControl) _controllable.EnableControl();
            else _controllable.DisableControl();
        }
    }
}
