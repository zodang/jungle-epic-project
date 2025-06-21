using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class GrassLeaf : MonoBehaviour
{
    public enum State
    {
        normal,
        dry
    };

    public GameObject normalGrass;
    public GameObject dryGrass;

    private SpriteRenderer _normalRenderer;
    private SpriteRenderer _dryRenderer;

    public State state;
    private bool _isDry;

    //[Header("Dry")]
    //private int DrySteps = 3;
    //private float DryStepDuration = 0.8f;

    private void Awake()
    {
        if (!normalGrass.IsUnityNull())
        {
            _normalRenderer = normalGrass.GetComponent<SpriteRenderer>();
        }
        if (!dryGrass.IsUnityNull())
        {
            _dryRenderer = dryGrass.GetComponent<SpriteRenderer>();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (state != State.normal)
        {
            if (!_normalRenderer.IsUnityNull())
            {
                Color normalColor = _normalRenderer.color;
                normalColor.a = 0f;
                _normalRenderer.color = normalColor;

                //normalGrass.SetActive(false);
            }
        }
        if (state != State.dry)
        {
            _isDry = false;
            if (!_dryRenderer.IsUnityNull())
            {
                Color dryColor = _dryRenderer.color;
                dryColor.a = 0f;
                _dryRenderer.color = dryColor;

                //dryGrass.SetActive(false);
            }
        }
    }


    [ContextMenu("FadeToDry()")]
    public void FadeToDry()
    {
        if(_isDry)
        {
            return;
        }
        _isDry = true;
        state = State.dry;
        //dryGrass.SetActive(true);
        StartCoroutine(FadeToDryCoroutine());// DrySteps, DryStepDuration));
    }

    IEnumerator FadeToDryCoroutine()//int fadeSteps, float stepDuration)
    {
        if (_normalRenderer.IsUnityNull() || _dryRenderer.IsUnityNull())
        {
            yield break;
        }

        Color normalColor = _normalRenderer.color;
        Color dryColor = _dryRenderer.color;

        normalColor.a = 0.5f;
        dryColor.a = 1f - normalColor.a;
        _normalRenderer.color = normalColor;
        _dryRenderer.color = dryColor;
        yield return new WaitForSeconds(0.7f);

        normalColor.a = 0.2f;
        dryColor.a = 1f - normalColor.a;
        _normalRenderer.color = normalColor;
        _dryRenderer.color = dryColor;
        yield return new WaitForSeconds(0.9f);

        normalColor.a = 0.0f;
        dryColor.a = 1f - normalColor.a;
        _normalRenderer.color = normalColor;
        _dryRenderer.color = dryColor;


        //for (int i = 1; i <= fadeSteps; i++)
        //{
        //    float alpha = i / (float)fadeSteps;

        //    float normalAlpha = 1f - alpha;

        //    normalColor.a = normalAlpha;
        //    dryColor.a = 1f - normalAlpha;

        //    _normalRenderer.color = normalColor;
        //    _dryRenderer.color = dryColor;
        //    yield return new WaitForSeconds(stepDuration);
        //}
        ////normalGrass.SetActive(false);
    }


}
