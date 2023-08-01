using EventDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementAchievement : MonoBehaviour
{
    [SerializeField] Image iconArmorial;
    [SerializeField] Image iconBorderArmorial;
    [SerializeField] Text content;
    [SerializeField] Text topic;
    [SerializeField] Text txtProgress;
    [SerializeField] Image iconGift;
    [SerializeField] Text txtNumberGift;
    [SerializeField] Image fillAmount;
    Achivement achivement;
    GiftQuest giftQuest;
    [SerializeField] GameObject hand;
    [SerializeField] Button btnClaim;
    public void InitElement(Achivement achivement)
    {
        this.achivement = achivement;
        giftQuest = achivement.giftQuest;
        iconBorderArmorial.sprite = achivement.spBorderArmorial;
        iconArmorial.sprite = achivement.spArmorial;
        content.text = achivement.quest;
        topic.text = achivement.type.ToString();
        iconGift.sprite = achivement.giftQuest.sprite;
        txtNumberGift.text = "x" + achivement.giftQuest.numberItem.ToString();
        fillAmount.fillAmount = (float)achivement.Progress / (float)achivement.total;
        txtProgress.text = achivement.Progress + "/" + achivement.total;
        if (fillAmount.fillAmount >= 1)
        {
            btnClaim.interactable = true;
            btnClaim.GetComponent<Animator>().enabled = true;
            if (!TutorialHand.FinishTutAchiviement)
                hand.SetActive(true);
        }
        else
        {
            btnClaim.interactable = false;
            btnClaim.GetComponent<Animator>().enabled = false;
        }
    }
    public void ClaimQuest()
    {
        if (achivement.Progress == achivement.total)
        {
            switch (giftQuest.item)
            {
                case Item.Find:
                    GameData.ItemFind += giftQuest.numberItem;
                    GameController.Instance.moneyEffectController.SpawnEffect_GoDestination(btnClaim.transform.position, giftQuest.item, giftQuest.numberItem, ShowTextItem, MyProfileController.instance.txtFind.transform.position);
                    break;
                case Item.Star:
                    GameData.ItemStar += giftQuest.numberItem;
                    GameController.Instance.moneyEffectController.SpawnEffect_GoDestination(btnClaim.transform.position, giftQuest.item, giftQuest.numberItem, ShowTextItem, MyProfileController.instance.txtStar.transform.position);
                    break;
                case Item.Bomb:
                    GameData.ItemBomb += giftQuest.numberItem;
                    GameController.Instance.moneyEffectController.SpawnEffect_GoDestination(btnClaim.transform.position, giftQuest.item, giftQuest.numberItem, ShowTextItem, MyProfileController.instance.txtBomb.transform.position);
                    break;
                case Item.Pen:
                    GameData.ItemPen += giftQuest.numberItem;
                    GameController.Instance.moneyEffectController.SpawnEffect_GoDestination(btnClaim.transform.position, giftQuest.item, giftQuest.numberItem, ShowTextItem, MyProfileController.instance.txtPen.transform.position);
                    break;

            }
            hand.SetActive(false);
            achivement.Claimed++;
            btnClaim.interactable = false;
            achivement.Progress = 0;
            InitElement(achivement);
            AchievementController.instance.CloseNoti();
            MyProfileController.instance.InitArmorial();
            GameController.Instance.musicManager.PlayBtnClaim();
        }
    }
    void ShowTextItem()
    {
        MyProfileController.instance.ShowItem();
    }

}
