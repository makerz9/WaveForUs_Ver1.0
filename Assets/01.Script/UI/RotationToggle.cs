using UnityEngine;

public class RotationToggle : MonoBehaviour
{
    private bool isLandscapeLeft = true;

    void Update()
    {
        // 스페이스바로 회전 전환 (테스트용)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleOrientation();
        }
    }

    void ToggleOrientation()
    {
        //if (isLandscapeLeft)
        //{
        //    Screen.orientation = ScreenOrientation.LandscapeRight;
        //    Debug.Log("Landscape Right");
        //}
        //else
        //{
        //    Screen.orientation = ScreenOrientation.LandscapeLeft;
        //    Debug.Log("Landscape Left");
        //}

        //isLandscapeLeft = !isLandscapeLeft;
    }

    // 게임 시작 시 가로 고정
    void Start()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
    }
}