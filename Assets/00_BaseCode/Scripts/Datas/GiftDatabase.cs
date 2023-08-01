using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[CreateAssetMenu(menuName = "Datas/GiftDatabase", fileName = "GiftDatabase.asset")]
public class GiftDatabase : SerializedScriptableObject
{
    public Dictionary<Item, Gift> giftList;

    public bool GetGift(Item giftType, out Gift gift)
    {
        return giftList.TryGetValue(giftType, out gift);
    }

    public Sprite GetIconItem(Item giftType)
    {
        Gift gift = null;
        //if (IsCharacter(giftType))
        //{
        //    var Char = GameController.Instance.dataContain.dataSkins.GetSkinInfo(giftType);
        //    if (Char != null)
        //        return Char.iconSkin;
        //}
        bool isGetGift = GetGift(giftType, out gift);
        return isGetGift ? gift.getGiftSprite : null;
    }
    public GameObject GetAnimItem(Item giftType)
    {
        Gift gift = null;
        bool isGetGift = GetGift(giftType, out gift);
        return isGetGift ? gift.getGiftAnim : null;
    }

    public void Claim(Item giftType, int amount, Reason reason = Reason.none)
    {

        //switch (giftType)
        //{
        //    case Item.Coin:
        //        // GameController.Instance.useProfile.Coin += amount;
        //        break;
        //        //case GiftType.Health:
        //        //    // GameController.Instance.useProfile.Health += amount;
        //        //    break;
        //        //case GiftType.RemoveAds:
        //        //    GameController.Instance.useProfile.IsRemoveAds = true;
        //        //    GameController.Instance.admobAds.DestroyBanner();
        //        //    break;

        //        //case GiftType.Return:
        //        //    GameController.Instance.useProfile.CurrentNumReturn += amount;
        //        //    break;
        //        //case GiftType.AddBranch:
        //        //    GameController.Instance.useProfile.CurrentNumAddBranch += amount;
        //        //    break;
        //        //case GiftType.RemoveBomb:
        //        //    GameController.Instance.useProfile.CurrentNumRemoveBomb += amount;
        //        //    break;
        //        //case GiftType.RemoveCage:
        //        //    GameController.Instance.useProfile.CurrentNumRemoveCage += amount;
        //        //    break;
        //        //case GiftType.RemoveEgg:
        //        //    GameController.Instance.useProfile.CurrentNumRemoveEgg += amount;
        //        //    break;
        //        //case GiftType.RemoveSleep:
        //        //    GameController.Instance.useProfile.CurrentNumRemoveSleep += amount;
        //        //    break;
        //        //case GiftType.RemoveJail:
        //        //    GameController.Instance.useProfile.CurrentNumRemoveJail += amount;
        //        //    break;

        //        //case GiftType.RandomSkin:
        //        //    //List<BirdSkinData> randomBirds = GameController.Instance.dataContain.birdSkinDatabase.GetRandomListIAPBirdSkinData(amount);
        //        //    //for(int i = 0; i < randomBirds.Count; i++)
        //        //    //{
        //        //    //    ClaimSkin(randomBirds[i].birdSkin);
        //        //    //}
        //        //    break;
        //        //case GiftType.RandomBranch:
        //        //    //List<BranchSkinData> randomBranches = GameController.Instance.dataContain.branchSkinDatabase.GetRandomListIAPBranchData(amount);
        //        //    //for (int i = 0; i < randomBranches.Count; i++)
        //        //    //{
        //        //    //    ClaimBranch(randomBranches[i].id);
        //        //    //}
        //        //    break;
        //        //case GiftType.RandomTheme:
        //        //    //List<ThemeSkinData> randomThemes = GameController.Instance.dataContain.themeSkinDatabase.GetRandomListIAPThemeData(amount);
        //        //    //for (int i = 0; i < randomThemes.Count; i++)
        //        //    //{
        //        //    //    ClaimTheme(randomThemes[i].id);
        //        //    //}
        //        //    break;
        //}
    }

    //public static bool IsCharacter(GiftType giftType)
    //{
    //    switch (giftType)
    //    {
    //        case GiftType.RandomSkin:
    //            return true;
    //    }
    //    return false;
    //}
}

public class Gift
{
    [SerializeField] private Sprite giftSprite;
    [SerializeField] private GameObject giftAnim;
    public virtual Sprite getGiftSprite => giftSprite;
    public virtual GameObject getGiftAnim => giftAnim;

}

public enum Item
{
    Star = 0,
    Bomb = 1,
    Find = 2,
    Pen = 3,
    FreePen = 4,
    FreeFind = 5,
    Eraser = 6,
    Gem = 7
}

public enum Reason
{
    none = 0,
    play_with_item = 1,
    watch_video_claim_item_main_home = 2,
    daily_login = 3,
    lucky_spin = 4,
    unlock_skin_in_special_gift = 5,
    reward_accumulate = 6,
}

[Serializable]
public class RewardRandom
{
    public int id;
    public Item typeItem;
    public int amount;
    public int weight;

    public RewardRandom()
    {
    }
    public RewardRandom(Item item, int amount, int weight = 0)
    {
        this.typeItem = item;
        this.amount = amount;
        this.weight = weight;
    }

    public GiftRewardShow GetReward()
    {
        GiftRewardShow rew = new GiftRewardShow();
        rew.type = typeItem;
        rew.amount = amount;

        return rew;
    }
}
