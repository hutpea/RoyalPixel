using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupClaimVip : BaseBox
{
    private static PopupClaimVip instance;
    public static PopupClaimVip Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupClaimVip>(PathPrefabs.POPUP_CLAIM_VIP));
        }
        instance.Init();
        return instance;
    }
    private void Init()
    {
        GameData.SetDateTimeReciveGift(UnbiasedTime.Instance.Now());
        HomeController.instance.SetTextItem();
    }
}
