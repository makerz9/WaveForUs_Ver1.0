using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    public Vector2 referenceResolution = new Vector2(1080, 1920);

    void Start()
    {
        AdjustResolution();
    }

    void AdjustResolution()
    {
        float screenRatio = (float)Screen.height / Screen.width;
        float referenceRatio = referenceResolution.y / referenceResolution.x;

        if (screenRatio >= referenceRatio)
        {
            // 세로 해상도 기준
            Camera.main.orthographicSize = referenceResolution.y / 200f;
        }
        else
        {
            // 가로 해상도 기준
            Camera.main.orthographicSize = (referenceResolution.y / 200f) * (referenceRatio / screenRatio);
        }
    }
}
