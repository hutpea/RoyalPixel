using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Firebase.Analytics;
using Firebase;
using Facebook.Unity;
using Firebase.Analytics;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
using com.adjust.sdk;

public class AnalyticsController : MonoBehaviour
{
    #region Init
    static UnityEvent onFinishFirebaseInit = new UnityEvent();
    private static bool m_firebaseInitialized = false;
    public static bool firebaseInitialized
    {
        get
        {
            return m_firebaseInitialized;
        }
        set
        {
            m_firebaseInitialized = value;
            if (value == true)
            {
                if (onFinishFirebaseInit != null)
                {
                    onFinishFirebaseInit.Invoke();
                    onFinishFirebaseInit.RemoveAllListeners();
                }

                //SetUserProperties();
            }
        }
    }
    #endregion

    public const string aj_inters_ad_eligible = "lvghbs";
    public const string aj_inters_api_called = "dacwpr";
    public const string aj_inters_displayed = "xwf3w4";
    public const string aj_level_complete = "dbltlf";
    public const string aj_purchase = "lizm6f";
    public const string aj_rewarded_ad_completed = "fzna67";
    public const string aj_rewarded_ad_eligible = "nkewwc";
    public const string aj_rewarded_api_called = "wwnq9m";
    public const string aj_rewarded_displayed = "yj9npp";
    public const string aj_tutorial_completion = "qksj8v";

    private static void LogBuyInappAdjust(string inappID, string trancstionID)
    {

    }

    public static void LogEventFirebase(string eventName, Parameter[] parameters)
    {

        if (firebaseInitialized)
        {

            FirebaseAnalytics.LogEvent(eventName, parameters);
        }
        else
        {
            onFinishFirebaseInit.AddListener(() =>
            {
                FirebaseAnalytics.LogEvent(eventName, parameters);
            });
        }
    }

    public static void LogEventFacebook(string eventName, Dictionary<string, object> parameters)
    {
        if (FB.IsInitialized)
        {
#if !ENV_PROD
            parameters["test"] = true;
#endif
            FB.LogAppEvent(eventName, null, parameters);
        }
    }
    public static void LogSelectArea(string idArea)
    {
        try
        {
            if (firebaseInitialized)
            {
                Parameter[] parameters = new Parameter[1]
      {
            new Parameter("select_area", idArea.ToString())
      };
                FirebaseAnalytics.LogEvent("select_area", parameters);
            }
        }
        catch
        {

        }

    }
    public static void LogComplete(string id)
    {
        try
        {
            if (firebaseInitialized)
            {
                Parameter[] parameters = new Parameter[1]
      {
            new Parameter("pic_id", id.ToString())
      };
                FirebaseAnalytics.LogEvent("complete_pic", parameters);
            }
        }
        catch
        {

        }

        try
        {
            AdjustEvent adjustEvent = new AdjustEvent(aj_inters_displayed);
            Adjust.trackEvent(adjustEvent);
        }
        catch
        {

        }
    }
    public static void LogSelectPic(string id)
    {
        try
        {
            if (firebaseInitialized)
            {
                Parameter[] parameters = new Parameter[1]
      {
            new Parameter("pic_id", id.ToString())
      };
                FirebaseAnalytics.LogEvent("select_pic", parameters);
            }
        }
        catch
        {

        }
    }
    public static void LogClickVIP(string id = "")
    {
        try
        {
            if (firebaseInitialized)
            {
                Parameter[] parameters = new Parameter[1]
      {
            new Parameter("select_iap", id.ToString())
      };
                FirebaseAnalytics.LogEvent("select_iap", parameters);
            }
        }
        catch
        {

        }
    }
    public static void SetUserProperties()
    {
        if (!firebaseInitialized) return;

        FirebaseAnalytics.SetUserProperty(StringHelper.RETENTION_D, UseProfile.RetentionD.ToString());
        FirebaseAnalytics.SetUserProperty(StringHelper.DAYS_PLAYED, UseProfile.DaysPlayed.ToString());
        FirebaseAnalytics.SetUserProperty(StringHelper.PAYING_TYPE, UseProfile.PayingType.ToString());
        FirebaseAnalytics.SetUserProperty(StringHelper.LEVEL, UseProfile.CurrentLevel.ToString());
    }

