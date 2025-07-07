using TMPro;
using UnityEditor;
using UnityEngine;

public class BuildVersionDisplayer : MonoBehaviour
{
    //public bool ShowBuildGUID = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI textMeshProUGUI))
        {
            string str = "[" + Application.version + " ver]";
            //if(ShowBuildGUID)
            //{
            //    str = str + "\n\r(GUID: " + Application.buildGUID + ")";
            //}
            textMeshProUGUI.text = str;
        }
    }
}
