using UnityEngine;
using UnityEngine.UI;

public class MobileSetup : MonoBehaviour
{
    [Header("Canvas Settings")]
    [SerializeField] private Canvas canvas;

    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float baseOrthographicSize = 10f;

    void Start()
    {
        SetupCanvas();
        SetupCamera();
        SetupSafeArea();
    }

    void SetupCanvas()
    {
        if (canvas == null) return;

        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
    }

    void SetupCamera()
    {
        if (mainCamera == null) return;

        float targetAspect = 16f / 9f;
        float currentAspect = (float)Screen.width / Screen.height;

        if (currentAspect < targetAspect)
        {
            mainCamera.orthographicSize = baseOrthographicSize / (currentAspect / targetAspect);
        }
        else
        {
            mainCamera.orthographicSize = baseOrthographicSize;
        }
    }

    void SetupSafeArea()
    {
        if (canvas == null) return;

        Rect safeArea = Screen.safeArea;
        RectTransform rectTransform = canvas.GetComponent<RectTransform>();

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = anchorMin + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}

