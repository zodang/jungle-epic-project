using UnityEngine;

public class Sun : MonoBehaviour, ILightAdjustable, IRotatable
{
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AdjustLight(float brightness)
    {
        if (brightness >= 2.0f)
        {
            Debug.Log("≥Ï¿Ω, ¡ıπﬂ »£√‚");
            EvaporationHandler[] evaporations = FindObjectsByType<EvaporationHandler>(FindObjectsSortMode.None);
            for(int i = 0; i < evaporations.Length; i++)
            {
                evaporations[i].Evaporate();
            }
        }
        else if (brightness <= 0.2f)
        {
            Debug.Log("π„");
        }
    }

    // 0 ~ 359
    public void SetRotate(float angle)
    {
        if(angle < 60)
        {

        }
        else if(angle < 200)
        {

        }
        else if(angle < 280)
        {

        }
        else
        {

        }
    }

    [Header("Context Menu")]
    public float testBrightness;

    [ContextMenu("AdjustLight(testBrightness)")]
    void TestAdjustLight()
    {
        AdjustLight(testBrightness);
    }
}
