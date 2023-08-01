using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpriteItem
{
    public Item itemName;
    public Sprite spItem;
}

[CreateAssetMenu(menuName = "ScriptableObject/RewardDatabase", fileName = "RewardDatabase.asset")]
public class RewardDatabase : SerializedScriptableObject
{
    //public List<SpriteItem> spriteItems;

    //public Sprite GetSpriteItem(TypeItem typeItem)
    //{
    //    SpriteItem spItm = spriteItems.Find(spItem => spItem.itemName == typeItem);

    //    return spItm.spItem;
    //}

    public static void Claim(Reward reward, int claimx2 = 1, bool isShowPopup = true)
    {

        switch (reward.item)
        {
            case Item.Pen:
                GameData.ItemPen += reward.amount * claimx2;
                break;
            case Item.Bomb:
                GameData.ItemBomb += reward.amount * claimx2;
                break;
            case Item.Find:
                GameData.ItemFind += reward.amount * claimx2;
                break;
            case Item.Star:
                GameData.ItemStar += reward.amount * claimx2;
                break;
            case Item.Gem:
                GameData.Gem += reward.amount * claimx2;
                EventDispatcher.EventDispatcher.Instance.PostEvent(EventID.CHANGE_VALUE_GEM);
                break;
            default:
                int value = GameData.GetTotalTimeItem(reward.item.ToString());
                value += reward.amount * 60 * claimx2;
                GameData.SetTotalTimeItem(reward.item.ToString(), value);
                GameData.SetDateTimeLastFreeItem(reward.item.ToString(), UnbiasedTime.Instance.Now().AddSeconds(value));
                break;
        }

        if (isShowPopup)
            PopupRewardSpin.Setup(reward).Show();
    }


    [Serializable]
    public class Reward
    {
        public Item item;
        public int amount;
        public int weight;

        public Reward()
        {
        }
        public Reward(Item item, int amount, int weight = 0)
        {
            this.item = item;
            this.amount = amount;
            this.weight = weight;
        }
    }
}

