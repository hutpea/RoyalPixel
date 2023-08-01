using BizzyBeeGames.ColorByNumbers;
using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public MoneyEffectController moneyEffectController;
    public UseProfile useProfile;
    public DataContain dataContain;
    public MusicManager musicManager;
    public AdmobAds admobAds;
    public AnalyticsController AnalyticsController;
    public IapController iapController;
    [HideInInspector] public SceneType currentScene;
    public LoadDataCate dataAreas;
    public LoadDataPic dataPic;
    public LoadDataPic dataEventFarm;
    public LoadAssetBundles assetBundles;
    //public CategoryInfo[] noelEvent;
    [SerializeField] TextureController textureController;
    public bool isTest;
    [SerializeField] private TestLevel testController;
    public List<TextAsset> picDaily;
    [SerializeField] TextAsset firstGame;
   // [SerializeField] NativeAdsManager adsManager;
    [SerializeField] Slider progressBar;
    protected void Awake()
    {
        if (Instance == null)
            Instance = this;

        DontDestroyOnLoad(this);

        //GameController.Instance.useProfile.IsRemoveAds = true;
#if UNITY_IOS
        if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == 
    ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
    {
        ATTrackingStatusBinding.RequestAuthorizationTracking();
    }
#endif

        //if (UseProfile.IsTrackedPremission)
        //{
            Init();
        //}
        //else
        //{
        //    TrackingBox.Setup().Show(() =>
        //    {
        //        Init();
        //    });
        //}
    }

    public void Init()
    {
        UseProfile.NumberOfAdsInPlay = 0;
        Application.targetFrameRate = 60;
        useProfile.CurrentLevelPlay = UseProfile.CurrentLevel;
        admobAds.Init();
        //adsManager.Init();
        musicManager.Init();
        musicManager.PlayBGHomeMusic();
        LoadFirstGame();
        //iapController.Init();
        GameData.aspect = Camera.main.aspect;
        MMVibrationManager.SetHapticsActive(useProfile.OnVibration);
        //UseProfile.IsVip = true;
#if !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;
#endif

    }
    public void LoadScene(string sceneName)
    {
        Initiate.Fade(sceneName.ToString(), Color.black, 1.2f);
    }
    void LoadFirstGame()
    {
        if (isTest)
        {
            testController.Inits();
            return;
        }

        //if (useProfile.NewUser)
        //{
        //    PictureInformation pictureInfo = new PictureInformation(firstGame.text);
        //    GameData.picChoice = pictureInfo;
        //    Debug.Log(" GameData.picChoice " + GameData.picChoice.Id);
        //    Texture2D tex = textureController.GenerateGrayscaleTexture(GameData.picChoice, 0.8f, true);
        //    Texture2D texture = textureController.GenerateColorPicture(GameData.picChoice);
        //    GameData.CurColorTexture = texture;
        //    GameData.curGrayTexture = tex;
        //    GameData.grayScale = textureController.GenerateGrayscaleTexture(GameData.picChoice, 0.2f, false);
        //    StartCoroutine(Helper.StartAction(() => LoadScene(SceneName.HOME_SCENE), 1));
        //}
        //else
        StartCoroutine(LoadLevelAsync(SceneName.HOME_SCENE));
    }
    private IEnumerator LoadLevelAsync(string scene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress);
            progressBar.value = progress;
            yield return null;
        }
        Time.timeScale = 1;
    }
    public static void SetUserProperties()
    {
        if (UseProfile.IsFirstTimeInstall)
        {
            UseProfile.FirstTimeOpenGame = UnbiasedTime.Instance.Now();
            UseProfile.LastTimeOpenGame = UseProfile.FirstTimeOpenGame;
            UseProfile.IsFirstTimeInstall = false;
        }

        var lastTimeOpen = UseProfile.LastTimeOpenGame;
        UseProfile.RetentionD = (UseProfile.FirstTimeOpenGame - UnbiasedTime.Instance.Now()).Days;

        var dayPlayerd = (TimeManager.ParseTimeStartDay(UnbiasedTime.Instance.Now()) - TimeManager.ParseTimeStartDay(UseProfile.LastTimeOpenGame)).Days;
        if (dayPlayerd >= 1)
        {
            UseProfile.LastTimeOpenGame = UnbiasedTime.Instance.Now();
            UseProfile.DaysPlayed++;
        }

        AnalyticsController.SetUserProperties();
    }
    public void ShowPanelPermissionSetting(int typePermission)
    {
        PopupPermission.Setup(typePermission).Show();
    }
}
public enum SceneType
{
    StartLoading = 0,
    MainHome = 1,
    GamePlay = 2
}
