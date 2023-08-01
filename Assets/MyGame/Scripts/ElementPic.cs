using BizzyBeeGames.ColorByNumbers;
using EnhancedUI.EnhancedScroller;
using EventDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ElementPic : EnhancedScrollerCellView, IUpdateSelectedHandler, IPointerExitHandler, IPointerUpHandler
{
    [SerializeField] Image pic;
    [SerializeField] Image status;
    [SerializeField] Sprite ads, vip, gift, tick;
    public PictureInformation picture;
    public Texture2D texPic;
    public string id;
    private System.Action<object> actionBuyVip;
    [SerializeField] TextAsset defaultText;
    [SerializeField] Text txtNumberAds;
    [SerializeField] GameObject adsMore;
    [SerializeField] GameObject adsOne;
    [SerializeField] Text txtTEstID;
    void OnEnable()
    {
        for (int i = 0; i < GameData.picSinglePainted.Count; i++)
        {
            if (GameData.picSinglePainted[i] == id && picture != null)
            {
                InitPic(picture, GameData.isDrawPixel);
                //GameData.picSinglePainted = new List<string>();
            }
        }
        actionBuyVip = (sender) => InitState();
        this.RegisterListener(EventID.BUY_VIP_SUB, actionBuyVip);
        if (picture != null)
        {
            if (UseProfile.IsVip)
                InitState();
        }
    }

    void InitState()
    {
        if (!picture.Completed)
            status.gameObject.SetActive(false);
    }
    void OnDisable()
    {
        if (actionBuyVip != null)
            this.RemoveListener(EventID.BUY_VIP_SUB, actionBuyVip);
    }
    bool resetActive;
    public void InitPic(PictureInformation picture, bool isDraw = false)
    {
        if (picture == null && defaultText != null)
        {
            picture = new PictureInformation(defaultText.text);
        }
        else if (picture == null)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
            resetActive = true;
            GetComponent<Image>().color = Color.clear;
            return;
        }
        if (resetActive)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
            resetActive = false;
        }
        this.picture = picture;
        this.id = picture.Id;
        if (txtTEstID != null)
            txtTEstID.text = id;
        Rect rect = new Rect(0, 0, picture.XCells, picture.YCells);
        
        texPic = TextureController.Instance.GenerateGrayscaleTexture(picture, 0.8f, true, isDraw);
        pic.sprite = Sprite.Create(texPic, rect, new Vector2(0, 0));
        if (picture.TotalPixelPainted != 0 && !picture.Completed)
        {
            status.gameObject.SetActive(false);
        }
        else if (picture.Completed)
        {
            status.gameObject.SetActive(true);
            adsMore.SetActive(false);
            adsOne.SetActive(false);
            status.sprite = tick;
        }
        else
        {
            status.gameObject.SetActive(true);
            switch (picture.PictureType)
            {
                case PictureType.Ads:
                    if (picture.CurrentAds != 0 && !UseProfile.IsVip)
                    {
                        status.sprite = ads;
                        if (picture.CurrentAds > 1)
                        {
                            adsMore.SetActive(true);
                            adsOne.SetActive(false);
                            txtNumberAds.text = "x" + picture.CurrentAds;
                        }
                        else
                        {
                            adsMore.SetActive(false);
                            adsOne.SetActive(true);
                        }
                    }
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
                    adsMore.SetActive(false);
                    adsOne.SetActive(false);
                    if (!UseProfile.IsVip)
                        status.sprite = vip;
                    else
                        status.gameObject.SetActive(false);
                    break;
            }
        }
    }
    public void SelectPic()
    {
        //if (Application.internetReachability == NetworkReachability.NotReachable)
        //{
        //    PopupNoInternet.Setup().Show();
        //}
        if (!picture.DrawPic)
            GameData.isDrawPixel = false;
        AnalyticsController.LogSelectPic(id);
        if (UseProfile.IsVip)
        {
            PlayGame();
        }
        else
        {
            //if (Application.internetReachability == NetworkReachability.NotReachable)
            //{
            //    PopupNoInternet.Setup().Show();
            //}
            //else
            //{
            switch (picture.PictureType)
            {
                case PictureType.Ads:
                    //#if UNITY_EDITOR
                    //                    PlayGame();
                    //#endif
                    if (picture.TotalPixelPainted != 0 || picture.CurrentAds == 0)
                    {
                        PlayGame();
                    }
                    else
                        GameController.Instance.admobAds.ShowVideoReward(() => PlayGame(), () => ShowFail(), () => { }, ActionWatchVideo.UnlockPic, id.ToString());
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
        //}
    }
    void PlayGame()
    {
        //if (Application.internetReachability == NetworkReachability.NotReachable)
        //{
        //    PopupNoInternet.Setup().Show();
        //}
        //else
        //{
        if (picture.CurrentAds != 0)
        {
            picture.CurrentAds--;
            txtNumberAds.text = "x" + picture.CurrentAds;
            if (picture.CurrentAds == 1)
            {
                txtNumberAds.transform.parent.gameObject.SetActive(false);
                GameData.isSelectInEventFarm = false;
            }
        }
        if (picture.CurrentAds <= 0)
        {
            if (picture.PictureType == PictureType.Ads)
                DailyQuestController.instance.UpdateProgressQuest(TypeQuest.UnlockPic);
            GameData.picChoice = picture;
            Texture2D tex = texPic;
            Texture2D texture = TextureController.Instance.GenerateColorPicture(picture);
            GameData.CurColorTexture = texture;
            GameData.curGrayTexture = tex;
            GameData.grayScale = TextureController.Instance.GenerateGrayscaleTexture(picture, 0.2f, false);

            if (HomeController.instance != null && UIGameController.instance != null && UIGameController.instance.gameObject.activeInHierarchy)
            {
                SceneManager.UnloadScene(2);
                SceneManager.LoadScene(SceneName.GAME_PLAY, LoadSceneMode.Additive);
                HomeController.instance.EnableHomeScene(false);
            }
            else
            {
                SceneManager.LoadScene(SceneName.GAME_PLAY, LoadSceneMode.Additive);
            }
            HomeController.instance.EnableHomeScene(false);
        }
        //}
    }
    void ShowFail()
    {
        GameData.isSelectInEventFarm = false;
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
   (
      transform.position,
       "No Video Ads",
       Color.red,
       isSpawnItemPlayer: true
   );
    }
    float timer;
    public void OnUpdateSelected(BaseEventData data)
    {
        if (!GameData.isDrawPixel)
            return;
        timer += Time.deltaTime;
        if (timer >= 0.8f)
        {
            timer = 0;
            PopupContinueDrawPic popupContinueDraw = PopupContinueDrawPic.Setup(picture.Id);
            popupContinueDraw.Show();
            popupContinueDraw.imgPic.sprite = pic.sprite;
            popupContinueDraw.OnClickDelete = () => { Destroy(gameObject); };
            popupContinueDraw.OnClickContinue = SelectPic;
            return;
        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        timer = 0;
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        timer = 0;
    }
}
