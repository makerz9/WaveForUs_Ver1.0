using UnityEngine;
using UnityEngine.UI;

public class UIResizer : MonoBehaviour
{
    public RectTransform buttonRect; // 버튼 RectTransform
    public RectTransform imageRect; // 이미지 RectTransform

    void Start()
    {
        ResizeUI();
    }

    void ResizeUI()
    {
        // 화면 비율
        float screenRatio = (float)Screen.height / Screen.width;

        // 버튼 크기 조정 (예: 화면의 10% 너비, 5% 높이)
        float buttonWidth = Screen.width * 0.1f;
        float buttonHeight = Screen.height * 0.05f;
        buttonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);

        // 이미지 비율 유지하며 화면 크기에 맞추기
        float imageWidth = Screen.width * 0.5f; // 화면의 50% 너비
        float imageHeight = imageWidth * (1080f / 1920f); // 이미지의 고유 비율
        imageRect.sizeDelta = new Vector2(imageWidth, imageHeight);

        // 버튼 위치 (화면 하단 중앙)
        buttonRect.anchoredPosition = new Vector2(0, -Screen.height * 0.1f);

        // 이미지 위치 (화면 상단 중앙)
        imageRect.anchoredPosition = new Vector2(0, Screen.height * 0.3f);
    }
}