    #region Event
    public void LogWatchVideo(ActionWatchVideo action, bool isHasVideo, bool isHasInternet, string level)
    {
        if (!firebaseInitialized) return;
        Parameter[] parameters = new Parameter[4]
        {
            new Parameter("actionWatch", action.ToString()) ,
             new Parameter("has_ads", isHasVideo.ToString()) ,
              new Parameter("has_internet", isHasInternet.ToString()) ,
               new Parameter("level", level)
        };

        FirebaseAnalytics.LogEvent("watch_video_game", parameters);
        LogVideoRewardShowDone(action.ToString());
    }
    public static void LogUseItem(Item item, string level = "")
    {
        if (!firebaseInitialized) return;
        Parameter[] parameters = new Parameter[2]
        {
            new Parameter("item", item.ToString()) ,
             new Parameter("pic_id", level.ToString()) ,
        };

        FirebaseAnalytics.LogEvent("use_item", parameters);
    }
    public void LogWatchInter(string action, bool isHasVideo, bool isHasInternet, string level)
    {
        if (!firebaseInitialized) return;
        Parameter[] parameters = new Parameter[4]
        {
            new Parameter("actionWatch", action.ToString()) ,
             new Parameter("has_ads", isHasVideo.ToString()) ,
              new Parameter("has_internet", isHasInternet.ToString()) ,
              new Parameter("level", level)
        };

        FirebaseAnalytics.LogEvent("show_inter", parameters);
    }

    public static void LogBuyInapp(string inappID, string trancstionID)
    {
        try
        {
            LogBuyInappAdjust(inappID, trancstionID);
        }
        catch
        {

        }
        try
        {
            if (firebaseInitialized)
            {
                Parameter[] parameters = new Parameter[1]
                {
                new Parameter("id", inappID),
                };
                LogEventFirebase("inapp_event", parameters);
            }
        }
        catch
        {

        }
    }

    public void LogStartLevel(int level)
    {
        try
        {
            if (!firebaseInitialized) return;

            Parameter[] parameters = new Parameter[1]
            {
            new Parameter("level", level.ToString())
            };


            FirebaseAnalytics.LogEvent("level_start", parameters);
        }
        catch
        {

        }
    }

    public void LogLevelComplete(string level)
    {
        try
        {
            if (firebaseInitialized)
            {
                Parameter[] parameters = new Parameter[1]
           {
            new Parameter("level", level.ToString())
           };


                FirebaseAnalytics.LogEvent("level_complete", parameters);
            }
        }
        catch
        {

        }

        try
        {
            AdjustEvent adjustEvent = new AdjustEvent(aj_level_complete);
            adjustEvent.addCallbackParameter("level", level.ToString());
            Adjust.trackEvent(adjustEvent);
        }
        catch
        {

        }
    }

    public void LogLevelFail(int level)
    {
        if (!firebaseInitialized) return;
        Parameter[] parameters = new Parameter[1]
       {
            new Parameter("level", level.ToString())
       };


        FirebaseAnalytics.LogEvent("level_fail", parameters);
    }

    public void LogRequestVideoReward(string placement)
    {
        try
        {
            if (firebaseInitialized)
            {
                Parameter[] parameters = new Parameter[1]
               {
            new Parameter("placement", placement.ToString())
               };


                FirebaseAnalytics.LogEvent("ads_reward_offer", parameters);
            }
        }
        catch
        {

        }
    }

    public void LogVideoRewardEligible()
    {
        try
        {
            AdjustEvent adjustEvent = new AdjustEvent(aj_rewarded_ad_eligible);
            Adjust.trackEvent(adjustEvent);
        }
        catch
        {

        }
    }

    public void LogClickToVideoReward(string placement)
    {
        if (!firebaseInitialized) return;
        Parameter[] parameters = new Parameter[1]
       {
            new Parameter("placement", placement.ToString())
       };


        FirebaseAnalytics.LogEvent("ads_reward_click", parameters);
    }

    public void LogVideoRewardShow(string placement)
    {
        try
        {
            if (firebaseInitialized)
            {
                Parameter[] parameters = new Parameter[1]
               {
            new Parameter("placement", placement.ToString())
               };


                FirebaseAnalytics.LogEvent("ads_reward_show", parameters);
            }
        }
        catch
        {

        }

        try
        {
            AdjustEvent adjustEvent = new AdjustEvent(aj_rewarded_displayed);
            Adjust.trackEvent(adjustEvent);
        }
        catch
        {

        }
    }

    public void LogVideoRewardLoadFail(string placement, string errormsg)
    {
        if (!firebaseInitialized) return;
        Parameter[] parameters = new Parameter[2]
       {
            new Parameter("placement", placement.ToString()),
            new Parameter("errormsg", errormsg.ToString())
       };


        FirebaseAnalytics.LogEvent("ads_reward_fail", parameters);
    }

    public void LogVideoRewardShowDone(string placement)
    {
        try
        {
            if (firebaseInitialized)
            {
                Parameter[] parameters = new Parameter[1]
               {
            new Parameter("placement", placement.ToString()),
               };


                FirebaseAnalytics.LogEvent("ads_reward_complete", parameters);
            }
        }
        catch
        {

        }

        try
        {
            AdjustEvent adjustEvent = new AdjustEvent(aj_rewarded_ad_completed);
            Adjust.trackEvent(adjustEvent);
        }
        catch
        {

        }
    }

