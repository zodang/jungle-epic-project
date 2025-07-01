using Define;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceDetectionGuard : MonoBehaviour
{
    private RespawnPointSquare _respawnPoint;
    
    private DetectionRange _range;
    private DialogueTrigger _dialogueTrigger;
    private WantedPoster _poster;
    
    private void Awake()
    {
        _range = GetComponentInChildren<DetectionRange>();
        _dialogueTrigger = GetComponent<DialogueTrigger>();
        _poster = FindAnyObjectByType<WantedPoster>();

        _respawnPoint = FindAnyObjectByType<RespawnPointSquare>();
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
        // player의 Graphic 상태와 Poster의 Graphic 상태 비교
        IGraphicChangeable player = playerObj.GetComponent<IGraphicChangeable>();
        IGraphicChangeable poster = _poster.GetComponent<IGraphicChangeable>();

        if (player == null || poster == null)
        {
            Debug.LogWarning("GraphicChangeable 없음!");
            return;
        }
        
        GraphicType playerType = player.GetCurrentValue();
        GraphicType posterType = poster.GetCurrentValue();
        
        if (DetectionDialogueTable.TryGetValue((posterType, playerType), out string dialogueId))
        {
            _dialogueTrigger.TriggerDialogue(dialogueId);
        }
        
        StartCoroutine(WaitCo(playerObj));
    }
    
    private IEnumerator WaitCo(GameObject player)
    {
        yield return new WaitForSeconds(0.1f);
        player.GetComponent<Movement2D>().Respawn(_respawnPoint.transform.position);
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
