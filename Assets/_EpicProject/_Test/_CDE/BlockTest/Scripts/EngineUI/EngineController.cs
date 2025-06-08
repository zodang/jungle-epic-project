using Define;
using UnityEngine;
using System.Collections;

public class EngineController : MonoBehaviour
{
    public Clickable CurrentTarget { get; private set; }

    private EngineUIController _engineUIController;
    private EngineBlockController _engineBlockInspector;
    private Animator _engineAnimator;

    //0.25초 후에 UI 끄기 위해
    [SerializeField] private float _durationTime = 0.25f;

    private void Awake()
    {
        _engineUIController = GetComponent<EngineUIController>();
        _engineBlockInspector = GetComponent<EngineBlockController>();
        _engineAnimator = GetComponent<Animator>(); 
    }

    private void Start()
    {
        gameObject.AddComponent<DraggableUI>();
        gameObject.SetActive(false);
    }

    public void OpenInspector(Clickable target)
    {
        AudioManager.instance.playSfx(SfxType.Open);
        
        // Block 세팅
        CurrentTarget = target;
        _engineBlockInspector.AddBlock(target);
        
        // UI 세팅
        _engineUIController.SetProfile(target.GetProfile(), target);
        _engineUIController.SetUIPosition(target);
        
        gameObject.SetActive(true);
        _engineAnimator.Play("On Ani");        // UI "On Ani" 실행~
    }

    public void CloseInspector()
    {
        AudioManager.instance.playSfx(SfxType.Close);

        CurrentTarget = null;
        _engineUIController.ClearProfile();

        _engineAnimator.Play("Off Ani");       // UI "Off Ani" 실행~
        StartCoroutine(CloseAfterAnimation()); // 0.25초 후 비활성화 실행 ㅋ
    }

    private IEnumerator CloseAfterAnimation()
    {
        yield return new WaitForSeconds(_durationTime);

        CurrentTarget = null;
        _engineUIController.ClearProfile();

        gameObject.SetActive(false);
    }
    public void RefreshSlot(Clickable target)
    {
        CurrentTarget = target;
        _engineBlockInspector.AddBlock(target);
    }
}