    public void LogInterLoadFail(string errormsg)
    {
        if (!firebaseInitialized) return;
        Parameter[] parameters = new Parameter[1]
       {
            new Parameter("errormsg", errormsg.ToString())
       };


        FirebaseAnalytics.LogEvent("ad_inter_fail", parameters);
    }

    public void LogInterLoad()
    {
        try
        {
            if (firebaseInitialized)
                FirebaseAnalytics.LogEvent("ad_inter_load");
        }
        catch
        {

        }


    }

    public void LoadInterEligible()
    {
        try
        {
            AdjustEvent adjustEvent = new AdjustEvent(aj_inters_ad_eligible);
            Adjust.trackEvent(adjustEvent);
        }
        catch
        {

        }
    }

    public void LogInterShow(string actionWatchLog)
    {
        try
        {
            if (firebaseInitialized)
            {
                bool isHasInternet = Application.internetReachability != NetworkReachability.NotReachable;
                Parameter[] parameters = new Parameter[2]
            {
                new Parameter("action_watch", actionWatchLog.ToString()),
                new Parameter("has_internet", isHasInternet.ToString())
            };
                FirebaseAnalytics.LogEvent("ad_inter_show", parameters);
            }
        }
        catch
        {

        }

        try
        {
            AdjustEvent adjustEvent = new AdjustEvent(aj_inters_displayed);
            Adjust.trackEvent(adjustEvent);
        }
        catch
        {

        }
    }

    public void LogInterClick()
    {
        if (!firebaseInitialized) return;
        FirebaseAnalytics.LogEvent("ad_inter_click");
    }

    public void LogInterReady()
    {
        try
        {
            AdjustEvent adjustEvent = new AdjustEvent(aj_inters_api_called);
            Adjust.trackEvent(adjustEvent);
        }
        catch
        {

        }
    }

    public void LogVideoRewardReady()
    {
        try
        {
            AdjustEvent adjustEvent = new AdjustEvent(aj_rewarded_api_called);
            Adjust.trackEvent(adjustEvent);
        }
        catch
        {

        }
    }

    public void LogTutLevelStart(int level)
    {
        try
        {
            if (firebaseInitialized)
                FirebaseAnalytics.LogEvent(string.Format("tutorial_start_{0}", level));

        }
        catch
        {


        }
    }

    public void LogTutLevelEnd(int level)
    {
        try
        {
            if (firebaseInitialized)
                FirebaseAnalytics.LogEvent(string.Format("tutorial_end_{0}", level));

        }
        catch
        {

        }

        try
        {
            AdjustEvent adjustEvent = new AdjustEvent(aj_tutorial_completion);
            adjustEvent.addCallbackParameter("level", level.ToString());
            adjustEvent.addCallbackParameter("tutorial_id", level.ToString());
            Adjust.trackEvent(adjustEvent);
        }
        catch
        {

        }
    }

    public static void LogIAP(int level, string productID, string price, string currency)
    {
        try
        {
            AdjustEvent adjustEvent = new AdjustEvent(aj_purchase);
            adjustEvent.addCallbackParameter("level", level.ToString());
            adjustEvent.addCallbackParameter("productID", productID.ToString());
            adjustEvent.addCallbackParameter("price", price.ToString());
            adjustEvent.addCallbackParameter("currency", currency.ToString());
            Adjust.trackEvent(adjustEvent);
        }
        catch
        {

        }
    }
    #endregion

    private void OnApplicationQuit()
    {
        SetUserProperties();
    }
}

public enum ActionClick
{
    None = 0,
    Play = 1,
    Rate = 2,
    Share = 3,
    Policy = 4,
    Feedback = 5,
    Term = 6,
    NoAds = 10,
    Settings = 11,
    ReplayLevel = 12,
    SkipLevel = 13,
    Return = 14,
    BuyStand = 15
}

public enum ActionWatchVideo
{
    None = 0,
    Skip_level = 1,
    Return = 2,
    AddItemStar = 3,
    AddItemBomb = 4,
    ClaimSkin = 5,
    FinishPic = 6,
    UsePicCamera = 7,
    SavePic = 8,
    Claimx2 = 9,
    UnlockPic = 10,
    ClaimDaily = 11,
    PlayHome = 12,
    SelectPicInArea = 13,
    GetMoreBomb = 14,
    GetMoreStar = 15,
    UnlockAreaEvent = 16,
    AddItemFind = 17,
    RemoveLogo = 18,
    UnlockNoel = 19,
    AddItemPen = 20,
    AddMorePen = 21,
    SpinADS = 22,
    Claimx2Spin = 23,
    SelectBox = 24,
    UnlockPicEvent = 25,
    BuyGem = 26,
    AutoClaim = 27,
    ClaimAdComplete = 28,
    NoThanksClaimComplete = 29
}

public enum ActionShowInter
{
    None = 0,
    Skip_level = 1,
    Return = 2,
    BuyStand = 3,

    EndGame = 4,
    Click_Setting = 5,
    Click_Replay = 6,
    SelectPic = 7
}
