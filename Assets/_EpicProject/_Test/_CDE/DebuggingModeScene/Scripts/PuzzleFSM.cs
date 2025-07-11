using Define;
using UnityEngine;

public class PuzzleFSM : MonoBehaviour
{
    private FSM<FSMState> _fsm;
    private DebugSlot _debugSlot;
    private SpeechAnchor _speechAnchor;
    private ConsoleBox _consoleBox;
    
    private BlockType[] _answerBlockTypes = new[] { BlockType.Light, BlockType.Scale, BlockType.Speed };
    private int _currentStep;
    
    private void Awake()
    {
        _fsm = new FSM<FSMState>();
    }
    
    public void StartPuzzle()
    {
        _debugSlot = FindAnyObjectByType<DebugSlot>();
        _speechAnchor = FindAnyObjectByType<SpeechAnchor>();
        _consoleBox = FindAnyObjectByType<ConsoleBox>();
        
        _debugSlot.OnBlockSet += CheckAnswer;
    }
    
    private void CheckAnswer(EngineBlock block)
    {
        if (block.Type != _answerBlockTypes[_currentStep])
        {
            Debug.Log("오답");
            return;
        }
        
        Debug.Log("정답");
        _currentStep++;
    }

    public void ShowDialogue(string id)
    {
        StageBaseManager.Instance.DialogueManager.StartDialogue(id, _speechAnchor.transform);
    }

    public void ChangeMessage()
    {
        
    }
}
