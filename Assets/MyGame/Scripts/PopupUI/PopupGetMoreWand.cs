using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupGetMoreWand : BaseBox
{
    private static PopupGetMoreWand instance;
    public int star = 2;
    [SerializeField] Transform btnClaim;
    [SerializeField] GameObject btnGoVip;
    public static PopupGetMoreWand Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupGetMoreWand>(PathPrefabs.POPUP_GET_MORE_WAND));
        }
        instance.InitPopup();
        return instance;
    }
    void InitPopup()
    {
        if (GameData.BuyPackIAP(PackIAP.SUBSCRIPTION))
            btnGoVip.SetActive(false);
        else
            btnGoVip.SetActive(true);
    }
    public void GetMore()
    {
//#if UNITY_EDITOR
//        Success();
//#endif
        GameController.Instance.admobAds.ShowVideoReward(Success, ShowFail, null, ActionWatchVideo.GetMoreStar, GameData.picChoice.Id);
    }
    void Success()
    {
        Close();
        GameData.ItemStar += star;
        GameController.Instance.moneyEffectController.SpawnEffect_FlyUp(transform.position, Item.Star, star.ToString(), Color.yellow);
        UIGameController.instance.SetTotalStar();
    }
    void ShowFail()
    {
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
    (
       btnClaim.transform.position,
        "Video is not available!",
        Color.red,
        isSpawnItemPlayer: true
    );
    }
    public void ShowVip()
    {
        PopupVip.Setup().Show();
    }    
}
