using EventDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupSuggestAdsWand : BaseBox
{
    private static PopupSuggestAdsWand instance;
    [SerializeField] Transform btnGetMore;
    [SerializeField] GameObject btnGoVip;
    [SerializeField] GameObject btnGem;
    public static PopupSuggestAdsWand Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupSuggestAdsWand>(PathPrefabs.POPUP_SUGGEST_ITEM_WANDS));
        }
        instance.InitPopup();
        //ChickenDataManager.CountTillShowRate = 0;
        return instance;
    }
    void InitPopup()
    {
        if (GameData.BuyPackIAP(PackIAP.SUBSCRIPTION))
            btnGoVip.SetActive(false);
        else
        {
            if (GameData.Gem >= 6)
            {
                btnGem.SetActive(true);
                btnGoVip.SetActive(false);
            }
            else
            {
                btnGem.SetActive(false);
                btnGoVip.SetActive(true);
            }
        }
    }
    public void GoVip()
    {
        PopupVip.Setup().Show();
    }
    public void GetItemByGem()
    {
        GameData.ItemStar += 1;
        this.PostEvent(EventID.CHANGE_VALUE_ITEM_WAND);
        GameController.Instance.moneyEffectController.SpawnEffect_FlyUp(btnGem.transform.position, Item.Star, "+1", Color.black);
        if (GameData.Gem >= 6)
            GameData.Gem -= 6;
        else
        {
            InitPopup();
        }
        this.PostEvent(EventID.CHANGE_VALUE_GEM);
    }
    public void GetItem()
    {
        //#if UNITY_EDITOR
        //        GameData.ItemStar += 3;
        //        this.PostEvent(EventID.CHANGE_VALUE_ITEM_WAND);
        //#endif
        GameController.Instance.admobAds.ShowVideoReward(() =>
        {
            GameData.ItemStar += 1;
            this.PostEvent(EventID.CHANGE_VALUE_ITEM_WAND);
            Close();
            PopupGetMoreWand.Setup().Show();
        },
        FailToLoad, null, ActionWatchVideo.AddItemStar, "Popup_suggest");
    }
    public void FailToLoad()
    {
        //StartCoroutine(Helper.StartAction(() =>
        //{
        Debug.Log("close");
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
    (
       btnGetMore.transform.position,
        "Video is not available!",
        Color.red,
        isSpawnItemPlayer: true
    );
        // }, 0.5f));
    }
}
