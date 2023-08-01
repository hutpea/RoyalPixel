using EventDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupCompleteLevel : BaseBox
{
    private static PopupCompleteLevel instance;
    [SerializeField] Text txtAutoClaim;
    [SerializeField] Button btnClaim;
    [SerializeField] Button btnNoThank;
    Coroutine coroutine;
    public UnityAction OnCancelClaim;
    public static PopupCompleteLevel Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupCompleteLevel>(PathPrefabs.POPUP_COMPLETELEVEL));
        }
        return instance;
    }
    public override void Show()
    {
        base.Show();
        instance.InitPopup();
    }
    void InitPopup()
    {
        btnNoThank.interactable = true;
        btnClaim.interactable = true;
        txtAutoClaim.gameObject.SetActive(true);
        txtAutoClaim.text = "Auto claim in 3s";
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        coroutine = StartCoroutine(StartCountAutoClaim());
        claimClick = false;
    }
    bool claimClick;
    IEnumerator StartCountAutoClaim()
    {
        float timer = 3;
        while (timer > 0)
        {
            yield return new WaitForSeconds(0.8f);
            if (!claimClick)
            {
                timer--;
                txtAutoClaim.text = "Auto claim in " + timer + "s";
            }
        }
        txtAutoClaim.gameObject.SetActive(false);
        btnClaim.interactable = false;
        GameController.Instance.admobAds.ShowRewardedInterstitialAd(() => { ClaimAdsSuccess(20); }, NotLoadAdAutoClaim, CloseAction, ActionWatchVideo.AutoClaim);
    }
    public void ClaimAdClick()
    {
        claimClick = true;
        btnNoThank.interactable = false;
        btnClaim.interactable = false;
        GameController.Instance.admobAds.ShowVideoReward(() => { ClaimAdsSuccess(20); }, ShowFail, CloseAction, ActionWatchVideo.ClaimAdComplete, "");
    }
    public override void Close()
    {
        base.Close();
        StopCoroutine(coroutine);
    }
    void ClaimAdsSuccess(int value)
    {
        GameData.Gem += value;
        this.PostEvent(EventID.CHANGE_VALUE_GEM);
        GameController.Instance.moneyEffectController.SpawnEffect_FlyUp(btnClaim.transform.position, Item.Gem, "+" + value, Color.black);
        Invoke("Close", 0.2f);
    }
    public void CancelClaimAds()
    {
        claimClick = true;
        btnNoThank.interactable = false;
        btnClaim.interactable = false;
        GameController.Instance.admobAds.ShowInterstitial(false, "no_thanks", () => { ClaimAdsSuccess(6); }, GameData.picChoice.Id);
    }
    void NotLoadAdAutoClaim()
    {
        btnNoThank.interactable = true;
        btnClaim.interactable = true;
    }
    void ShowFail()
    {
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
   (
      btnClaim.transform.position,
       "No Video Ads",
       Color.red,
       isSpawnItemPlayer: true
   );
        btnNoThank.interactable = true;
        btnClaim.interactable = true;
        claimClick = false;
    }
    void CloseAction()
    {
        claimClick = false;
        btnNoThank.interactable = true;
        btnClaim.interactable = true;
    }
}
