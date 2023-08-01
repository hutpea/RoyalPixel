using com.adjust.sdk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;


public class AdsController : MonoBehaviour
{
    private UnityAction onInterstitialClosed;
    private UnityAction onInterstitialShowFailed;

    private UnityAction onRewardedClosed;
    private UnityAction onRewaredShowFailed;

    private bool gotRewarded;
    private bool isShownBanner;
    private bool loadBannerCalled;

    private const string IronsouceAppID = Config.IRONSOURCE_DEV_KEY;
    bool closeReward;

    public void Init()
    {
        try
        {
            IronSource.Agent.setMetaData("is_child_directed", "false");
            IronSource.Agent.shouldTrackNetworkState(true);
        }
        catch
        {

        }

        IronSource.Agent.init(Config.IRONSOURCE_DEV_KEY, IronSourceAdUnits.BANNER, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.REWARDED_VIDEO);
        IronSourceEvents.onInterstitialAdClosedEvent += IronSourceEvents_onInterstitialAdClosedEvent;
        IronSourceEvents.onInterstitialAdLoadFailedEvent += IronSourceEvents_onInterstitialAdLoadFailedEvent;
        IronSourceEvents.onInterstitialAdShowFailedEvent += IronSourceEvents_onInterstitialAdShowFailedEvent;
        IronSourceEvents.onInterstitialAdShowSucceededEvent += IronSourceEvents_onInterstitialAdShowSucceededEvent;

        IronSourceEvents.onRewardedVideoAdClosedEvent += IronSourceEvents_onRewardedVideoAdClosedEvent;
        IronSourceEvents.onRewardedVideoAdShowFailedEvent += IronSourceEvents_onRewardedVideoAdShowFailedEvent;
        IronSourceEvents.onRewardedVideoAdRewardedEvent += IronSourceEvents_onRewardedVideoAdRewardedEvent;


        IronSourceEvents.onBannerAdLoadFailedEvent += OnBannerLoadFail;
        IronSourceEvents.onBannerAdLoadedEvent += OnBannerLoaded;
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
        // Load banner & interstitial
        loadInterstitial();
        //#if !ENV_PROD
        IronSource.Agent.validateIntegration();
        //#endif

        IronSourceEvents.onImpressionDataReadyEvent += ImpressionSuccessEvent;
#if ENV_LOG
Debug.Log("AdsController initialized");
#endif
    }


    private void IronSourceEvents_onInterstitialAdShowSucceededEvent()
    {

    }

    private void IronSourceEvents_onRewardedVideoAdRewardedEvent(IronSourcePlacement obj)
    {
        closeReward = true;
        // GameController.Instance.dataContains.questDatabase.UpdateQuest(QuestEnum.Watch_X_Video, 1);
    }

    private void IronSourceEvents_onRewardedVideoAdShowFailedEvent(IronSourceError obj)
    {
        onRewaredShowFailed?.Invoke();
    }

    private void IronSourceEvents_onRewardedVideoAdClosedEvent()
    {
        //InvokeRewardedClosedLater();
        if (closeReward)
        {
            if (onRewardedClosed != null)
                onRewardedClosed();
            closeReward = false;
        }
    }

    private void IronSourceEvents_onInterstitialAdShowFailedEvent(IronSourceError obj)
    {
        Time.timeScale = 1;
        onInterstitialShowFailed?.Invoke();
    }

    private void IronSourceEvents_onInterstitialAdLoadFailedEvent(IronSourceError obj)
    {

    }

    private void IronSourceEvents_onInterstitialAdClosedEvent()
    {
        loadInterstitial();
        onInterstitialClosed?.Invoke();
    }

    //public static AdsController Instance
    //{
    //    //get
    //    //{
    //    //    if (instance == null)
    //    //    {
    //    //        Init();
    //    //    }
    //    //    return instance;
    //    //}
    //}

    public bool IsInterstitialAvailable
    {
        get
        {
            bool ret = IronSource.Agent.isInterstitialReady();
            if (!ret)
            {
                loadInterstitial();
            }
            return ret;
        }
    }


    public bool CanShowInterstitial
    {
        get
        {
            return IsInterstitialAvailable;
        }
    }

    public void loadInterstitial()
    {
        if (!IronSource.Agent.isInterstitialReady())
            IronSource.Agent.loadInterstitial();
    }

    public void ShowInterstitial(UnityAction interstitialShowFailedAction, UnityAction interstitialClosedAction)
    {
        this.onInterstitialClosed = interstitialClosedAction;
        this.onInterstitialShowFailed = interstitialShowFailedAction;
        this.onRewaredShowFailed = null;
        this.onRewardedClosed = null;
        if (!IronSource.Agent.isInterstitialReady())
        {
            this.onInterstitialShowFailed?.Invoke();
            Time.timeScale = 1;
        }
        else
        {
            if (GamePlayControl.Instance != null)
            {
                GamePlayControl.Instance.ExitButtonPen();
            }
            IronSource.Agent.showInterstitial();
        }
    }

    public bool IsRewardedVideoAvailable
    {
        get
        {
            return IronSource.Agent.isRewardedVideoAvailable();
        }
    }

    public bool ShowRewardedVideo(UnityAction rewardedShowFailedAction, UnityAction rewardedClosedAction)
    {
        this.onRewaredShowFailed = rewardedShowFailedAction;
        this.onRewardedClosed = rewardedClosedAction;
        this.onInterstitialClosed = rewardedClosedAction;
        this.onInterstitialShowFailed = rewardedShowFailedAction;
        this.gotRewarded = false;
        if (!IronSource.Agent.isRewardedVideoAvailable())
        {
            if (IronSource.Agent.isInterstitialReady())
            {
                //GameController.Instance.dataContains.questDatabase.UpdateQuest(QuestEnum.Watch_X_Video, 1);
                IronSource.Agent.showInterstitial();
                return true;
            }
            else
            {
                this.onRewaredShowFailed?.Invoke();
                return false;
            }

        }
        else
        {
            IronSource.Agent.showRewardedVideo();
            return true;
        }
    }

    public void ShowBanner()
    {
        if (UseProfile.IsRemoveAds || UseProfile.IsVip)
            return;

        IronSource.Agent.displayBanner();
    }

    public void HideBanner()
    {
        isShownBanner = false;
        //if (IronSource.Agent.IsBa)
        IronSource.Agent.hideBanner();
    }

    private IEnumerator reloadBannerCoru;
    public void OnBannerLoadFail(IronSourceError err)
    {
        if (reloadBannerCoru != null)
        {
            StopCoroutine(reloadBannerCoru);
            reloadBannerCoru = null;
        }

        reloadBannerCoru = Helper.StartAction(() => { IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM); }, 0.3f);
        StartCoroutine(reloadBannerCoru);
    }

    public void OnBannerLoaded()
    {
        if (reloadBannerCoru != null)
        {
            StopCoroutine(reloadBannerCoru);
            reloadBannerCoru = null;
        }

        //ShowBanner();
    }

    //private async void InvokeRewardedClosedLater()
    //{
    //    await Task.Delay(10);
    //    //onRewardedClosed?.Invoke(gotRewarded);  
    //}

    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }

    private void ImpressionSuccessEvent(IronSourceImpressionData impressionData)
    {
        var adRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceIronSource);
        adRevenue.setRevenue(impressionData.revenue.Value, "USD");
        adRevenue.setAdRevenueNetwork(impressionData.adNetwork);
        adRevenue.setAdRevenueUnit(impressionData.adUnit);
        adRevenue.setAdRevenuePlacement(impressionData.placement);
        Adjust.trackAdRevenue(adRevenue);
    }
}

