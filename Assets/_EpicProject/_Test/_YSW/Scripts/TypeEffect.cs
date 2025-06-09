using UnityEngine;
using UnityEngine.UI;

public class TypeEffect : MonoBehaviour
{
    public string targetMsg;
    int CharPerSeconds;
    Text msgText;
    int index;
    //public GameObject EndCursor;

    private void Awake()
    {
        msgText = GetComponent<Text>();
    }

    public void SetMsg(string msg)
    {
        targetMsg = msg;
        EffectStart();
    }

    void EffectStart()
    {
        msgText.text = "";
        index = 0;
        //EndCursor.SetActive(false);
        Invoke("Effecting", 1/CharPerSeconds);
    }

    void Effecting()
    {
        if(msgText.text == targetMsg)
        {
            EffectEnd();
            return;
        }

        msgText.text += targetMsg[index];
        index++;

        Invoke("Effecting", 1 / CharPerSeconds);
    }

    void EffectEnd()
    {
        //EndCursor.SetActive(true);
    }
}
