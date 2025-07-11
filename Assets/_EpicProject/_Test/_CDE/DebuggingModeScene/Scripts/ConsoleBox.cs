using TMPro;
using UnityEngine;

public class ConsoleBox : MonoBehaviour
{
    [SerializeField] private TMP_Text consoleText;
    
    private SimpleDialogueLoader _dialogueLoader;
    private TypeEffect _typeEffect;
    
    private void Awake()
    {
        _dialogueLoader = FindAnyObjectByType<SimpleDialogueLoader>();
        _typeEffect = consoleText.GetComponent<TypeEffect>();
    }

    public void ChangeConsoleText(string id)
    {
        string targetText = _dialogueLoader.GetDialogue(id);
        _typeEffect.SetMsg(targetText);
    }
}
