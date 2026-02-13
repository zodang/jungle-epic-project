using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HyperLinkHelper : MonoBehaviour, IPointerClickHandler
{
    private TMP_Text _tmpText;
    private Camera _camera;

    private void Start()
    {
        _tmpText = GetComponent<TMP_Text>();
        _camera =  Camera.main;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(_tmpText, Input.mousePosition, null);

        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = _tmpText.textInfo.linkInfo[linkIndex];
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }
}
