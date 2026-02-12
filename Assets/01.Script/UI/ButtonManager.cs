using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonManager : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private string buttonName;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;

    }

    // Update is called once per frame
    void Update()
    {
        


    }

    public void OnClick()
    {
        GameManager.Instance.SoundCall("button");

        if (buttonName == "Restart")
        {
            gameManager.GameRestart();
        }
        else if(buttonName == "TitleStart")
        {
            gameManager.TitleStart();
            gameObject.SetActive(false);
        }
        else if(buttonName == "GameRealStart")
        {
            gameManager.GameRealStart();
            gameObject.SetActive(false);
        }
        else if (buttonName == "AdRevive")
        {
            gameManager.OnAdButtonClick();
            gameObject.SetActive(false);
        }
        else if (buttonName == "Setting")
        {
            gameManager.SettingUI();
            //gameObject.SetActive(false);
        }

    }

}
