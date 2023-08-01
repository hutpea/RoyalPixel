using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoAds : MonoBehaviour
{
    public string str;
    private void OnEnable()
    {
        if (UseProfile.IsRemoveAds || UseProfile.IsVip)
            gameObject.SetActive(false);
    }
    public void BuyNoAds()
    {
        AnalyticsController.LogClickVIP("complete_noads");
        UseProfile.IsRemoveAds = true;
        gameObject.SetActive(false);
        GameController.Instance.admobAds.DestroyBanner();
    }
    public void Click()
    {
        AnalyticsController.LogClickVIP(str);
    }
}
