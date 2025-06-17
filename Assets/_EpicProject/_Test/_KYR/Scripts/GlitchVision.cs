using UnityEngine;
using System.Collections.Generic;

public class GlitchVision : MonoBehaviour
{

    public List<GlitchObject> glitchObjects; // GlitchObject 리스트



    private void Start()
    {
        // GlitchObject 리스트 초기화 및 추가
        glitchObjects = new List<GlitchObject>(FindObjectsByType<GlitchObject>(FindObjectsSortMode.None));
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ActivateGlitchVision();
        }
    }


    public void ActivateGlitchVision()
    {
        for (int i = 0; i < glitchObjects.Count; i++)
        {
            // 모든 glitchObjects에 대해 ShowGlitch 호출
            if (glitchObjects[i] != null)
            {
                glitchObjects[i].ShowGlitch();
            }
        }
    }
    

}
