using Define;
using UnityEngine;

public class PuzzleFSM : MonoBehaviour
{
    private FSM<FSMState> _fsm;
    
    private DebugSlot _debugSlot;


    private SimpleDialogueUI _simpleDialogueUI;
    private ConsoleBox _consoleBox;
    
    private BlockType[] _answerBlockTypes = new[] { BlockType.Light, BlockType.Scale, BlockType.Speed };
    private int _currentStep;
    
    private void Awake()
    {
        _fsm = new FSM<FSMState>();
        _simpleDialogueUI = FindAnyObjectByType<SimpleDialogueUI>();
    }
    
    public void StartPuzzle()
    {
        _debugSlot = FindAnyObjectByType<DebugSlot>();
        _consoleBox = FindAnyObjectByType<ConsoleBox>();
        
        _debugSlot.OnBlockSet += CheckAnswer;
        _consoleBox.ChangeConsoleText("console_speed_solved");

    }
    
    private void CheckAnswer(EngineBlock block)
    {
        if (block.Type != _answerBlockTypes[_currentStep])
        {
            _simpleDialogueUI.ChangeSpeechBubbleUI("step_retry_player");
            return;
        }
        
        Debug.Log("정답");
        _currentStep++;
    }

    public void ShowDialogue(string id)
    {
        
    }

    public void ChangeMessage()
    {
        
    }
}
