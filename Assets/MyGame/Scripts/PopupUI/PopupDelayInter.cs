using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupDelayInter : BaseBox
{
    private static PopupDelayInter instance;
    int number;
    public static PopupDelayInter Setup(int numberGift)
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupDelayInter>(PathPrefabs.POPUP_DELAY_INTER));
        }
        instance.number = numberGift;
        //ChickenDataManager.CountTillShowRate = 0;
        return instance;
    }
    public override void Show()
    {
        base.Show();
        StartCoroutine(Helper.StartAction(() =>
        {
            GameController.Instance.admobAds.ShowInterstitial(false, "in_game_80%", () => { UIGameController.instance.ReciveGift(number); });
            Close();
        }, 0.5f));

    }
}
