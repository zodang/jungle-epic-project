using UnityEngine;

public class PopupOnClick : MonoBehaviour, IClickable
{
    public void OnClicked()
    {
        Debug.Log("@@DE ---> 클릭!");
        // 팝업 띄우기
    }
}
