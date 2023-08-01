using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupRewardSpin : BaseBox
{
    private static PopupRewardSpin instance;
    [SerializeField] Image icon;
    [SerializeField] Text txtAmount;
    private RewardDatabase.Reward reward;
    [SerializeField] Button btnSpin;
    public static PopupRewardSpin Setup(RewardDatabase.Reward reward)
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupRewardSpin>(PathPrefabs.POPUP_REWARD_SPIN));
        }
        instance.InitSpin(reward);
        GameController.Instance.musicManager.PlayWinSound();
        Debug.Log("=========== Show: PopupRewardSpin");
        return instance;
    }
    void InitSpin(RewardDatabase.Reward reward)
    {
        this.reward = reward;
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
            txtAmount.text = "+" + reward.amount.ToString();
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
    }
    public void Claimx2()
    {
        GameController.Instance.admobAds.ShowVideoReward(Success, ShowFail, null, ActionWatchVideo.Claimx2Spin, "");
    }
    void Success()
    {

        if (reward.item == Item.FreeFind)
        {
            GameController.Instance.moneyEffectController.SpawnEffect_FlyUp(transform.position, reward.item, (1 * 2) + "hours", Color.green, true);
        }
        else if (reward.item == Item.FreePen)
        {
            GameController.Instance.moneyEffectController.SpawnEffect_FlyUp(transform.position, reward.item, (15 * 2) + "minutes", Color.green, true);
        }
        else
        {
            GameController.Instance.moneyEffectController.SpawnEffect_FlyUp(transform.position, reward.item, (reward.amount * 2).ToString(), Color.green);
        }
        Close();
        RewardDatabase.Claim(reward, 2, isShowPopup: false);
    }
    void ShowFail()
    {
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
  (
     btnSpin.transform.position,
      "No Video Ads",
      Color.red,
      isSpawnItemPlayer: true
  );
    }
}
