using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using com.adjust.sdk;

public class AdmobAds : MonoBehaviour
{

    [SerializeField] private AdsController ironsourceController;
    public static bool isAdmobInitDone;

#if UNITY_ANDROID
    private const string InterstitialAdUnitId = "b54ac498d13ef37f";
    private const string RewardedAdUnitId = "6b227ce71cafca73";
    private const string BanerAdUnitId = "a7c39fcfb47b2e38";
#elif UNITY_IOS
    private const string InterstitialAdUnitId = "c8d31e48f08ed31e";
    private const string RewardedAdUnitId = "02932bb866cbb369";
    private const string BanerAdUnitId = "ff665c0a75cadcc4";
#endif
#if UNITY_ANDROID
    private string _adUnitIdHigh = "ca-app-pub-8467610367562059/9656627420";
    private string _adUnitIdMedium = "ca-app-pub-8467610367562059/2324932310";
    private string _adUnitIdLow = "ca-app-pub-8467610367562059/8893924685";
#elif UNITY_IPHONE
    private string _adUnitIdHigh = "ca-app-pub-3940256099942544/6978759866";
    private string _adUnitIdMedium = "ca-app-pub-3940256099942544/6978759866";
    private string _adUnitIdLow = "ca-app-pub-8467610367562059/4180939469";
#else
  private string _adUnitId = "unused";
#endif
    private RewardedInterstitialAd rewardedInterstitialAd;
    public float countdownAds;
    bool closeAds;
    public void Init()
    {
        ironsourceController.Init();
        countdownAds = 1000;

//#if !UNITY_EDITOR
        Debug.Log("===== Init Admob ====");
        MobileAds.Initialize((initStatus) =>
        {
            Debug.Log("===== Init Admob Done ====");
            AppOpenAdManager.Instance.LoadAd(() => { AppOpenAdManager.Instance.ShowAdIfAvailable(); });
            AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
            isAdmobInitDone = true;
            LoadRewardedInterstitialAd();
        });
//#endif
    }

    #region Interstitial
    public bool ShowInterstitial(bool isShowImmediatly = false, string actionWatchLog = "other", UnityAction actionIniterClose = null, string level = null)
    {
        if (UseProfile.IsRemoveAds || UseProfile.IsVip)
        {
            actionIniterClose?.Invoke();
            Time.timeScale = 1;
            return false;
        }

        //#if UNITY_EDITOR
        //        actionIniterClose?.Invoke();
        //        Time.timeScale = 1;
        //        return true;
        //#endif
        //if (UseProfile.CurrentLevel > RemoteConfigController.GetFloatConfig(FirebaseConfig.LEVEL_START_SHOW_INITSTIALL, 3))
        //{
        GameController.Instance.AnalyticsController.LoadInterEligible();
        Debug.Log("show inter ============== " + RemoteConfigController.GetFloatConfig(FirebaseConfig.DELAY_SHOW_INITSTIALL, 5));
        if (countdownAds > RemoteConfigController.GetFloatConfig(FirebaseConfig.DELAY_SHOW_INITSTIALL, 5) || isShowImmediatly)
        {
            GameController.Instance.AnalyticsController.LogInterShow(actionWatchLog);
            Debug.Log("show inter");
            //Time.timeScale = 0;

            ironsourceController.ShowInterstitial(() =>
            {
                if (actionIniterClose != null)
                {
                    actionIniterClose();
                }
                Time.timeScale = 1;

            },
        () =>
        {
            closeAds = true;
            UseProfile.NumberOfAdsInDay = UseProfile.NumberOfAdsInDay + 1;
            UseProfile.NumberOfAdsInPlay = UseProfile.NumberOfAdsInPlay + 1;
            actionIniterClose?.Invoke();
            Time.timeScale = 1;
            countdownAds = 0;
        });
        }
        else
        {
            if (actionIniterClose != null)
            {
                actionIniterClose();
            }
            Time.timeScale = 1;
        }
        //}
        //else
        //{
        //    if (actionIniterClose != null)
        //        actionIniterClose();
        //}

        return true;
    }
    public bool IsAvailableInterstitial()
    {
#if UNITY_EDITOR
        return false;
#else
        return IronSource.Agent.isInterstitialReady() && countdownAds > RemoteConfigController.GetFloatConfig(FirebaseConfig.DELAY_SHOW_INITSTIALL, 15);
#endif
    }
    public void ShowInterAutoClaim(string actionWatchLog = "other", UnityAction actionIniterClose = null, UnityAction actionFail = null)
    {
        ironsourceController.ShowInterstitial(() =>
        {
            if (actionFail != null)
            {
                actionFail();
            }
            Time.timeScale = 1;

        },
        () =>
        {
            closeAds = true;
            UseProfile.NumberOfAdsInDay = UseProfile.NumberOfAdsInDay + 1;
            UseProfile.NumberOfAdsInPlay = UseProfile.NumberOfAdsInPlay + 1;
            if (actionIniterClose != null)
                actionIniterClose?.Invoke();
            Time.timeScale = 1;
            countdownAds = 0;
        });
    }

    #endregion

    #region Video Reward

