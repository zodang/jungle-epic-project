using Unity.VisualScripting;
using UnityEngine;

public class ScaleChecker : MonoBehaviour
{
    [SerializeField] private float scaleThreshold = 1.5f;
    private float _currentScale;
    public bool IsHeavyEnough => _currentScale >= scaleThreshold;

    private void Start()
    {
        GetComponent<ScaleHandler>().OnSetValue += CheckHeavyEnough;
        _currentScale = GetComponent<ScaleHandler>().CurrentScale;
    }

    private void OnEnable()
    {
        
    }

    private void OnDestroy()
    {
        GetComponent<ScaleHandler>().OnSetValue -= CheckHeavyEnough;
    }

    private void CheckHeavyEnough(float scale)
    {
        _currentScale = scale;
    }
}
