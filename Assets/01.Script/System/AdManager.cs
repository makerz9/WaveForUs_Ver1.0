using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static AdManager Instance;

    [Header("Game IDs")]
    //[SerializeField] private string androidGameId = "6f561b47-0a52-422e-b556-60b51bdf9bd1";
    //[SerializeField] private string iOSGameId = "6f561b47-0a52-422e-b556-60b51bdf9bd1";

    [Header("Game IDs")]
    [SerializeField] private string androidGameId = "5734242"; // ← 이렇게 바꾸기!
    [SerializeField] private string iOSGameId = "5734243";


    [Header("Settings")]
    [SerializeField] private bool testMode = true;

    private string gameId;
    private string adPlacementId = "Rewarded_Android";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeAds();
    }

    void InitializeAds()
    {
#if UNITY_ANDROID
        gameId = androidGameId;
#elif UNITY_IOS
            gameId = iOSGameId;
#else
            gameId = androidGameId;
#endif

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(gameId, testMode, this);
            Debug.Log($"Unity Ads 초기화 시작! Game ID: {gameId}");
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log(" Unity Ads 초기화 완료!");
        LoadRewardedAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($" Unity Ads 초기화 실패: {error} - {message}");
    }

    public void LoadRewardedAd()
    {
        Debug.Log("광고 로드 시작...");
        Advertisement.Load(adPlacementId, this);
    }

    public void ShowRewardedAd()
    {
        if (Advertisement.isInitialized)
        {
            Debug.Log("광고 표시 시도...");
            Advertisement.Show(adPlacementId, this);
        }
        else
        {
            Debug.LogError("Unity Ads가 초기화되지 않았습니다!");
        }
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log(" 광고 로드 완료!");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($" 광고 로드 실패: {error} - {message}");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log($"광고 종료: {showCompletionState}");

        if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log(" 광고 시청 완료! 부활!");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.RevivePlayer();
            }
        }

        LoadRewardedAd();
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError($" 광고 표시 실패: {error} - {message}");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("광고 시작!");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("광고 클릭!");
    }
}