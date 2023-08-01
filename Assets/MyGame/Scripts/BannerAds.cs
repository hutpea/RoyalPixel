using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerAds : MonoBehaviour
{
    public bool showEnable;
    private void OnEnable()
    {
        if (showEnable)
        {
            GameController.Instance.admobAds.ShowBanner();
        }
        else
            GameController.Instance.admobAds.DestroyBanner();
    }
    private void OnDisable()
    {
        if (showEnable)
        {
            GameController.Instance.admobAds.DestroyBanner();
        }
        else
            GameController.Instance.admobAds.ShowBanner();
    }
}
