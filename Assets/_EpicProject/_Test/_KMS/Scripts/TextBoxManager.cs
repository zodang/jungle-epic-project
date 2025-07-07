using System.Collections;
using UnityEngine;
using TMPro;

public class TextBoxManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _targetText;
    private float interval = 0.5f;

    private Coroutine _loopRoutine;

    private void Start()
    {
        _targetText = GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        _loopRoutine = StartCoroutine(AnimateDots());
    }

    private void OnDisable()
    {
        if (_loopRoutine != null)
            StopCoroutine(_loopRoutine);
    }

    private IEnumerator AnimateDots()
    {
        string[] dotPhases = { "   ", ".  ", ".. ", "..." };
        int index = 0;

        while (true)
        {
            _targetText.text = dotPhases[index];
            index = (index + 1) % dotPhases.Length;
            yield return new WaitForSeconds(interval);
        }
    }
}
