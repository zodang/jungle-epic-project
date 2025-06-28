using UnityEngine;

public class TempWall : MonoBehaviour
{
    public GameObject Left;
    public GameObject Right;
    public float XOffset;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Left.transform.localPosition = new Vector3(+XOffset, 0f, 0f);
        Right.transform.localPosition = new Vector3(-XOffset, 0f, 0f);
    }
}
