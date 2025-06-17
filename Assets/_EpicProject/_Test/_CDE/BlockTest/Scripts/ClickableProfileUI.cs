using UnityEngine;
using UnityEngine.UI;

public class ClickableProfileUI : MonoBehaviour
{
    private Image _profileImg;

    private void Awake()
    {
        _profileImg = GetComponentInChildren<Image>();
    }

    public void SetProfileImg(ClickableProfile profile)
    {
        // 프로필 이미지 변경
        if (profile == null) return;
        _profileImg.sprite = profile.sprite;
    }
}
