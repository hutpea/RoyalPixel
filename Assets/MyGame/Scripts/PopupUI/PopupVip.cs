using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class PopupVip : BaseBox
{
    private static PopupVip instance;
    [SerializeField] Button btnBuy;
    public static PopupVip Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupVip>(PathPrefabs.POPUP_VIP));
        }
        if (UseProfile.IsVip)
        {
            instance.btnBuy.interactable = false;
        }
        else
        {
            instance.btnBuy.interactable = true;
        }
        return instance;
    }
    protected override void OnEnable()
    {
        AnalyticsController.LogClickVIP("vip");
        base.OnEnable();
    }
    public void LinkPrivacy()
    {
        Application.OpenURL("https://sites.google.com/view/global-play-policy/");
    }
    public void LinkTerm()
    {
        Application.OpenURL("https://sites.google.com/view/global-play-terms-of-use");
    }
}
