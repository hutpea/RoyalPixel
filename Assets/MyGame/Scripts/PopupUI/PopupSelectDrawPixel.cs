using BizzyBeeGames.ColorByNumbers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopupSelectDrawPixel : BaseBox
{
    [SerializeField] Texture2D[] boxs;
    private static PopupSelectDrawPixel instance;
    [SerializeField] GameObject sceneDrawPixel;
    [SerializeField] CreatePicImportTexture importTexture;
    public static PopupSelectDrawPixel Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupSelectDrawPixel>(PathPrefabs.POPUP_DRAW_PIXEL));
        }
        //ChickenDataManager.CountTillShowRate = 0;
        return instance;
    }
    public void SelectBox(int order)
    {
        GameData.whiteColor = boxs[order];
        GameData.isDrawPixel = true;
        importTexture.ImportTexture(boxs[order]);
        GameData.CurColorTexture = UIDrawController.instance.colorTexture;
        HomeController.instance.EnableHomeScene(false);
        GameController.Instance.admobAds.ShowInterstitial(false, "select_draw", () => StartCoroutine(LoadGame()));
    }
    IEnumerator LoadGame()
    {
        PictureInformation pictureInformation = new PictureInformation(GameData.content);
        UIDrawController.instance.oldContent = GameData.content;
        //CreateController.instance.AddItem(GameData.content);
        GameData.picChoice = pictureInformation;
        Texture2D tex = TextureController.Instance.GenerateGrayscaleTexture(pictureInformation, 0.8f, false, true); ;
        GameData.curGrayTexture = tex;
        GameData.grayScale = TextureController.Instance.GenerateGrayscaleTexture(pictureInformation, 0.2f, false, true);
        SceneManager.LoadScene(SceneName.GAME_PLAY, LoadSceneMode.Additive);
        HomeController.instance.EnableHomeScene(false);
        Close();
        yield return null;
        //Instantiate(sceneDrawPixel);
    }
    public void ShowAdsBox(int box)
    {
        if (box == 2 || box == 3)
        {
            GameController.Instance.admobAds.ShowVideoReward(() => SuccesBox(box), ShowFail, () => { }, ActionWatchVideo.SelectBox, "");
        }
    }
    int count = 0;
    void SuccesBox(int box)
    {
        count++;
        if (count == 2)
        {
            count = 0;
            PopupVip.Setup().Show();
        }
        else
        {
            SelectBox(box);
        }
    }
    void ShowFail()
    {
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
   (
      transform.position,
       "No Video Ads",
       Color.red,
       isSpawnItemPlayer: true
   );
    }
}
