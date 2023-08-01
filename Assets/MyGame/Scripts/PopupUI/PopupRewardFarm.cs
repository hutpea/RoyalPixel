using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class PopupRewardFarm : BaseBox
{
    private const float giftRectY = 142.5f;

    private static PopupRewardFarm instance;
    [SerializeField] RectTransform giftRect;
    [SerializeField] Image icon;
    [SerializeField] Text txtAmount;

    private RewardDatabase.Reward reward;
    [SerializeField] Text descLabel;
    [SerializeField] Button bgBtn;
    [SerializeField] Button btnOpenChest;
    [SerializeField] Button btnOk;
    //[SerializeField] Button btnNoThanks;
    //[SerializeField] Button btnClaimX2;
    private SkeletonGraphic skeletonGraphic;

    public static PopupRewardFarm Setup(RewardDatabase.Reward reward)
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupRewardFarm>(PathPrefabs.POPUP_REWARD_FARM));
        }

        instance.reward = reward;
        GameController.Instance.musicManager.PlayWinSound();
        Debug.Log("=========== Show: PopupRewardFarm");
        return instance;
    }

    private void Start()
    {
        btnOpenChest.onClick.AddListener(delegate { OpenChest(1); });
        btnOk.onClick.AddListener(delegate { Close(); });
        //btnNoThanks.onClick.AddListener(delegate { Close(); });
        //btnClaimX2.onClick.AddListener(delegate { OnClickX2Btn(); });
    }

    public void OnShow()
    {
        base.Show();
        descLabel.text = "OPEN YOUR FARM CHEST!";
        btnOk.gameObject.SetActive(false);
        bgBtn.interactable = false;
        btnOpenChest.interactable = true;
        //btnNoThanks.gameObject.SetActive(false);
        //btnClaimX2.gameObject.SetActive(false);
        icon.gameObject.SetActive(false);
        txtAmount.gameObject.SetActive(false);
        giftRect.anchoredPosition = new Vector2(giftRect.anchoredPosition.x, giftRectY);
        giftRect.GetComponent<CanvasGroup>().alpha = 1f;
        skeletonGraphic = btnOpenChest.GetComponent<SkeletonGraphic>();
        TrackEntry track0 = skeletonGraphic.AnimationState.SetAnimation(0, "anim", true);
    }

    public void OpenChest(int multipler = 1)
    {
        btnOpenChest.interactable = false;
        //btnNoThanks.gameObject.SetActive(true);
        //btnClaimX2.gameObject.SetActive(true);
        TrackEntry track1 = skeletonGraphic.AnimationState.SetAnimation(0, "anim1", false);
        track1.Complete += delegate(TrackEntry entry)
        {
            descLabel.text = "";
            icon.gameObject.SetActive(true);
            txtAmount.gameObject.SetActive(true);

            if (reward.item == Item.FreeFind || reward.item == Item.FreePen)
            {
                if (reward.item == Item.FreeFind)
                {
                    txtAmount.text = "1 hour";
                }
                else
                    txtAmount.text = "15 minutes";
            }
            else
            {
                int amount = reward.amount;
                txtAmount.text = amount.ToString();
            }

            if (GameController.Instance.dataContain.giftDatabase.GetGift(reward.item, out Gift gift))
            {
                icon.sprite = gift.getGiftSprite;
                icon.gameObject.SetActive(true);
            }
            else
            {
                icon.sprite = null;
                icon.gameObject.SetActive(false);
            }

            var track2 = skeletonGraphic.AnimationState.SetAnimation(0, "anim2", true);

            PopupEventFarmBuilding.instance.UpdateUI();
            bgBtn.interactable = true;
            btnOk.gameObject.SetActive(true);
        };
    }

    private void OnClickX2Btn()
    {
        GameController.Instance.admobAds.ShowVideoReward(delegate
        {
            descLabel.text = "YOU RECEIVED";
            bgBtn.interactable = true;
            //btnNoThanks.gameObject.SetActive(false);
            //btnClaimX2.gameObject.SetActive(false);
            RewardDatabase.Claim(reward, isShowPopup: false);
            txtAmount.text = reward.amount.ToString() + "x2";
            /*giftRect.DOLocalMoveY(giftRectY + 85, 0.5f).SetEase(Ease.Linear).OnComplete(delegate
            {
                giftRect.GetComponent<CanvasGroup>().DOFade(0f, 0.5f).SetDelay(0.65f);
            });*/
            PopupEventFarmBuilding.instance.UpdateUI();
        }, ShowFail, null, ActionWatchVideo.Claimx2, "null");
    }

    private void ShowFail()
    {
        /*GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
        (
            btnClaimX2.transform.position,
            "No Video Ads",
            Color.red,
            isSpawnItemPlayer: true
        );*/
    }
}