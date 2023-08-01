//using BizzyBeeGames.ColorByNumbers;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class AlbumElement : MonoBehaviour
//{
//    public GiftItem gift;
//    [SerializeField] Text txtNumberAds;
//    private CategoryInfo categoryInfo;
//    public ElementPic elementPic;
//    [SerializeField] Transform content;
//    public int totalAds;
//    [SerializeField] GameObject panelAlbumObject;
//    public int index;
//    [SerializeField] Text txtProgress;
//    [SerializeField] Image fill;
//    [SerializeField] GameObject btnClaim;
//    private void OnEnable()
//    {
//        categoryInfo = GameController.Instance.noelEvent[index];
//        totalAds = categoryInfo.CurrentNumberAds;
//        if (totalAds > 0)
//        {
//            txtNumberAds.text = "x" + totalAds.ToString();
//        }
//        else
//        {
//            txtNumberAds.transform.parent.gameObject.SetActive(false);
//        }
//        float width = 0;
//        int currentPic = GameData.GetCurrentPicInAreas(categoryInfo.Id);
//        int totalPic = categoryInfo.pictureFiles.Count;
//        if (currentPic != 0)
//        {
//            width = 210 * ((float)currentPic / (float)totalPic);
//        }
//        fill.rectTransform.sizeDelta = new Vector2(width, fill.rectTransform.rect.height);
//        //fillProgress.fillAmount = (float)currentPic / (float)totalPic;
//        txtProgress.text = currentPic + "/" + totalPic;
//        if (currentPic == totalPic && !GameData.GetReciveGift(categoryInfo.Id))
//        {
//            btnClaim.SetActive(true);
//            fill.transform.parent.gameObject.SetActive(false);
//        }
//        if (GameData.GetReciveGift(categoryInfo.Id))
//        {
//            btnClaim.SetActive(false);
//            fill.transform.parent.gameObject.SetActive(false);
//        }
//    }
//    public void ClaimGift()
//    {
//        GameData.SetReciveGift(categoryInfo.Id, true);
//        Vector2 pos1 = new Vector2(btnClaim.transform.position.x - 0.7f, btnClaim.transform.position.y);
//        Vector2 pos2 = new Vector2(btnClaim.transform.position.x, btnClaim.transform.position.y);
//        Vector2 pos3 = new Vector2(btnClaim.transform.position.x + 0.7f, btnClaim.transform.position.y);
//        GameController.Instance.moneyEffectController.SpawnEffect_FlyUp(pos1, Item.Bomb, gift.itemBomb.ToString(), Color.yellow);
//        GameController.Instance.moneyEffectController.SpawnEffect_FlyUp(pos2, Item.Star, gift.itemStar.ToString(), Color.yellow);
//        GameController.Instance.moneyEffectController.SpawnEffect_FlyUp(pos3, Item.Find, gift.itemFind.ToString(), Color.yellow);
//        GameData.ItemBomb += gift.itemBomb;
//        GameData.ItemFind += gift.itemFind;
//        GameData.ItemStar += gift.itemStar;
//        btnClaim.SetActive(false);
//    }
//    public void SelectAlbum()
//    {
//        if (totalAds > 0)
//            GameController.Instance.admobAds.ShowVideoReward(UnlocSuccess, ShowFail, null, ActionWatchVideo.UnlockNoel, "");
//        else
//        {
//            OpenAlbum();
//        }
//    }
//    void UnlocSuccess()
//    {
//        totalAds--;
//        categoryInfo.CurrentNumberAds = totalAds;
//        if (totalAds > 0)
//        {
//            txtNumberAds.text = "x" + totalAds.ToString();
//        }
//        else
//        {
//            OpenAlbum();
//        }
//    }
//    void OpenAlbum()
//    {
//        txtNumberAds.transform.parent.gameObject.SetActive(false);
//        panelAlbumObject.SetActive(true);
//        if (content.childCount == 0)
//            for (int i = 0; i < categoryInfo.pictureFiles.Count; i++)
//            {
//                PictureInformation picture = new PictureInformation(categoryInfo.pictureFiles[i].text);
//                ElementPic element = Instantiate(elementPic, content);
//                element.InitPic(picture);
//            }
//    }
//    void ShowFail()
//    {
//        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
//  (
//     transform.position,
//      "No Video Ads",
//      Color.red,
//      isSpawnItemPlayer: true
//  );
//    }
//}
