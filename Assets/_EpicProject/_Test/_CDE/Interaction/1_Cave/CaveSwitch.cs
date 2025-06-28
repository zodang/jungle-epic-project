using System.Collections.Generic;
using UnityEngine;

public class CaveSwitch : MonoBehaviour
{
    [Header("Sprite")]
    [SerializeField] private SpriteRenderer upSwitch;
    
    private readonly HashSet<ScaleChecker> _onSwitchObjects = new();
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        ScaleChecker scaleChecker = other.GetComponent<ScaleChecker>();
        if (scaleChecker != null)
            _onSwitchObjects.Add(scaleChecker);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ScaleChecker scaleChecker = other.GetComponent<ScaleChecker>();
        if (scaleChecker != null)
            _onSwitchObjects.Remove(scaleChecker);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        ScaleChecker scaleChecker = other.GetComponent<ScaleChecker>();
        
        if (scaleChecker == null) return;
        Debug.Log(scaleChecker.IsHeavyEnough);

        ChangeSwitchVisual(scaleChecker.IsHeavyEnough);
    }
    
    private void Update()
    {
        bool anyHeavy = false;
        foreach (var checker in _onSwitchObjects)
        {
            if (checker != null && checker.IsHeavyEnough)
            {
                anyHeavy = true;
                break;
            }
        }
        
        ChangeSwitchVisual(!anyHeavy);
    }

    private void ChangeSwitchVisual(bool isUp)
    {
        upSwitch.enabled = isUp;
    }
}
