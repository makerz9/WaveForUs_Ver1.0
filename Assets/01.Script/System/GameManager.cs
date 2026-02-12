using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement; // ← 상단에 추가!
using UnityEngine.Advertisements;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    #region UISetting

    [Header("[SetUI]")]
    [Space(5f)]
    public int score;
    public int highScore;

    [Space(10f)]

    [SerializeField] private TMP_Text txtScore;
    [SerializeField] private TMP_Text txtHighScore;
    [SerializeField] private GameObject txtNewRecord;

    [SerializeField] private GameObject scoreBase;
    [SerializeField] private GameObject highScoreBase;

    [SerializeField] private Transform GameOverUI;

    [SerializeField] private Transform[] uiBlind;
    //[SerializeField] private UnityEngine.UI.Image IMGCastleHP;

    //타이틀
    [SerializeField] private TMP_Text txtAreYouReady;

    //튜토
    [SerializeField] private Transform Finger;
    private Vector3 fingerStartPos;

    //튜토
    [SerializeField] private Transform title;
    [SerializeField] private Transform startReady;

    [SerializeField] private Transform howToPlay;
    [SerializeField] private Transform getReady;

    [SerializeField] private Transform tImage;

    [SerializeField] private GameObject WallCreater;
    [SerializeField] private GameObject tButton1;
    [SerializeField] private GameObject tButton2;

    [Header("Revive")]
    [SerializeField] private GameObject shipPrefab; // Ship 프리팹
    [SerializeField] private GameObject adButton; // 광고 버튼
    [SerializeField] private Transform playButton; // 광고 버튼

    private bool isShowingAd = false; // ← 추가!


    // 원래 txtScore 위치/크기 저장
    private Vector3 originalScorePos;
    private Vector3 originalScoreScale;

    [SerializeField] private GameObject UISetting;

    private bool isSetting = false;
    #endregion


    #region Sound

    [Space(20f)]

    [Header("Sound")]

    public string BGM_Main;
    [SerializeField] private string SE_Button1;
    [SerializeField] private string SE_MiniWaterDrop;
    [SerializeField] private string SE_WaterDrop1;
    [SerializeField] private string SE_WaterDrop2;
    [SerializeField] private string SE_Stone;

    [SerializeField] private string SE_Boom1;
    [SerializeField] private string SE_Boom2;
    [SerializeField] private string SE_Boom3;
    [SerializeField] private string SE_Boom4;

    [SerializeField] private string SE_Wave1;
    [SerializeField] private string SE_Wave2;
    [SerializeField] private string SE_Wave3;

    [SerializeField] private string SE_Button;

    #endregion


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.PlayBGM(BGM_Main);
        LoadHighScore();

        fingerStartPos = Finger.localPosition;

        // SessionStart 체크
        int sessionStart = PlayerPrefs.GetInt("SessionStart", 1);

        if (sessionStart == 1)
        {
            // 처음: 타이틀 표시, AutoStart 실행 안 함!
            GameStarting();
            StartCoroutine(FingerMove());

            txtAreYouReady.transform.DOLocalMoveY(15.0f, 1.0f)
                .SetRelative(true)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            // 재시작: 타이틀 건너뛰고 AutoStart
            GameStarting();
            StartCoroutine(AutoStart());
        }


        // txtScore 원래 상태 저장
        originalScorePos = txtScore.transform.localPosition;
        originalScoreScale = txtScore.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        UIShow();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //ResetHighScore();
            //ResetSession(); // ← 추가!
            ResetAll();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            SoundCall("waterMiniDrop");
        }

        // R 키로 부활 테스트
        if (Input.GetKeyDown(KeyCode.R))
        {
            RevivePlayer();
        }
    }

    #region LoadHighScore

    void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        Debug.Log($"불러온 최고점수: {highScore}");
    }

    void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save(); // 즉시 저장
        Debug.Log($"저장된 최고점수: {highScore}");
    }

    #endregion


    public void ScoreUp()
    {
        score++;

        Sequence scaleSeq = DOTween.Sequence();

        scaleSeq.Append(scoreBase.transform.DOScale(1.4f, 0.2f).SetEase(Ease.OutBack));
        scaleSeq.Append(scoreBase.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack));


        if (score >= highScore)
        {
            highScore = score;
            SaveHighScore(); //호출

            Sequence scaleSeq2 = DOTween.Sequence();

            scaleSeq2.Append(highScoreBase.transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack));
            scaleSeq2.Append(highScoreBase.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack));
        }
    }


    private void UIShow()
    {
        txtScore.text = $"{score}";
        txtHighScore.text = $"{highScore}";



        //float PlayerHpvalue = CastleHP / CastleMaxHP;
        //IMGCastleHP.fillAmount = PlayerHpvalue;
    }

    public void GameOver()
    {
        SaveHighScore();

        GameOverUI.transform.DOLocalMoveY(25, 1.0f).SetEase(Ease.OutQuad);

        txtScore.transform.DOLocalMoveY(-80, 0.8f).SetEase(Ease.OutQuad);
        txtScore.transform.DOScale(1.25f, 1.0f).SetEase(Ease.OutQuad);

        // 광고 버튼 표시
        //if (adButton != null) adButton.SetActive(true);

        if (score > 0)
        {
            if (score >= highScore)
            {
                txtNewRecord.SetActive(true);

                float originalY = txtNewRecord.transform.localPosition.y;

                //
                txtNewRecord.transform.DOLocalMoveY(originalY + 10.0f, 1.0f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }
        }

    }

    public void GameRestart()
    {
        Debug.Log("게임오버");
        SoundManager.instance.PlaySE(SE_MiniWaterDrop, 1.0f, 0.008f); //????  //????
        StartCoroutine(RestartSequence());
    }


    //게임 최초 시작
    public void TitleStart()
    {
        Debug.Log("게임최초시작");

        // 이제부터 재시작 시 타이틀 건너뛰기
        PlayerPrefs.SetInt("SessionStart", 0);
        PlayerPrefs.Save();
        //SoundManager.instance.PlaySE(SE_MiniWaterDrop, 1.0f, 0.008f); //????  //????

        // 최초 실행 플래그 OFF
        PlayerPrefs.SetInt("IsFirstPlay", 0);
        PlayerPrefs.Save();

        txtAreYouReady.transform.DOKill();
        startReady.transform.DOLocalMoveY(-800, 1.0f).SetEase(Ease.OutQuad);
        title.transform.DOLocalMoveY(800, 1.0f).SetEase(Ease.OutQuad);



        getReady.transform.DOLocalMoveY(-400, 1.0f).SetEase(Ease.OutQuad);
        howToPlay.transform.DOLocalMoveY(255, 1.0f).SetEase(Ease.OutQuad);

        TUP();

        // 1초 딜레이 후 실행!
        getReady.transform.DOLocalMoveY(15.0f, 1.0f)
             .SetDelay(1.0f)// ← 1초 대기!
            .SetRelative(true)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);


    }

    private void TUP()
    {
        tImage.transform.DOLocalMoveY(8, 1.0f).SetEase(Ease.OutQuad);
    }



    public void GameRealStart()
    {
        Debug.Log("게임진짜시작");
        //SoundManager.instance.PlaySE(SE_MiniWaterDrop, 1.0f, 0.008f); //????  //????

        getReady.transform.DOKill();
        getReady.transform.DOLocalMoveY(-800, 1.0f).SetEase(Ease.OutQuad);
        howToPlay.transform.DOLocalMoveY(800, 1.0f).SetEase(Ease.OutQuad);

        tImage.transform.DOLocalMoveY(-8, 1.0f).SetEase(Ease.OutQuad);

        WallCreater.SetActive(true);
    }

    IEnumerator RestartSequence()
    {


        for (int i = 0; i < uiBlind.Length; i++)
        {
            yield return new WaitForSeconds(0.05f);

            uiBlind[i].transform.DOLocalMoveY(-1300, 0.5f).SetEase(Ease.OutQuad);


        }

        yield return new WaitForSeconds(0.5f);
        //
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void GameStarting()
    {
        SoundManager.instance.PlaySE(SE_MiniWaterDrop, 1.0f, 0.008f); //????  //????
        StartCoroutine(StartingSequence());
    }

    IEnumerator StartingSequence()
    {


        for (int i = 0; i < uiBlind.Length; i++)
        {
            yield return new WaitForSeconds(0.05f);

            uiBlind[i].transform.DOLocalMoveY(-3800, 0.5f).SetEase(Ease.OutQuad);


        }

        yield return new WaitForSeconds(0.5f);
        //
        

    }



    // 자동 시작 코루틴
    IEnumerator AutoStart()
    {
        getReady.transform.DOKill();
        startReady.transform.DOKill();

        tButton1.SetActive(false);
        tButton2.SetActive(false);

        // 타이틀 UI 숨기기 (보이지 않게)
        startReady.localPosition = new Vector3(startReady.localPosition.x, -800, 0);
        title.localPosition = new Vector3(title.localPosition.x, 800, 0);
        getReady.localPosition = new Vector3(getReady.localPosition.x, -800, 0);
        howToPlay.localPosition = new Vector3(howToPlay.localPosition.x, 800, 0);


        yield return new WaitForSeconds(0.5f); // StartingSequence 기다리기


        // 바로 게임 시작
        WallCreater.SetActive(true);
    }


    // 새로 추가!
    public void ResetSession()
    {
        PlayerPrefs.SetInt("SessionStart", 1);
        PlayerPrefs.Save();
        Debug.Log("세션 초기화됨 (다음 시작 시 타이틀 표시)");
    }

    public void ResetAll()
    {
        // 최고점수 초기화
        PlayerPrefs.DeleteKey("HighScore");
        highScore = 0;

        // 세션 초기화 (다음 재시작 시 타이틀 표시)
        PlayerPrefs.SetInt("SessionStart", 1);

        PlayerPrefs.Save();

        Debug.Log("모든 데이터 초기화됨");
    }

    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey("HighScore");
        highScore = 0;
        Debug.Log("최고점수 초기화됨");
    }

    #region SoundCall

    public void SoundCall(string soundName)
    {
        if(soundName == "waterMiniDrop")
        {
            SoundManager.instance.PlaySE(SE_MiniWaterDrop, 1.0f, 0.06f); //????  //????
        }
        else if (soundName == "waterDrop1")
        {
            SoundManager.instance.PlaySE(SE_WaterDrop1, 1.0f, 0.06f); //????  //????
        }
        else if (soundName == "waterDrop2")
        {
            SoundManager.instance.PlaySE(SE_WaterDrop2, 1.0f, 0.07f); //????  //????
        }
        else if (soundName == "stone")
        {
            SoundManager.instance.PlaySE(SE_Stone, 1.0f, 0.05f); //????  //????
        }

        else if (soundName == "boom1")
        {
            SoundManager.instance.PlaySE(SE_Boom1, 1.0f, 0.5f); //????  //????
        }
        else if (soundName == "boom2")
        {
            SoundManager.instance.PlaySE(SE_Boom2, 1.0f, 0.08f); //????  //????
        }
        else if (soundName == "boom3")
        {
            SoundManager.instance.PlaySE(SE_Boom3, 1.0f, 0.08f); //????  //????
        }
        else if (soundName == "boom4")
        {
            SoundManager.instance.PlaySE(SE_Boom4, 1.0f, 0.04f); //????  //????
        }

        else if (soundName == "wave1")
        {
            SoundManager.instance.PlaySE(SE_Wave1, 1.0f, 0.07f); //????  //????
        }
        else if (soundName == "wave2")
        {
            SoundManager.instance.PlaySE(SE_Wave2, 1.0f, 0.07f); //????  //????
        }
        else if (soundName == "wave3")
        {
            SoundManager.instance.PlaySE(SE_Wave3, 1.0f, 0.07f); //????  //????
        }
        
        else if (soundName == "button")
        {
            SoundManager.instance.PlaySE(SE_Button, 1.0f, 0.07f); //????  //????
        }



    }

    #endregion

    #region FingerMove

    IEnumerator FingerMove()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.15f);

            // 터치 1 (작아졌다 커지기)
            yield return Finger.DOScale(0.4f, 0.1f).WaitForCompletion();
            yield return Finger.DOScale(0.6f, 0.1f).WaitForCompletion();

            yield return new WaitForSeconds(0.5f);

            // 위로 이동
            yield return Finger.DOLocalMoveY(fingerStartPos.y + 3.5f, 0.3f).WaitForCompletion();

            yield return new WaitForSeconds(0.5f);

            // 터치 2 (작아졌다 커지기)
            yield return Finger.DOScale(0.4f, 0.1f).WaitForCompletion();
            yield return Finger.DOScale(0.6f, 0.1f).WaitForCompletion();

            yield return new WaitForSeconds(1f);

            // 원위치
            yield return Finger.DOLocalMove(fingerStartPos, 1.35f).WaitForCompletion();

            yield return new WaitForSeconds(0.5f);
        }
    }

    #endregion

    #region GameReset

    // [안드로이드용] 
    private void OnApplicationPause(bool pause)
    {
        if (pause && !isShowingAd)
        {
            PlayerPrefs.SetInt("SessionStart", 1);
            PlayerPrefs.Save();
            Debug.Log("앱 백그라운드 - SessionStart 초기화");
        }
    }

    // [PC/에디터용] 앱 종료 시 자동 초기화
    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("SessionStart", 1);
        PlayerPrefs.Save();
        Debug.Log("앱 종료 - SessionStart 초기화됨");
    }

    #endregion


    #region Revive

    // 광고 시청 완료 후 부활!

    // 광고 버튼 클릭
    public void OnAdButtonClick()
    {
        SoundCall("button");

        if (AdManager.Instance != null)
        {
            isShowingAd = true; // ← 추가!
            AdManager.Instance.ShowRewardedAd();

            //playButton x가 0이 되게해줘
            playButton.localPosition = new Vector3(0, playButton.localPosition.y, 1f);

        }
        else
        {
            Debug.LogError("AdManager 없음!");
        }
    }


    public void RevivePlayer()
    {
        Debug.Log("플레이어 부활!");

        isShowingAd = false; // ← 추가!

        // 1. Spawner 비활성화
        WallCreater.SetActive(false);

        // 2. UI 치우기
        GameOverUI.transform.DOLocalMoveY(-800, 0.5f).SetEase(Ease.OutQuad);
        if (adButton != null) adButton.SetActive(false);
        txtNewRecord.SetActive(false);

        // txtScore 원래 위치/크기로 복귀
        txtScore.transform.DOLocalMove(originalScorePos, 1.0f).SetEase(Ease.OutQuad);
        txtScore.transform.DOScale(originalScoreScale, 1.0f).SetEase(Ease.OutQuad);

        // 3. 장애물 제거
        StartCoroutine(ClearObstacles());

        // 4. 기존 플레이어 삭제
        GameObject oldPlayer = GameObject.FindGameObjectWithTag("Player");
        if (oldPlayer != null)
        {
            Destroy(oldPlayer);
        }

        // 5. 새 Ship 생성 (-15, 20)
        if (shipPrefab != null)
        {
            Instantiate(shipPrefab, new Vector3(-15, 20, 0), Quaternion.identity);
            Debug.Log("새 Ship 생성됨!");
        }
        else
        {
            Debug.LogError("Ship Prefab이 할당되지 않았습니다!");
        }

        // 5. Spawner 재활성화
        Invoke(nameof(ReactivateSpawner), 1.5f);


    }

    IEnumerator ClearObstacles()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");

        // 아래로 이동
        foreach (GameObject obj in obstacles)
        {
            if (obj != null)
                obj.transform.DOLocalMoveY(-50, 1.0f).SetEase(Ease.InQuad);
        }
        foreach (GameObject obj in walls)
        {
            if (obj != null)
                obj.transform.DOLocalMoveY(-50, 1.0f).SetEase(Ease.InQuad);
        }

        yield return new WaitForSeconds(1.0f);

        // 삭제
        foreach (GameObject obj in obstacles)
        {
            if (obj != null) Destroy(obj);
        }
        foreach (GameObject obj in walls)
        {
            if (obj != null) Destroy(obj);
        }
    }

    void ReactivateSpawner()
    {
        WallCreater.SetActive(true);
        adButton.SetActive(false);
        Debug.Log("광고없어져라");
    }


    public void SettingUI()
    {
        if(isSetting == false)
        {
            UISetting.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutQuad);
            isSetting = true;
        }
        else
        {
            UISetting.transform.DOLocalMoveY(1000, 0.5f).SetEase(Ease.OutQuad);
            isSetting = false;
        }
    }


    #endregion

}
