using UnityEngine;
using System.Collections.Generic;

public class GlitchVision : MonoBehaviour
{
    public float glitchVisionDuration = 3f; // GlitchVision 지속 시간
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
            // Q 키를 눌렀을 때 GlitchVision 활성화
            ActivateGlitchVision();

            // glitchVisionDuration 일정시간 후, GlitchVision 비활성화
            Invoke("DeactivateGlitchVision", glitchVisionDuration); 
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

    public void DeactivateGlitchVision()
    {
        for (int i = 0; i < glitchObjects.Count; i++)
        {
            // 모든 glitchObjects에 대해 HideGlitch 호출
            if (glitchObjects[i] != null)
            {
                glitchObjects[i].HideGlitch();
            }
        }
    }


}
