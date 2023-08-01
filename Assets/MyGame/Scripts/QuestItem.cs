using EventDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestItem : MonoBehaviour
{
    [SerializeField] Text txtQuest;
    [SerializeField] Image fillProgress;
    [SerializeField] Text txtNumberQuest;
    GiftQuest giftQuest;
    [SerializeField] Transform gift;
    [SerializeField] Transform giftOpen;
    [SerializeField] GameObject tick;
    Quest quest;
    [SerializeField] Animator giftAnim;
    [SerializeField] Sprite iconNoti, iconTick;
    [SerializeField] GameObject hand;
    public void InitQuest(Quest quest, string Id)
    {
        txtQuest.text = quest.quest;
        this.quest = quest;
        Debug.Log("quest " + quest.Progress);
        fillProgress.fillAmount = (float)Mathf.Min(quest.Progress, quest.total) / (float)quest.total;
        txtNumberQuest.text = Mathf.Min(quest.Progress, quest.total) + "/" + quest.total;
        giftQuest = quest.giftQuest;
        tick.SetActive(false);
        gift.gameObject.SetActive(true);
        giftOpen.gameObject.SetActive(false);
        if (quest.Claimed)
        {
            tick.GetComponent<Image>().sprite = iconTick;
            tick.SetActive(true);
            gift.gameObject.SetActive(false);
            giftOpen.gameObject.SetActive(true);
        }
        else
        {
            if (fillProgress.fillAmount == 1)
            {
                if (!TutorialHand.FinishTutDailyQuest)
                    hand.SetActive(true);
                tick.SetActive(true);
                gift.GetComponent<Animator>().enabled = true;
                tick.GetComponent<Image>().sprite = iconNoti;
            }
        }
    }
    public void ClaimQuest()
    {
        if (quest.Claimed)
            return;
        if (quest.Progress == quest.total)
        {
            switch (giftQuest.item)
            {
                case Item.Find:
                    GameData.ItemFind += giftQuest.numberItem;
                    this.PostEvent(EventID.CHANGE_VALUE_ITEM_FIND);
                    break;
                case Item.Star:
                    GameData.ItemStar += giftQuest.numberItem;
                    this.PostEvent(EventID.CHANGE_VALUE_ITEM_WAND);
                    break;
                case Item.Bomb:
                    GameData.ItemBomb += giftQuest.numberItem;
                    this.PostEvent(EventID.CHANGE_VALUE_ITEM_BOMB);
                    break;
                case Item.Gem:
                    GameData.Gem += giftQuest.numberItem;
                    this.PostEvent(EventID.CHANGE_VALUE_GEM);
                    break;
            }
            gift.gameObject.SetActive(false);
            giftOpen.gameObject.SetActive(true);
            giftAnim.gameObject.SetActive(true);
            giftAnim.GetComponentInChildren<Text>().text = "+" + giftQuest.numberItem;
            if (GameController.Instance.dataContain.giftDatabase.GetGift(giftQuest.item, out Gift giftQ))
            {
                giftAnim.GetComponentInChildren<Image>().sprite = giftQ.getGiftSprite;
            }
            quest.Claimed = true;
            HomeController.instance.notiDaily.SetActive(false);
            tick.SetActive(true);
            tick.GetComponent<Image>().sprite = iconTick;
            DailyQuestController.instance.CloseNoti();
            hand.SetActive(false);
            TutorialHand.FinishTutDailyQuest = true;
            GameController.Instance.musicManager.PlayBtnClaim();
        }
    }
}
