using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAspectRatio : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private float targetAspect = 16f / 9f;
    [SerializeField] private float baseOrthographicSize = 10f;

    void Start()
    {
        AdjustCamera();
    }

    void AdjustCamera()
    {
        float currentAspect = (float)Screen.width / Screen.height;

        if ((currentAspect > targetAspect))
        {
            Camera.main.orthographicSize = baseOrthographicSize;
        }
        else
        {
            Camera.main.orthographicSize = baseOrthographicSize / (currentAspect / targetAspect);
        }


    }


}
