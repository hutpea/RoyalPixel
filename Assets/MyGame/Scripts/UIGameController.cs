using DG.Tweening;
using EventDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UIGameController : MonoBehaviour
{
    [SerializeField] Image fillProgress;
    public static UIGameController instance;
    public Text txtTotalBomb;
    public Text txtTotalStar;
    public Text txtTotalPen;
    public GameObject choiceItemBomb;
    public GameObject choiceItemStar;
    public GameObject choiceItemPen;
    public Image fillChoicePen;
    public GameObject showAdsItemBomb;
    public GameObject showAdsItemStar;
    public GameObject showAdsItemFind;
    public GameObject showAdsItemPen;
    public GameObject panelComplete;
    public GameObject choiceEraser;
    public Animator animNotVideo;
    [SerializeField] GameObject panelTut;
    [SerializeField] GameObject sliderZoom;
    public GameObject[] selectNote;
    public GameObject btnFinishPic;
    bool suggestBtnFinish, isCongra, isPainted;
    public Animator congratulaton;
    public Text txtCongra;
    string[] strCongra = { "WONDERFUL!", "PERFECT!", "FANTASIC!", "AMAZING!" };
    float startProcess;
    [SerializeField] HorizontalScrollSnap horizontalScroll;
    private System.Action<object> actionTotalStar;
    private System.Action<object> actionTotalBomb;
    private System.Action<object> actionTotalFind;
    private System.Action<object> actionTotalPen;
    private System.Action<object> actionBuyVip;
    private System.Action<object> actionGem;
    public GameObject tutHandStar;
    public GameObject tutHandNumberColor;
    [SerializeField] Transform gift50, gift30, gift100, gift70;
    [SerializeField] Text txtPercent;
    [SerializeField] Text txtFind;
    bool showAds;
    public GameObject internetConnect;
    public GameObject btnPen;
    public GameObject unlimtedPen;
    public GameObject unlimtedFind;
    [SerializeField] GameObject panelItem;
    [SerializeField] GameObject itemDrawPixel;
    public GameObject btnSaveDraw;
    [SerializeField] Sprite zoomIn, zoomOut;
    [SerializeField] Image imgBtnZoom;
    public ParticleSystem particle;
    [SerializeField] Image fillCombo;
    List<ParticleSystem> gameObjects;
    [SerializeField] GameObject giftClose;
    [SerializeField] GameObject giftOpen;
    [SerializeField] GameObject particleCongra;
    [SerializeField] Text txtGem;
    [SerializeField] GameObject buyGemItemBomb, buyGemItemStar, buyGemItemPen, buyGemItemFind;
    [SerializeField] GameObject progress;
    public GameObject handBomb1, handBomb2;
    [SerializeField] GameObject giftUsePicCamera;
    public Button btnBack;
    public Transform poolPart;
    void InitPoolParticle()
    {
        gameObjects = new List<ParticleSystem>();
        for (int i = 0; i < 10; i++)
        {
            ParticleSystem a = Instantiate(particle, poolPart);
            a.gameObject.SetActive(false);
            gameObjects.Add(a);
        }
    }
    ParticleSystem GetPool()
    {
        for (int i = 0; i < gameObjects.Count; i++)
        {
            if (!gameObjects[i].gameObject.activeInHierarchy)
                return gameObjects[i];
        }
        ParticleSystem newObj = Instantiate(particle, poolPart);
        gameObjects.Add(newObj);
        return newObj;
    }
    private void Awake()
    {
        instance = this;
        InitPoolParticle();
    }
    private void Start()
    {
        Debug.Log("UIGAMECONTROLLER Start");
        if (!GameData.isDrawPixel)
        {
            SetTextItem();
#if UNITY_EDITOR
            sliderZoom.SetActive(true);
#else
        sliderZoom.SetActive(false);
#endif
            if (GameController.Instance.useProfile.NewUser)
            {
                panelTut.SetActive(true);
                btnPen.SetActive(false);
            }
            else
            {
                if (!PlayerPrefs.HasKey("show_tut"))
                {
                    PopupTutPen.Setup().Show();
                    PlayerPrefs.SetInt("show_tut", 1);
                }
                btnPen.SetActive(true);
            }
            actionTotalBomb = (sender) => SetTotalBomb();
            actionTotalStar = (sender) => SetTotalStar();
            actionTotalFind = (sender) => SetTotalFind();
            actionTotalPen = (sender) => SetTotalPen();
            actionBuyVip = (sender) => SetTextItem();
            actionGem = (sender) => SetTextItem();
        }
        this.RegisterListener(EventID.CHANGE_VALUE_ITEM_BOMB, actionTotalBomb);
        this.RegisterListener(EventID.CHANGE_VALUE_ITEM_WAND, actionTotalStar);
        this.RegisterListener(EventID.CHANGE_VALUE_ITEM_FIND, actionTotalFind);
        this.RegisterListener(EventID.BUY_VIP_SUB, actionBuyVip);
        this.RegisterListener(EventID.CHANGE_VALUE_GEM, actionGem);
        this.RegisterListener(EventID.CHANGE_VALUE_ITEM_PEN, actionTotalPen);
        btnBack.interactable = true;
        GameController.Instance.admobAds.ShowBanner();
        if (GameData.isDrawPixel)
        {
            itemDrawPixel.SetActive(true);
            panelItem.SetActive(false);
            progress.SetActive(false);
        }
        else
        {
            itemDrawPixel.SetActive(false);
            panelItem.SetActive(true);
            progress.SetActive(true);
        }
        if (UseProfile.IsVip)
        {
            txtTotalBomb.transform.parent.gameObject.SetActive(false);
            txtTotalPen.transform.parent.gameObject.SetActive(false);
            txtTotalStar.transform.parent.gameObject.SetActive(false);
            txtFind.transform.parent.gameObject.SetActive(false);
        }
    }
    public void ShowPopupHead()
    {
        PopupHeadPhones.Setup().Show();
        GameData.isShowHeadPhone = false;
    }
    private void OnEnable()
    {
        if (!TutorialHand.FinishTutItem)
        {
            handBomb2.SetActive(true);
        }
    }
    float combo;
    float preCombo;
    bool finishCombo;
    Coroutine coroutine;
    float timer;
    DateTime oldTime = DateTime.MinValue;
    public void SetFillCombo(Vector3 start, Color color)
    {
        //if (finishCombo)
        //    return;
        double t = TimeManager.CaculateDetalTime(oldTime, DateTime.Now);
        if (t > 65)
        {
            ParticleSystem part = GetPool();
            Color newColor = new Color(color.r, color.g, color.b, 0.7f);
            part.startColor = newColor;
            //part.transform.DOKill();
            part.transform.position = start;
            part.transform.localScale = new Vector3(Mathf.Min(0.08f, 0.08f * Camera.main.orthographicSize), Mathf.Min(0.08f, 0.08f * Camera.main.orthographicSize), Mathf.Min(0.08f, 0.08f * Camera.main.orthographicSize));
            part.gameObject.SetActive(true);
            StartCoroutine(Helper.StartAction(() => part.gameObject.SetActive(false), 0.9f));
            oldTime = DateTime.Now;
        }

        //SpriteRenderer sprite = part.GetComponent<SpriteRenderer>();
        //sprite.color = new Color(color.r, color.g, color.b, 1);
        //sprite.DOKill();
        //Vector3 posEnd = fillProgress.transform.position;
        //float dis = Vector2.Distance(part.transform.position, posEnd);
        //part.transform.DOMove(posEnd, dis).OnComplete(() => part.SetActive(false));
        //sprite.color = color;
        //sprite.DOColor(new Color(1, 1, 1, 0), 1.5f);
        //combo++;
        //if (coroutine != null)
        //    StopCoroutine(coroutine);
        //coroutine = StartCoroutine(DOFillCombo());
    }
    IEnumerator DOFillCombo()
    {
        while (fillCombo.fillAmount < 1)
        {
            preCombo = combo;
            fillCombo.fillAmount = combo / 150;
            yield return new WaitForSeconds(1.5f);
            while (combo > 0)
            {
                combo -= 0.2f;
                yield return new WaitForFixedUpdate();
                fillCombo.fillAmount = combo / 150;
            }
        }
        if (combo >= 150)
        {
            GameData.Gem += 2;
            GameController.Instance.moneyEffectController.SpawnEffect_FlyUp(fillCombo.transform.position, Item.Gem, "+" + 2, Color.black);
            combo = 0;
            this.PostEvent(EventID.CHANGE_VALUE_GEM);
            fillCombo.DOFillAmount(0, 2).OnComplete(() =>
            {
                finishCombo = false;
                giftClose.SetActive(true);
                giftOpen.SetActive(false);
                particleCongra.SetActive(false);
            });
            finishCombo = true;
            giftClose.SetActive(false);
            giftOpen.SetActive(true);
            particleCongra.SetActive(true);
        }
    }
    public void SetUIZoom(bool isZoom)
    {
        if (isZoom)
        {
            imgBtnZoom.sprite = zoomIn;
        }
        else
            imgBtnZoom.sprite = zoomOut;
        GamePlayControl.Instance.mainCamera.isZoom = isZoom;

    }
    public void ShowShop()
    {
        GameController.Instance.musicManager.PlayBtnClick();
        PanelShopController.Setup().Show();
    }
    public void SetTextItem(object a = null)
    {
        SetTotalBomb();
        SetTotalStar();
        SetTotalFind();
        SetTotalPen();
        SetTextGem();
    }
    public void SetTextGem()
    {
        txtGem.text = GameData.Gem.ToString();
    }
    public void SetTotalFind()
    {
        if (txtFind != null)
        {
            if (GameData.ItemFind > 0 || UseProfile.IsVip)
            {
                if (!UseProfile.IsVip)
                    txtFind.transform.parent.gameObject.SetActive(true);
                txtFind.text = GameData.ItemFind.ToString();
                showAdsItemFind.SetActive(false);
                buyGemItemFind.SetActive(false);
            }
            else
            {
                txtFind.transform.parent.gameObject.SetActive(false);
                if (GameData.Gem >= 6)
                {
                    showAdsItemFind.SetActive(false);
                    buyGemItemFind.SetActive(true);
                }
                else
                {
                    buyGemItemFind.SetActive(false);
                    showAdsItemFind.SetActive(true);
                }
            }
        }
    }

    int prePage;
    public void ScrollTut()
    {
        selectNote[horizontalScroll.CurrentPage].transform.GetChild(0).gameObject.SetActive(false);
        if (horizontalScroll._previousPage != horizontalScroll.CurrentPage)
            selectNote[horizontalScroll._previousPage].transform.GetChild(0).gameObject.SetActive(true);
    }
    public void OnClickBack()
    {
        if (GameData.picChoice.IdArea != CategoryConst.FARM)
        {
            BackHome();
        }
        else
        {
            PopupBackHome.Setup().Show();
        }
    }
    public void BackHome()
    {
        GameController.Instance.musicManager.PlayBtnClick();
        GameController.Instance.admobAds.ShowInterstitial(actionIniterClose: () =>
        {
            transform.parent.gameObject.SetActive(false);
            SceneManager.UnloadSceneAsync(2, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

            if (HomeController.instance != null)
                HomeController.instance.EnableHomeScene(true);
            else
            {
                GameController.Instance.LoadScene(SceneName.HOME_SCENE);
            }

            Debug.Log($"@@@@@@@@@ {GameData.isSelectInEventFarm}");
            if (GameData.choicePicEvent)
            {
                PopupEventJigsaw.Setup().Show();
                GameData.choicePicEvent = false;
            }
            else if (GameData.isSelectInEventFarm && GameData.picChoice.IdArea == CategoryConst.FARM)
            {
                if (!GamePlayControl.Instance.isGameComplete)
                {
                    Debug.Log("Picture Farm Event not complete, remove all picture history");
                    GamePlayControl.Instance.numberColoring.history.RemoveHistory();
                    GameData.EventFarmTutorial_2_Done = true;
                    /*SceneManager.UnloadScene(2);
                    SceneManager.LoadScene(SceneName.GAME_PLAY, LoadSceneMode.Additive);*/
                }
                else
                {
                    Debug.Log("Picture Farm Event complete, save picture history");
                    GameData.isBackFromEventFarmGameplay = true;
                }

                PopupEventFarmBuilding.Setup().OnShow();
                Debug.Log("Set GameData.isSelectInEventFarm = false");
                GameData.isSelectInEventFarm = false;
            }
            else
            {
                GameData.isBackFromEventFarmGameplay = false;
            }
            //GameController.Instance.LoadScene(SceneName.HOME_SCENE);
        }, actionWatchLog: "back_home");
    }
    private void OnDisable()
    {
        this.RemoveListener(EventID.CHANGE_VALUE_ITEM_BOMB, actionTotalBomb);
        this.RemoveListener(EventID.CHANGE_VALUE_ITEM_WAND, actionTotalStar);
        this.RemoveListener(EventID.CHANGE_VALUE_ITEM_FIND, actionTotalFind);
        this.RemoveListener(EventID.CHANGE_VALUE_ITEM_FIND, actionBuyVip);
        this.RemoveListener(EventID.CHANGE_VALUE_GEM, actionGem);
        this.RemoveListener(EventID.CHANGE_VALUE_ITEM_PEN, actionTotalPen);
        //GameData.isDrawPixel = false;
    }
    bool suggestFinish = false;
    bool showInter = true;
    public void SetProgressComplete(float progess, int totalPainted = 0, float totalPixel = 0)
    {
        fillProgress.transform.parent.gameObject.SetActive(true);
        if (!isPainted)
        {
            isPainted = true;
            startProcess = progess;
        }
        fillProgress.fillAmount = progess;
        GamePlayControl.Instance.percent = (int)(progess * 100);
        txtPercent.text = GamePlayControl.Instance.percent + "%";
        if (progess > 0.77f && !suggestBtnFinish)
        {
            btnFinishPic.SetActive(true);
            suggestBtnFinish = true;
        }
        if (progess > 0.9f && !suggestFinish)
        {
            btnFinishPic.GetComponent<Animator>().Rebind();
            btnFinishPic.SetActive(true);
            suggestFinish = true;
        }
        int totalPixel_1 = RemoteConfigController.GetIntConfig(FirebaseConfig.TOTAL_PIXEL_SHOW_INTER_1, 300);
        int totalPixel_2 = RemoteConfigController.GetIntConfig(FirebaseConfig.TOTAL_PIXEL_SHOW_INTER_2, 800);
        if (GameData.picChoice.TotalPixel > totalPixel_1)
        {
            giftUsePicCamera.SetActive(true);
            gift70.gameObject.SetActive(false);
            if (GameData.picChoice.TotalPixel > totalPixel_2)
            {
                gift70.gameObject.SetActive(true);
                if (progess >= 0.75f)
                {
                    //if (RemoteConfigController.GetBoolConfig(FirebaseConfig.SHOW_INTER_IN_GAME, true))
                    //{
                    gift70.GetComponent<Image>().enabled = false;
                    gift70.transform.GetChild(0).gameObject.SetActive(true);
                    //}
                    if (GameData.GetReciveGiftInGame(GameData.picChoice.Id) < 3)
                    {
                        congratulaton.Rebind();
                        congratulaton.gameObject.SetActive(true);
                        txtCongra.text = "Completed 75%";
                        GameData.SetReciveGiftInGame(GameData.picChoice.Id, 3);
                        if (!ShowInternetConnect())
                        {
                            GameController.Instance.musicManager.PlayProgress();
                            if (GameController.Instance.admobAds.IsAvailableInterstitial())
                            {
                                PopupDelayInter.Setup(4).Show();
                            }
                            else
                                ReciveGift(4);
                        }
                    }
                }
            }

            if (progess >= 0.5f /*&& GameData.isPicBig*/)
            {
                gift50.gameObject.SetActive(true);
                if (RemoteConfigController.GetBoolConfig(FirebaseConfig.SHOW_INTER_IN_GAME, true))
                {
                    gift50.GetComponent<Image>().enabled = false;
                    gift50.transform.GetChild(0).gameObject.SetActive(true);
                }
                if (GameData.GetReciveGiftInGame(GameData.picChoice.Id) < 2)
                {
                    congratulaton.Rebind();
                    congratulaton.gameObject.SetActive(true);
                    txtCongra.text = "Completed 50%";
                    GameData.SetReciveGiftInGame(GameData.picChoice.Id, 2);
                    if (!ShowInternetConnect())
                    {
                        if (GameController.Instance.admobAds.IsAvailableInterstitial())
                        {
                            PopupDelayInter.Setup(4).Show();
                        }
                        else
                            ReciveGift(4);
                    }
                }
            }
            gift30.gameObject.SetActive(true);
            if (progess >= 0.25f)
            {
                gift30.GetComponent<Image>().enabled = false;
                gift30.transform.GetChild(0).gameObject.SetActive(true);
                if (GameData.GetReciveGiftInGame(GameData.picChoice.Id) < 1)
                {
                    congratulaton.Rebind();
                    congratulaton.gameObject.SetActive(true);
                    GameController.Instance.musicManager.PlayProgress();
                    txtCongra.text = "Completed 25%";
                    if (!ShowInternetConnect())
                    {
                        if (GameController.Instance.admobAds.IsAvailableInterstitial())
                        {
                            PopupDelayInter.Setup(4).Show();
                        }
                        else
                            ReciveGift(4);
                    }
                    GameData.SetReciveGiftInGame(GameData.picChoice.Id, 1);
                }
            }
        }
        else
        {
            giftUsePicCamera.SetActive(false);
            if (progess >= 0.25f && progess < 0.5f)
            {
                if (GameData.GetReciveGiftInGame(GameData.picChoice.Id) < 1)
                {
                    congratulaton.Rebind();
                    congratulaton.gameObject.SetActive(true);
                    GameController.Instance.musicManager.PlayProgress();
                    txtCongra.text = "Completed 25%";
                }
                GameData.SetReciveGiftInGame(GameData.picChoice.Id, 1);
            }
            if (progess >= 0.8f)
            {
                if (GameData.GetReciveGiftInGame(GameData.picChoice.Id) < 3)
                {
                    congratulaton.Rebind();
                    congratulaton.gameObject.SetActive(true);
                    txtCongra.text = "Completed 80%";
                    GameData.SetReciveGiftInGame(GameData.picChoice.Id, 3);
                }
            }
            if (progess >= 0.5f /*&& GameData.isPicBig*/)
            {
                if (RemoteConfigController.GetBoolConfig(FirebaseConfig.SHOW_INTER_IN_GAME, true))
                {
                    gift50.GetComponent<Image>().enabled = false;
                    gift50.transform.GetChild(0).gameObject.SetActive(true);
                }
                if (GameData.GetReciveGiftInGame(GameData.picChoice.Id) < 2)
                {
                    congratulaton.Rebind();
                    congratulaton.gameObject.SetActive(true);
                    txtCongra.text = "Completed 50%";
                    GameData.SetReciveGiftInGame(GameData.picChoice.Id, 2);
                    if (!ShowInternetConnect())
                    {
                        if (GameController.Instance.admobAds.IsAvailableInterstitial())
                        {
                            PopupDelayInter.Setup(4).Show();
                        }
                        else
                            ReciveGift(4);
                    }
                }
            }
        }
        if (progess >= 1)
        {
            gift100.GetComponent<Image>().enabled = false;
            gift100.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    bool ShowInternetConnect()
    {
        //bool showInter = RemoteConfigController.GetBoolConfig(FirebaseConfig.SHOW_INTER_IN_GAME, false);
        if (showInter)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                internetConnect.SetActive(true);
                StartCoroutine(Helper.StartAction(() => { internetConnect.SetActive(false); }, 4));
                return true;
            }
            return false;
        }
        return true;
    }
   public void ReciveGift(int value)
    {
        //int rad = Random.Range(0, 8);
        Item gift = Item.Gem;
        //if (rad == 1)
        //{
        //    gift = Item.Star;
        //    GameData.ItemStar += value;
        //    SetTotalStar();
        //}
        //else if (rad == 2 || rad == 3)
        //{
        //    gift = Item.Bomb;
        //    GameData.ItemBomb += value;
        //    SetTotalBomb();
        //}
        //else if (rad == 4 || rad == 5)
        //{
        //    gift = Item.Pen;
        //    GameData.ItemPen += value;
        //    SetTotalPen();
        //}
        //else
        //{
        //    gift = Item.Find;
        //    GameData.ItemFind += value;
        //    SetTotalFind();
        //}
        GameData.Gem += value;

        Color color = new Color32(253, 180, 100, 255);
        Vector3 pos = fillCombo.transform.position;
        GameController.Instance.moneyEffectController.SpawnEffect_GoDestination(transform.position, gift, value, () => { this.PostEvent(EventID.CHANGE_VALUE_GEM); }, txtGem.transform.position);
        //GameController.Instance.moneyEffectController.SpawnEffect_FlyUp(pos, gift, value.ToString(), color);
    }
    public void SetTotalBomb(object a = null)
    {
        if (txtTotalBomb != null)
        {
            if (GameData.ItemBomb <= 0 && !UseProfile.IsVip)
            {
                if (GameData.Gem >= 6)
                {
                    showAdsItemBomb.SetActive(false);
                    buyGemItemBomb.SetActive(true);
                }
                else
                {
                    showAdsItemBomb.SetActive(true);
                    buyGemItemBomb.SetActive(false);
                }
                //choiceItemBomb.SetActive(false);
                txtTotalBomb.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                txtTotalBomb.text = GameData.ItemBomb.ToString();
                if (!UseProfile.IsVip)
                    txtTotalBomb.transform.parent.gameObject.SetActive(true);
                buyGemItemBomb.SetActive(false);
                showAdsItemBomb.SetActive(false);
            }
        }
    }
    public void SetTotalStar(object a = null)
    {
        if (txtTotalStar != null)
        {
            if (GameData.ItemStar <= 0 && !UseProfile.IsVip)
            {
                if (GameData.Gem >= 6)
                {
                    buyGemItemStar.SetActive(true);
                    showAdsItemStar.SetActive(false);
                }
                else
                {
                    showAdsItemStar.SetActive(true);
                    buyGemItemStar.SetActive(false);
                }
                //choiceItemStar.SetActive(false);
                txtTotalStar.transform.parent.gameObject.SetActive(false);
            }
            else
            {

                txtTotalStar.text = GameData.ItemStar.ToString();
                showAdsItemStar.SetActive(false);
                buyGemItemStar.SetActive(false);
                if (!UseProfile.IsVip)
                    txtTotalStar.transform.parent.gameObject.SetActive(true);
            }

        }
    }
    public void SetTotalPen(object a = null)
    {
        if (txtTotalPen != null)
        {
            if (GameData.ItemPen <= 0 && !UseProfile.IsVip)
            {
                if (GameData.Gem >= 6)
                {
                    showAdsItemPen.SetActive(false);
                    buyGemItemPen.SetActive(true);
                }
                else
                {
                    showAdsItemPen.SetActive(true);
                    buyGemItemPen.SetActive(false);
                }
                //choiceItemStar.SetActive(false);
                txtTotalPen.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                txtTotalPen.text = GameData.ItemPen.ToString();
                showAdsItemPen.SetActive(false);
                buyGemItemPen.SetActive(false);
                if (!UseProfile.IsVip)
                    txtTotalPen.transform.parent.gameObject.SetActive(true);
            }
        }
    }
    public void SetFillItemPen(float progess)
    {
        fillChoicePen.fillAmount = progess / 300f;
    }

    //void OnApplicationPause(bool pauseStatus)
    //{
    //    if (pauseStatus)
    //    {
    //        Quit();
    //    }
    //    else
    //    {
    //        // The application is resuming from pause
    //    }
    //}

    static void Quit()
    {
        if (GameData.isSelectInEventFarm && !GamePlayControl.Instance.isGameComplete)
        {
            Debug.Log("Quit Game detect. Picture Farm Event not complete, remove all picture history");
            GamePlayControl.Instance.numberColoring.history.RemoveHistory();
            GameData.EventFarmTutorial_2_Done = true;
        }
    }
    void OnDestroy()
    {
        // The user has manually quit the application, so we can call our cleanup method here.
        // Do whatever cleanup you need to do here.
        Quit();
    }
}
