using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupRemoveAdsPlus : BaseBox
{
    private static PopupRemoveAdsPlus instance;
    public GameObject loading;
    [SerializeField] Transform btnBuy;
    public static PopupRemoveAdsPlus Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupRemoveAdsPlus>(PathPrefabs.POPUP_REMOVE_ADS_PLUS));
        }
        //ChickenDataManager.CountTillShowRate = 0;
        return instance;
    }
    public void StartBuy()
    {
        loading.SetActive(true);
    }
    public void BuySuccess()
    {
        UseProfile.IsRemoveAds = true;
        GameData.Gem += 100;
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
  (
     btnBuy.position,
      "Buy Complete!",
      Color.green,
      isSpawnItemPlayer: true
  );
        loading.SetActive(false);
        Invoke("Close", 1);
    }
    public void BuyCancel()
    {
        loading.SetActive(false);
    }
}
