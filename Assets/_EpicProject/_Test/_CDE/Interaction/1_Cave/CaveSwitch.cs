using System.Collections.Generic;
using UnityEngine;

public class CaveSwitch : MonoBehaviour
{
    [Header("Sprite")]
    [SerializeField] private SpriteRenderer upSwitch;
    private readonly HashSet<ScaleChecker> _upObjects = new();
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Switch 올려진 오브젝트 추가
        ScaleChecker scaleChecker = other.GetComponent<ScaleChecker>();
        if (scaleChecker != null)
            _upObjects.Add(scaleChecker);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Switch 올려진 오브젝트 제거
        ScaleChecker scaleChecker = other.GetComponent<ScaleChecker>();
        if (scaleChecker != null)
            _upObjects.Remove(scaleChecker);
    }
    
    private void Update()
    {
        bool isPressed = false;
        foreach (var upObj in _upObjects)
        {
            if (upObj != null && upObj.IsHeavyEnough)
            {
                isPressed = true;
                break;
            }
        }
        
        // Switch 상태 변경
        ChangeSwitchVisual(!isPressed);
    }

    private void ChangeSwitchVisual(bool isUp)
    {
        upSwitch.enabled = isUp;
    }
}
