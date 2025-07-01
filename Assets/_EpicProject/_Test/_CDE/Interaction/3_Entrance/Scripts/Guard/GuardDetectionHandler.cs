using Define;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum GuardType
{
    Patrol,
    Detection,
}

public class GuardDetectionHandler : MonoBehaviour
{
    [SerializeField] private GuardType guardType;
    [SerializeField] private Transform respawnPoint;
    private DetectionRange _range;
    private DialogueTrigger _dialogueTrigger;
    private WantedPoster _poster;

    private void Awake()
    {
        _range = GetComponentInChildren<DetectionRange>();
        _dialogueTrigger = GetComponent<DialogueTrigger>();
        _poster = FindAnyObjectByType<WantedPoster>();
    }

    private void Start()
    {
        _range.OnPlayerDetected += WhenPlayerDetected;
    }
    
    private void OnDestroy()
    {
        _range.OnPlayerDetected -= WhenPlayerDetected;
    }
    
    private void WhenPlayerDetected(GameObject playerObj)
    {
        switch (guardType)
        {
            case GuardType.Detection:
                HandleDetectionGuard(playerObj);
                break;
            case GuardType.Patrol:
                HandlePatrolGuard(playerObj);
                break;
        }
    }

    private void HandleDetectionGuard(GameObject playerObj)
    {
        IGraphicChangeable player = playerObj.GetComponent<IGraphicChangeable>();
        IGraphicChangeable posterGraphic = _poster.GetComponent<IGraphicChangeable>();
            
        if (player == null || posterGraphic == null)
        {
            Debug.LogWarning("GraphicChangeable 없음!");
            return;
        }
            
        GraphicType playerType = player.GetCurrentValue();
        GraphicType posterType = posterGraphic.GetCurrentValue();

        if (DetectionDialogueTable.TryGetValue((posterType, playerType), out string dialogueId))
        {
            _dialogueTrigger.TriggerDialogue(dialogueId);
        }

        bool condition1 = playerType == GraphicType.Middle && posterType == GraphicType.Low; 
        bool condition2 = playerType == GraphicType.High && posterType == GraphicType.Low;
        
        if (condition1 || condition2) return;
        if (respawnPoint == null) return;
        
        StartCoroutine(RespawnCo(playerObj));
    }

    private void HandlePatrolGuard(GameObject playerObj)
    {
        _dialogueTrigger.TriggerDialogue();
        
        if (respawnPoint == null) return;
        StartCoroutine(RespawnCo(playerObj));
    }
    
    private IEnumerator RespawnCo(GameObject player)
    {
        yield return new WaitForSeconds(0.1f);
        player.GetComponent<Movement2D>().Respawn(respawnPoint.position);
    }
    
    private static readonly Dictionary<(GraphicType poster, GraphicType player), string> DetectionDialogueTable = new()
    {
        { (GraphicType.Low, GraphicType.Low), "Fail_Low_GraphicPlayer" },
        { (GraphicType.Low, GraphicType.Middle), "Pass_Middle_GraphicPlayer" },
        { (GraphicType.Low, GraphicType.High), "Pass_High_GraphicPlayer" },
        { (GraphicType.Middle, GraphicType.Low), "Fail_Low_GraphicPlayer" },
        { (GraphicType.Middle, GraphicType.Middle), "Fail_Same_GraphicPlayer" },
        { (GraphicType.Middle, GraphicType.High), "Fail_Better_GraphicPlayer" },
        { (GraphicType.High, GraphicType.Low), "Fail_Low_GraphicPlayer" },
        { (GraphicType.High, GraphicType.Middle), "Fail_Worse_GraphicPlayer" },
        { (GraphicType.High, GraphicType.High), "Fail_Same_GraphicPlayer" }
    };
}
