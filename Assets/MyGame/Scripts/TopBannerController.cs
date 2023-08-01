using BizzyBeeGames.ColorByNumbers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class TopBannerController : MonoBehaviour
{
    [SerializeField] List<Transform> selectNote;
    [SerializeField] HorizontalScrollSnap horizontalScroll;
    [SerializeField] Button btnPaintPicDaily;
    [SerializeField] Button btnBecomVip;
    [SerializeField] Button btnDrawPic;
    [SerializeField] Button btnCreateCamera;
    [SerializeField] Button btnRemoveAds;
    [SerializeField] Button btnEventJigsaw;
    [SerializeField] Image imgPaintPic;
    [SerializeField] Text txtDay;
    [SerializeField] List<GameObject> topBanners;
    PictureInformation pictureInfo;
    [SerializeField] Sprite ads, vip, gift, tick;
    private void Start()
    {
        btnPaintPicDaily.onClick.AddListener(PaintRandomPic);
        btnDrawPic.onClick.AddListener(DrawPic);
        btnCreateCamera.onClick.AddListener(CreateCamera);
        btnBecomVip.onClick.AddListener(GoVip);
        btnEventJigsaw.onClick.AddListener(ShowEvent);
        InitTopBannerPicDaily();
        selectNote[0].transform.GetChild(0).gameObject.SetActive(true);
        step = 0;
    }
    void InitStateVip()
    {

    }
    public void ScrollTut()
    {
        SwipeScroll();
        SetStep();
    }
    public void GoVip()
    {
        PopupVip.Setup().Show();
    }
    public void DrawPic()
    {
        HomeController.instance.SetActiveTab(3);
        DesignerController.instance.OpenDraw();
    }
    public void CreateCamera()
    {
        HomeController.instance.SetActiveTab(3);
        DesignerController.instance.OpenImport();
    }
    void PlayGame()
    {
        pictureInfo.CurrentAds = 0;
        imgPaintPic.transform.GetChild(0).gameObject.SetActive(false);
        GameData.isDrawPixel = false;
        GameData.picChoice = pictureInfo;
        Texture2D tex = TextureController.Instance.GenerateGrayscaleTexture(pictureInfo, 0.8f, true);
        Texture2D texture = TextureController.Instance.GenerateColorPicture(pictureInfo);
        GameData.CurColorTexture = texture;
        GameData.curGrayTexture = tex;
        GameData.grayScale = TextureController.Instance.GenerateGrayscaleTexture(GameData.picChoice, 0.2f, false);
        HomeController.instance.EnableHomeScene(false);
        SceneManager.LoadScene(SceneName.GAME_PLAY, LoadSceneMode.Additive);
    }
    public void PaintRandomPic()
    {
        switch (pictureInfo.PictureType)
        {
            case PictureType.Ads:
                //#if UNITY_EDITOR
                //                    PlayGame();
                //#endif
                if (pictureInfo.TotalPixelPainted != 0 || pictureInfo.CurrentAds == 0)
                {
                    PlayGame();
                }
                else
                    GameController.Instance.admobAds.ShowVideoReward(() => PlayGame(), () => ShowFail(), () => { }, ActionWatchVideo.UnlockPic, "");
                break;
            case PictureType.Gem:
                PlayGame();
                break;
            case PictureType.Free:
                GameController.Instance.admobAds.ShowInterstitial(false, "select_pic", () => { PlayGame(); });
                break;
            case PictureType.Vip:
                PopupVip.Setup().Show();
                break;
        }
    }
    bool countDown = false;
    public void ShowApp()
    {
        Application.OpenURL("market://details?id=com.gplay.art.puzzle.free.game");
    }    
    private void FixedUpdate()
    {
        if (countTimer > timer)
        {
            if (countDown)
                step--;
            else
                step++;
            if (step == 0)
            {
                countDown = false;
            }
            if (step == selectNote.Count - 1)
                countDown = true;
            horizontalScroll.ChangePage(step);
            countTimer = 0;
            SetStep();
        }
        countTimer += Time.deltaTime;
    }
    void ShowFail()
    {
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
   (
      imgPaintPic.transform.position,
       "No Video Ads",
       Color.red,
       isSpawnItemPlayer: true
   );
    }
    void InitTopBannerPicDaily()
    {
        DateTime now = UnbiasedTime.Instance.Now();
        string strMonth = "";
        switch (now.Month)
        {
            case 1:
                strMonth = "January";
                break;
            case 2:
                strMonth = "February";
                break;
            case 3:
                strMonth = "March";
                break;
            case 4:
                strMonth = "April";
                break;
            case 5:
                strMonth = "May";
                break;
            case 6:
                strMonth = "June";
                break;
            case 7:
                strMonth = "July";
                break;
            case 8:
                strMonth = "August";
                break;
            case 9:
                strMonth = "September";
                break;
            case 10:
                strMonth = "October";
                break;
            case 11:
                strMonth = "November";
                break;
            case 12:
                strMonth = "December";
                break;
        }
        txtDay.text = now.Day + " " + strMonth;
        Debug.Log("GameData.CountDailyPicture " + GameData.CountDailyPicture);
        System.TimeSpan timeSpanDaily = TimeManager.ParseTimeStartDay(UnbiasedTime.Instance.Now()) - TimeManager.ParseTimeStartDay(GameData.GetDateTimeTopBanner());
        if (timeSpanDaily.TotalDays >= 1)
        {
            GameData.CountDailyPicture++;
            if (GameData.CountDailyPicture >= GameController.Instance.picDaily.Count)
            {
                GameData.CountDailyPicture = 0;
            }
            GameData.SetDateTimeTopBanner(DateTime.Now);
        }
        TextAsset text = GameController.Instance.picDaily[GameData.CountDailyPicture];
        pictureInfo = new PictureInformation(text.text);
        Texture2D texture = TextureController.Instance.GenerateColorPicture(pictureInfo);
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        imgPaintPic.sprite = Sprite.Create(texture, rect, new Vector2(0, 0));
        imgPaintPic.transform.GetChild(0).gameObject.SetActive(true);
        Debug.Log("toop" + pictureInfo.Id);
        Image status = imgPaintPic.transform.GetChild(0).GetComponent<Image>();
        switch (pictureInfo.PictureType)
        {
            case PictureType.Ads:
                if (pictureInfo.CurrentAds != 0 && !UseProfile.IsVip)
                    status.sprite = ads;
                else
                    status.gameObject.SetActive(false);
                break;
            case PictureType.Gem:
                status.sprite = gift;
                break;
            case PictureType.Free:
                status.gameObject.SetActive(false);
                break;
            case PictureType.Vip:
                if (!UseProfile.IsVip)
                    status.sprite = vip;
                break;
        }
    }
    public void ShowShop()
    {
        GameController.Instance.musicManager.PlayBtnClick();
        PanelShopController.Setup().Show();
    }
    public void ShowEvent()
    {
        GameController.Instance.musicManager.PlayBtnClick();
        if (GameData.CountPicComplete < 5)
        {
            PopupSuggestEvent.Setup().Show();
        }
        else
            PopupEventJigsaw.Setup().Show();
    }
    float timer = 3;
    int step;
    float countTimer = 0;
    //IEnumerator AutoScroll()
    //{
    //    step = 0;
    //    bool countDown = false;
    //    while (true)
    //    {
    //        Debug.Log("step " + step);
    //        if (countTimer > timer)
    //        {
    //            if (countDown)
    //                step--;
    //            else
    //                step++;
    //            if (step == 0)
    //            {
    //                countDown = false;
    //            }
    //            if (step == selectNote.Count - 1)
    //                countDown = true;
    //            horizontalScroll.ChangePage(step);
    //            countTimer = 0;
    //            SetStep();
    //        }
    //        countTimer += 1;
    //        yield return new WaitForSecondsRealtime(1);
    //    }
    //}
    public void SetStep()
    {
        int step = horizontalScroll.CurrentPage;
        for (int i = 0; i < selectNote.Count; i++)
        {
            if (step == i)
            {
                selectNote[i].transform.GetChild(0).gameObject.SetActive(true);
            }
            else
                selectNote[i].transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    public void SwipeScroll()
    {
        step = horizontalScroll.CurrentPage;
        countTimer = 0;
    }
    public void BuyRemoveAds()
    {
        AnalyticsController.LogClickVIP("complete_noads");
        UseProfile.IsRemoveAds = true;
        GameController.Instance.admobAds.DestroyBanner();
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp(btnRemoveAds.transform.position, "BUY COMPLETED!", Color.green);
    }
}
