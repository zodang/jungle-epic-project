using Define;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class PuzzleFSM : MonoBehaviour
{
    public Action OnBlockCorrect; 

    [Header("Block System")]
    private BrokenEmotionBlock _target;
    private DebugSlot _debugSlot;
    private EngineController _engineController;
    private RectTransform _engineRectTransform;

    [Header("UI")]
    private SimpleDialogueUI _simpleDialogueUI;
    private ConsoleBox _consoleBox;

    [Header("Step")]
    private int _currentStep = -1;
    private EngineBlock _currentBlock;
    private FSM<FSMState> _fsm;
    private string _currentId;
    private BlockType[] _answerBlockTypes = new[] { BlockType.Light, BlockType.Scale, BlockType.Speed };

    private void Start()
    {
        _simpleDialogueUI = FindAnyObjectByType<SimpleDialogueUI>();
        _engineController = FindAnyObjectByType<EngineController>();
        _engineRectTransform = _engineController.GetComponent<RectTransform>();
        
        _fsm = new FSM<FSMState>();
    }

    public void StartPuzzle()
    {
        _consoleBox = FindAnyObjectByType<ConsoleBox>();
        _target = FindAnyObjectByType<BrokenEmotionBlock>();
        _debugSlot = FindAnyObjectByType<DebugSlot>();
        
        _debugSlot.OnBlockSet += CheckAnswer;

        ChangeStep();
    }
    
    private void CheckAnswer(EngineBlock block)
    {
        _currentBlock = block;
        
        if (block.Type != _answerBlockTypes[_currentStep])
        {
            // 오답 블록 시
            StopAllCoroutines();
            StartCoroutine(HandleWrongBlock(block));
            return;
        }
        
        // 정답 블록 시
        OnBlockCorrect?.Invoke();
    }

    public void ChangeToNextStep()
    {
        StopAllCoroutines();
        StartCoroutine(HandleAnswer(_currentBlock));
    }

    private void ChangeStep()
    {
        _currentStep++;
        
        switch (_currentStep)
        {
            case 0:
                _fsm.ChangeState(new LightStepState(this, _fsm, _target));
                break;
            case 1:
                _fsm.ChangeState(new ScaleStepState(this, _fsm, _target));
                break;
            case 2:
                _fsm.ChangeState(new SpeedStepState(this, _fsm, _target));
                break;
            case 3:
                GameManager.Instance.FadeManager.LoadNextScene(TransitionType.FadeType);
                break;
        }
    }

    public void ChangeDialogueText(string id)
    {
        _currentId = id;
        _simpleDialogueUI.ChangeSpeechBubbleUI(id);
    }

    public void ChangeConsoleText(string id)
    {
        _consoleBox.ChangeConsoleText(id);
    }

    private IEnumerator HandleWrongBlock(EngineBlock block)
    {
        // 오답 대사로 변경
        _simpleDialogueUI.ChangeSpeechBubbleUI("step_retry_player");
        _engineRectTransform.DOShakePosition(1f, new Vector2(10f, 10f));
        yield return new WaitForSeconds(1f);
        
        // 기존 Slot으로 이동
        _engineController.DropToToolBoxSlot(block);
        yield return new WaitForSeconds(3.0f);
        
        // 유도 대사로 변경
        _simpleDialogueUI.ChangeSpeechBubbleUI(_currentId);
    }

    private IEnumerator HandleAnswer(EngineBlock block)
    {
        yield return new WaitForSeconds(3f);
        
        _engineController.DropToToolBoxSlot(block);
        ChangeStep();
    }
}
