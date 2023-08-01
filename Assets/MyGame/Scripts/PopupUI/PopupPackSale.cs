using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupPackSale : BaseBox
{
    private static PopupPackSale instance;
    [SerializeField] GameObject loading;
    [SerializeField] DataGift data;
    [SerializeField] Button btnBuy;
    public static PopupPackSale Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupPackSale>(PathPrefabs.POPUP_PACK_SALE));
        }
        return instance;
    }
    public void ClickBuy()
    {
        AnalyticsController.LogClickVIP("pack_sale");
        loading.SetActive(true);
    }
    public void ClickBuyFail()
    {
        loading.SetActive(false);
    }
    public void BuyIAP()
    {
        loading.SetActive(false);
        GameData.ItemBomb += data.gifts[0].itemBomb;
        GameData.ItemStar += data.gifts[0].itemStar;
        GameData.ItemPen += data.gifts[0].itemPen;
        GameData.ItemFind += data.gifts[0].itemFind;
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp(btnBuy.transform.position, "BUY COMPLETE", Color.green);
        if (UIGameController.instance != null)
        {
            UIGameController.instance.SetTextItem();
        }
        GameData.SetPackIAP(PackIAP.SALE);
        StartCoroutine(Helper.StartAction(() => Close(), 1.5f));
        GameController.Instance.musicManager.PlayProgress();
    }
}
