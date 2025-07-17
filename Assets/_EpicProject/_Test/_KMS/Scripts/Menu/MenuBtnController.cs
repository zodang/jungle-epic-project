using UnityEngine;
using UnityEngine.EventSystems;

public class MenuBtnController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("Hierarchy에서 Folder l 오브젝트")]
    [SerializeField] private GameObject folderLeft;
    [Tooltip("Hierarchy에서 Folder r 오브젝트")]
    [SerializeField] private GameObject folderRight;

    private void Awake()
    {
        // 시작 시에는 숨겨놓기
        if (folderLeft) folderLeft.SetActive(false);
        if (folderRight) folderRight.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (folderLeft) folderLeft.SetActive(true);
        if (folderRight) folderRight.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (folderLeft) folderLeft.SetActive(false);
        if (folderRight) folderRight.SetActive(false);
    }
}
