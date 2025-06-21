using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DummyGrassAlpha : MonoBehaviour
{
    public SpriteRenderer wetGrass;
    public SpriteRenderer dryGrass;

    public int cnt;
    public float time;

    public bool testbool;
    [ContextMenu("test")]
    void test()
    {
        ChageGrass(testbool);
    }

    void ChageGrass(bool setDry)
    {
        StartCoroutine(ChageGrassCoroutine(setDry, cnt, time));
    }

    IEnumerator ChageGrassCoroutine(bool setDry, int cnt, float time)
    {
        for(int i = 1; i <= cnt; i++)
        {
            yield return new WaitForSeconds(time);

            float alpha = i / (float)cnt;

            Color wetColor = wetGrass.color;
            Color dryColor = dryGrass.color;

            // 1, 0.7, 0.3, 0
            float wetAlpha = (setDry) ? 1f - alpha : alpha;

            wetColor.a = wetAlpha;
            dryColor.a = 1f - wetAlpha;

            wetGrass.color = wetColor;
            dryGrass.color = dryColor;
        }
    }
}
