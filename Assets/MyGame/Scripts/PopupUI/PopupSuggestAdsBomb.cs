using EventDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupSuggestAdsBomb : BaseBox
{
    private static PopupSuggestAdsBomb instance;
    [SerializeField] Transform btnGetMore;
    [SerializeField] GameObject btnGoVip;
    [SerializeField] GameObject btnGem;
    public static PopupSuggestAdsBomb Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupSuggestAdsBomb>(PathPrefabs.POPUP_SUGGEST_ITEM_BOMB));
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
        GameData.ItemBomb += 1;
        this.PostEvent(EventID.CHANGE_VALUE_ITEM_BOMB);
        GameController.Instance.moneyEffectController.SpawnEffect_FlyUp(btnGem.transform.position, Item.Bomb, "+1", Color.black);
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
        //        GameData.ItemBomb += 5;
        //        this.PostEvent(EventID.CHANGE_VALUE_ITEM_BOMB);
        //#endif

        GameController.Instance.admobAds.ShowVideoReward(() =>
        {
            GameData.ItemBomb += 1;
            this.PostEvent(EventID.CHANGE_VALUE_ITEM_BOMB);
            Close();
            PopupGetMoreBomb.Setup().Show();
        },
        FailToLoad, null, ActionWatchVideo.AddItemBomb, "Popup_suggest");
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
