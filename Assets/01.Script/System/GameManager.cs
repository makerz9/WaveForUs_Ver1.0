using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if ( _instance == null )
            {   // 로그만 찍지 말고, 씬에 혹시 숨어있는 놈이 있는지 찾아보자!
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    Debug.Log("GameManager가 아직 없습니다!");
                }
            }
            return _instance;
        }


        private set
        {
            _instance = value;
        }
    }

    [Header("Value_Setting")]
    [SerializeField] private int _handleCount;
    [SerializeField] private int _timer;

    public int HandleCount
    {
        get {  return _handleCount; }
        set
        {
            _handleCount = value;
            UICheck();
        }
    
    }
    public int TIMER
    {
        get { return _timer; }
        set
        {
            _timer = value;
            UICheck();
        }

    }

    [Header("UI_Setting")]
    [SerializeField] private TMP_Text txtHandle;
    [SerializeField] private TMP_Text txtTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this) //1. 객체 존재함 과 동시에 그 객체가 내가 아님 = 내가 작동하는 게임매니저가 아니다
        {
            Destroy(gameObject); //2. 내가 아니라는건 이미 다른 동작하는 게임매니저가 있음 = 자살
            return; //3. 자살후 끝
        }

        //4.자신이 첫 싱글톤이라 위 검사에 걸리지 않았다면
        Instance = this; //5. 객체는 자신 (싱글톤) 인정
        DontDestroyOnLoad(gameObject); //6. 불사 부여

        HandleCount = _handleCount;
        TIMER = _timer;
    }

    private void Start()
    {
        UICheck();
        StartCoroutine(TimerRoutine());
    }

    public bool UseHandle()
    {
        if (HandleCount <= 0) return false;
        HandleCount--;
        return true;
    }

    private IEnumerator TimerRoutine()
    {
        while (TIMER > 0)
        {
            yield return new WaitForSeconds(1f);
            TIMER--;
        }
        OnTimerEnd();
    }

    private void OnTimerEnd()
    {
        Debug.Log("타이머 종료");
    }

    public void UICheck()
    {
        txtHandle.text = $"{HandleCount}";
        txtTimer.text = $"{TIMER}";
    }
}