    /// <summary>
    /// Xử lý Show Video
    /// </summary>
    /// <param name="actionReward">Hành động khi xem xong Video và nhận thưởng </param>
    /// <param name="actionNotLoadedVideo"> Hành động báo lỗi không có video để xem </param>
    /// <param name="actionClose"> Hành động khi đóng video (Đóng lúc đang xem dở hoặc đã xem hết) </param>
    public void ShowVideoReward(UnityAction actionReward, UnityAction actionNotLoadedVideo, UnityAction actionClose, ActionWatchVideo actionType, string level)
    {
#if UNITY_EDITOR
        actionReward.Invoke();
#endif
        GameController.Instance.AnalyticsController.LogWatchVideo(actionType, true, Application.internetReachability != NetworkReachability.NotReachable, level);
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            actionNotLoadedVideo?.Invoke();
            return;
        }
        if (!ironsourceController.ShowRewardedVideo(actionNotLoadedVideo, () => { actionReward?.Invoke(); countdownAds = 0; closeAds = true; }))
        {
            actionNotLoadedVideo?.Invoke();
        }
    }

    #endregion

    #region Banner
    public void ShowBanner()
    {
        if (UseProfile.IsRemoveAds || UseProfile.IsVip)
            return;
        ironsourceController.ShowBanner();
    }
    public void DestroyBanner()
    {
        Debug.Log("destroy banner");
        ironsourceController.HideBanner();
    }
    #endregion

    #region Open App Ads
    DateTime oldTime = DateTime.MinValue;
    public void OnAppStateChanged(AppState state)
    {
        if (state == AppState.Foreground && TimeManager.CaculateTime(oldTime, DateTime.Now) > 30 && !closeAds)
        {
            // COMPLETE: Show an app open ad if available.
            AppOpenAdManager.Instance.ShowAdIfAvailable();
            oldTime = DateTime.Now;
        }
        closeAds = false;
    }
    #endregion
    #region Reward Inter
    public void LoadRewardedInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (rewardedInterstitialAd != null)
        {
            rewardedInterstitialAd.Destroy();
            rewardedInterstitialAd = null;
        }

        Debug.Log("Loading the rewarded interstitial ad.");
        // create our request used to load the ad.
        var adRequest = new AdRequest.Builder().Build();
        // send the request to load the ad.
        RewardedInterstitialAd.LoadAd(_adUnitIdLow, adRequest,
            (RewardedInterstitialAd ad, AdFailedToLoadEventArgs error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("rewarded interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    //LoadMoreRewardInter(_adUnitIdMedium);
                    return;
                }
                Debug.Log("Rewarded interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                rewardedInterstitialAd = ad;
                InitRegister(ad);
            });
    }
    //void LoadMoreRewardInter(string ID)
    //{
    //    var adRequest = new AdRequest.Builder().Build();
    //    // send the request to load the ad.
    //    RewardedInterstitialAd.Load(ID, adRequest,
    //        (RewardedInterstitialAd ad, LoadAdError error) =>
    //        {
    //            // if error is not null, the load request failed.
    //            if (error != null || ad == null)
    //            {
    //                Debug.LogError("rewarded interstitial ad failed to load an ad " +
    //                               "with error : " + error);
    //                LoadMoreRewardInter(_adUnitIdLow);
    //                return;
    //            }
    //            Debug.Log("Rewarded interstitial ad loaded with response : "
    //                      + ad.GetResponseInfo());

    //            rewardedInterstitialAd = ad;
    //            InitRegister(ad);
    //        });
    //}
    private UnityAction actionRewardInter;
    private UnityAction actionCloseRewardInter;
    private bool isRewardInter;
    public void ShowRewardedInterstitialAd(UnityAction actionReward, UnityAction actionNotLoadedVideo, UnityAction actionClose, ActionWatchVideo actionType)
    {
        const string rewardMsg =
            "Rewarded interstitial ad rewarded the user. Type: {0}, amount: {1}.";
        if (actionCloseRewardInter != null)
            actionCloseRewardInter = null;
        if (actionRewardInter != null)
            actionRewardInter = null;
        if (actionClose != null)
            actionCloseRewardInter = actionClose;
        if (rewardedInterstitialAd != null)
        {
            rewardedInterstitialAd.Show((Reward reward) =>
            {

                actionRewardInter = actionReward;
                Debug.Log("=========show");
                isRewardInter = true;
                GameController.Instance.AnalyticsController.LogWatchVideo(actionType, true, true, "");
                // TODO: Reward the user.
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
                // actionReward.Invoke();
            });
        }
        else
        {
            actionNotLoadedVideo.Invoke();
            LoadRewardedInterstitialAd();
        }
    }
    private void HandleAdPaidEvent(object sender, AdValueEventArgs e)
    {
        AdValue adValue = e.AdValue;
        // send ad revenue info to Adjust
        AdjustAdRevenue adRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAdMob);
        adRevenue.setRevenue(adValue.Value / 1000000f, adValue.CurrencyCode);
        Adjust.trackAdRevenue(adRevenue);
    }
    void InitRegister(RewardedInterstitialAd ad)
    {
        ad.OnAdDidDismissFullScreenContent += OnAdContentclose;
    }

    private void OnAdContentclose(object sender, EventArgs e)
    {
        Debug.Log("=======close");
        Invoke("CloseAdReward", 0.1f);
        if (actionCloseRewardInter != null)
            actionCloseRewardInter.Invoke();
        LoadRewardedInterstitialAd();
    }
    void CloseAdReward()
    {
        if (isRewardInter)
            if (actionRewardInter != null)
            {
                actionRewardInter.Invoke();
                actionRewardInter = null;
            }
        isRewardInter = false;
    }

    #endregion
    private void Update()
    {
        countdownAds += Time.deltaTime;
    }
}
