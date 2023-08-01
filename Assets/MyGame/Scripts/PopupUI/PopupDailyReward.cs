using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupDailyReward : BaseBox
{
    public static PopupDailyReward instance;
    [SerializeField] Button btnClaim;
    [SerializeField] Button btnNoThanks;
    [SerializeField] int value;
    public static PopupDailyReward Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupDailyReward>(PathPrefabs.POPUP_DAILY_REWARD));
        }
        return instance;
    }
    public void ClaimItem()
    {
        //#if UNITY_EDITOR
        //        GetReward(2);
        //#endif
        GameController.Instance.admobAds.ShowVideoReward(() => GetReward(2), () => NotLoadVideo(), () => { }, ActionWatchVideo.ClaimDaily, "null");
    }
    public void GetReward(int number)
    {
        Vector2 pos = new Vector2(btnClaim.transform.position.x + 0.7f, btnClaim.transform.position.y);
        GameController.Instance.moneyEffectController.SpawnEffect_FlyUp(pos, Item.Gem, (value * number).ToString(), Color.black);
        ClaimGem(value * number);
        btnClaim.interactable = false;
        btnNoThanks.interactable = false;
    }
    void ClaimGem(int value)
    {
        GameData.Gem += value;
        HomeController.instance.SetTextItem();
        Close();
        GameData.SetDateTimeReciveDailyGift(UnbiasedTime.Instance.Now());
    }
    void ClaimStar(int value)
    {
        GameData.ItemStar += value;
        HomeController.instance.SetTextItem();
    }
    public void NotLoadVideo()
    {
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
   (
      btnClaim.transform.position,
       "No Video Ads",
       Color.red,
       isSpawnItemPlayer: true
   );
    }
    public override void Close()
    {
        base.Close();
        //bool canShow = RemoteConfigController.GetBoolConfig(FirebaseConfig.CAN_SHOW_POPUP, true);
        //if (canShow)
        //    PopupNotiEvent.Setup().Show();
    }
}
