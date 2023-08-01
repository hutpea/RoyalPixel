using BizzyBeeGames.ColorByNumbers;
using EventDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeController : MonoBehaviour
{
    public static HomeController instance;
    public Transform posBG;
    [SerializeField] Transform contentAreas;
    [SerializeField] GameObject panelAreas, panelHome, panelDiscover, panelCreate, panelMyWork;
    [SerializeField] GameObject tabAreas, tabHome, tabDiscover, tabCreate, tabMyWork;
    public Text txtBomb;
    public Text txtStar;
    public GameObject panelViewArea;
    public List<PictureInformation> pictures;
    public Material grayToColor;
    //private Dictionary<int, CategoryInfo> categoryInfos;
    public Material materialPic;
    public bool startView;
    public int idView;
    public GameObject btnPlay;
    public GameObject mainCamera;
    public GameObject canvasMain;
    [SerializeField] DiscoveryController discoveryController;
    [SerializeField] EventFarmBuildingController eventFarmBuildingController;
    public string progress = "";
    public GameObject notiDiscover;
    public GameObject notiArea;
    public GameObject notiDaily;
    public GameObject showNewArea;
    public Transform contentNewArea;
    public AreaController areaController;
    public Transform contentViewNewArea;
    public Transform contentViewNormalArea;
    //[SerializeField] GameObject btnNoAds;
    [SerializeField] GameObject popupNoel;
    [SerializeField] GameObject btnNoel;
    [SerializeField] Text txtTimeLeft;
    [SerializeField] Sprite[] icon_none;
    [SerializeField] Sprite[] icon_color;
    bool start;
    [SerializeField] Text txtGem;

    //public FarmCameraGroup farmCameraGroup;

    private IEnumerator openSpinAuto;

    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
        pictures = new List<PictureInformation>();
        //if (GameController.Instance.useProfile.NewUser)
        //{
        //    mainCamera.SetActive(false);
        //    SceneManager.LoadScene(2, LoadSceneMode.Additive);
        //    StartCoroutine(Helper.StartAction(() =>
        //    {
        //        EnableHomeScene(false);
        //    }, 1f));
        //}
        Debug.Log("update " + GameData.NewUpdate);
        if (GameData.NewUpdate)
        {
            notiDiscover.SetActive(true);
            notiArea.SetActive(true);
        }
        else
        {
            notiDiscover.SetActive(false);
            notiArea.SetActive(false);
        }
        start = true;
        //categoryInfos = new Dictionary<int, CategoryInfo>();
        //categoryInfos = GameController.Instance.dataImage.categoryInfos;
        Debug.Log("Set GameData.isSelectInEventFarm = false");
        GameData.isSelectInEventFarm = false;
        GameData.isInViewEventFarm = false;
        GameData.isBackFromEventFarmGameplay = false;
    }

    public void EnableHomeScene(bool enable)
    {
        mainCamera.SetActive(enable);
        canvasMain.SetActive(enable);
    }

    private void OnDisable()
    {
        if (openSpinAuto != null)
            StopCoroutine(openSpinAuto);
    }

    private void OnEnable()
    {

        GameController.Instance.musicManager.PlayBGHomeMusic();

        //if (UseProfile.IsVip)
        //{
        //    System.TimeSpan timeSpan = UnbiasedTime.Instance.Now() - GameData.GetDateTimeReciveGift();
        //    if (timeSpan.Days >= 1)
        //    {
        //        GameData.ItemBomb += VipController.Instance.giftVip.gifts[0].itemBomb;
        //        GameData.ItemStar += VipController.Instance.giftVip.gifts[0].itemStar;
        //        GameData.ItemFind += VipController.Instance.giftVip.gifts[0].itemFind;
        //        PopupClaimVip.Setup().Show();
        //    }
        //}
        if (!GameController.Instance.useProfile.NewUser)
        {
            System.TimeSpan timeSpanDaily = TimeManager.ParseTimeStartDay(UnbiasedTime.Instance.Now()) - TimeManager.ParseTimeStartDay(GameData.GetDateTimeReciveDailyGift());
            Debug.Log(timeSpanDaily.Days);
            if (timeSpanDaily.TotalDays >= 1)
                PopupDailyReward.Setup().Show();
            else
            {
                System.DateTime lastTimeFreeSpined = GameData.LuckySpinLastTimeFreeSpined;
                if (lastTimeFreeSpined.ToString() != "")
                {
                    if (lastTimeFreeSpined < UnbiasedTime.Instance.Now())
                    {
                        openSpinAuto = OpenLuckySpinAuto();
                        StartCoroutine(openSpinAuto);
                    }
                }
            }
            //else
            //{
            //    if (start)
            //    {
            //        bool canShow = RemoteConfigController.GetBoolConfig(FirebaseConfig.CAN_SHOW_POPUP, true);
            //        if (canShow && !popupNoel.gameObject.activeInHierarchy)
            //            PopupNotiEvent.Setup().Show();
            //        start = false;
            //    }
            //}
        }
        SetTextItem();
        ActiveBtnNoAds();

        //TimeLeftEvent();
    }

    private IEnumerator OpenLuckySpinAuto()
    {
        yield return new WaitForSeconds(0.2f);
        if (!GameData.isInViewEventFarm)
        {
            LuckySpin.Setup().Show();
        }
    }

    public void ActiveBtnNoAds()
    {
        //if (GameController.Instance.useProfile.IsRemoveAds)
        //{
        //    btnNoAds.SetActive(false);
        //}
        //else
        //    btnNoAds.SetActive(true);
    }
    public void OpenSpin()
    {
        GameController.Instance.musicManager.PlayBtnClick();
        LuckySpin.Setup().Show();
    }
    private void Start()
    {
        areaController.LoadFilePic();
        LoadAreasHome();
        SetActiveTab(0);
        eventFarmBuildingController.LoadFilePic();
        discoveryController.LoadFilePic();
        Invoke("SetActiveDiscover", 0.5f);
        if (!PlayerPrefs.HasKey(StringConstants.KEY.SAVE_INPROGRESS) && progress != "")
        {
            Debug.Log("inprogressXX " + progress);
            PlayerPrefs.SetString(StringConstants.KEY.SAVE_INPROGRESS, progress);
        }
        this.RegisterListener(EventID.BUY_VIP_SUB, SetTextItem);
        this.RegisterListener(EventID.CHANGE_VALUE_ITEM_BOMB, SetTextItem);
        this.RegisterListener(EventID.CHANGE_VALUE_ITEM_WAND, SetTextItem);
        this.RegisterListener(EventID.CHANGE_VALUE_GEM, SetTextItem);
        GameController.Instance.admobAds.ShowBanner();

        var allAudioListener = FindObjectsOfType<AudioListener>();
        for (int i = 0; i < allAudioListener.Length; i++)
        {
            if (i == 0)
                allAudioListener[i].enabled = true;
            else
                allAudioListener[i].enabled = false;
        }
    }
    void SetActiveDiscover()
    {
        SetActiveTab(0);
    }
    void TimeLeftEvent()
    {
        string strDefault = "08-01-2023";
        System.DateTime time = System.DateTime.Parse(RemoteConfigController.GetStringConfig(FirebaseConfig.TIME_EVENT, strDefault));
        System.TimeSpan timeSpan = (time - UnbiasedTime.Instance.Now());
        int day = (int)timeSpan.TotalDays;
        if (day > 1)
            txtTimeLeft.text = ((int)timeSpan.TotalDays).ToString() + "days";
        if (day <= 1)
        {
            int totalSeconds = (int)timeSpan.TotalSeconds;
            StartCoroutine(CountTimeEvent(totalSeconds));
        }
        if (day < 0)
            btnNoel.SetActive(false);
    }
    IEnumerator CountTimeEvent(int totalTime)
    {
        while (totalTime > 0)
        {
            totalTime--;
            yield return new WaitForSecondsRealtime(1);
            txtTimeLeft.text = Helper.getFormattedTimeFromSeconds(totalTime);
        }
        btnNoel.SetActive(false);
    }
    public void LoadAreasHome()
    {
        //GameData.idAreasSelected = GameData.PaintingAreas;
        Debug.Log(GameData.PaintingAreas);
        AreasPic areasPic;
        if (GameData.PaintingCateAreas == 1)
        {
            showNewArea.SetActive(true);
            posBG.gameObject.SetActive(false);
            areasPic = Instantiate(Resources.Load("Areas/" + GameData.PaintingAreas) as GameObject, contentNewArea).GetComponent<AreasPic>();

        }
        else
        {
            showNewArea.SetActive(false);
            posBG.gameObject.SetActive(true);
            areasPic = Instantiate(Resources.Load("Areas/" + GameData.PaintingAreas) as GameObject, posBG).GetComponent<AreasPic>();
        }
        areasPic.transform.SetAsFirstSibling();
        Debug.Log("GameData.PaintingCateAreas " + GameData.PaintingCateAreas + "|" + GameData.PaintingAreas);
        GameData.totalPicInAreas = GameController.Instance.dataAreas.cateItems[GameData.PaintingCateAreas].dataPic.CategoryInfos[GameData.PaintingAreas.ToString()].PictureInfos.Count;
        areasPic.InitArea(GameController.Instance.dataAreas.cateItems[GameData.PaintingCateAreas].dataPic.CategoryInfos[GameData.PaintingAreas.ToString()].PictureInfos.ToArray());
    }
    public void LoadListAreas(int idAreas, int total, PictureInformation[] pictureInformation)
    {
        //AreasElement areasElement = Instantiate(Resources.Load("Home/AreasElement") as GameObject, contentAreas).GetComponent<AreasElement>();
        //areasElement.InitAreas(idAreas, total, pictureInformation);
        //bool check = false;
        //if (!PlayerPrefs.HasKey(StringConstants.KEY.SAVE_INPROGRESS))
        //{
        //    for (int i = 0; i < pictureInformation.Length; i++)
        //    {
        //        if (pictureInformation[i].TotalPixelPainted != 0)
        //        {
        //            check = true;
        //        }
        //    }
        //    if (check)
        //    {
        //        progress += idAreas + ",";
        //    }
        //}
        //if (GameData.CompleteArea)
        //{
        //    if (!startView)
        //    {
        //        startView = true;
        //        idView = GameData.PaintingAreas;
        //    }
        //    if (idAreas == idView)
        //    {
        //        areasElement.ViewPic();
        //        SetActiveTab(1);
        //        GameData.CompleteArea = false;

        //    }
        //    //GameData.PaintingAreas = GetKey();
        //}
        //else
        //{
        //    SetActiveTab(2);
        //}

    }

    public int GetKey()
    {
        //foreach (string key in GameController.Instance.dataImage.CategoryInfos.Keys)
        //{
        //    if (!GameData.GetUnlockArea(int.Parse(key)))
        //    {
        //        return int.Parse(key);
        //    }
        //}
        return GameData.PaintingAreas;
    }
    public void OpenDailyQuest()
    {
        GameController.Instance.musicManager.PlayBtnClick();
        PopupDailyQuest.Setup().Show();
    }

    public void OpenEventFarmBuildingUI()
    {
        GameController.Instance.admobAds.ShowInterstitial(false, "open_farm", () =>
        {
            GameController.Instance.musicManager.PlayBtnClick();
            PopupEventFarmBuilding.Setup().OnShow();
        });
    }

    public void SetActiveTab(int tab)
    {
        GameController.Instance.musicManager.PlayBtnClick();
        panelAreas.SetActive(false);
        panelHome.SetActive(false);
        panelDiscover.SetActive(false);
        panelCreate.SetActive(false);
        panelMyWork.SetActive(false);
        tabDiscover.GetComponent<Image>().sprite = icon_none[0];
        tabAreas.GetComponent<Image>().sprite = icon_none[1];
        tabHome.GetComponent<Image>().sprite = icon_none[2];
        tabCreate.GetComponent<Image>().sprite = icon_none[3];
        tabMyWork.GetComponent<Image>().sprite = icon_none[4];
        tabDiscover.transform.parent.GetChild(1).GetComponent<Text>().color = new Color32(176, 176, 176, 255);
        tabAreas.transform.parent.GetChild(1).GetComponent<Text>().color = new Color32(176, 176, 176, 255);
        tabHome.transform.parent.GetChild(1).GetComponent<Text>().color = new Color32(176, 176, 176, 255);
        tabCreate.transform.parent.GetChild(1).GetComponent<Text>().color = new Color32(176, 176, 176, 255);
        tabMyWork.transform.parent.GetChild(1).GetComponent<Text>().color = new Color32(176, 176, 176, 255);
        GameData.isDrawPixel = false;
        ResetPosBtnMenu();
        if (tab == 0)
        {
            panelDiscover.SetActive(true);
            tabDiscover.GetComponent<Image>().sprite = icon_color[tab];
            tabDiscover.transform.parent.GetChild(1).GetComponent<Text>().color = new Color32(236, 126, 32, 255);
        }
        if (tab == 1)
        {
            panelAreas.SetActive(true);
            tabAreas.GetComponent<Image>().sprite = icon_color[tab];
            tabAreas.transform.parent.GetChild(1).GetComponent<Text>().color = new Color32(236, 126, 32, 255);
        }
        if (tab == 2)
        {
            panelHome.SetActive(true);
            tabHome.GetComponent<Image>().sprite = icon_color[tab];
            tabHome.transform.parent.GetChild(1).GetComponent<Text>().color = new Color32(236, 126, 32, 255);
        }
        if (tab == 3)
        {
            panelCreate.SetActive(true);
            tabCreate.GetComponent<Image>().sprite = icon_color[tab];
            tabCreate.transform.parent.GetChild(1).GetComponent<Text>().color = new Color32(236, 126, 32, 255);
        }
        if (tab == 4)
        {
            panelMyWork.SetActive(true);
            tabMyWork.GetComponent<Image>().sprite = icon_color[tab];
            tabMyWork.transform.parent.GetChild(1).GetComponent<Text>().color = new Color32(236, 126, 32, 255);
        }
    }
    private void ResetPosBtnMenu()
    {
        tabAreas.transform.localPosition = new Vector3(tabAreas.transform.localPosition.x, 15, tabAreas.transform.localPosition.z);
        tabHome.transform.localPosition = new Vector3(tabHome.transform.localPosition.x, 15, tabHome.transform.localPosition.z);
        tabDiscover.transform.localPosition = new Vector3(tabDiscover.transform.localPosition.x, 15, tabDiscover.transform.localPosition.z);
        tabCreate.transform.localPosition = new Vector3(tabCreate.transform.localPosition.x, 15, tabCreate.transform.localPosition.z);
        tabMyWork.transform.localPosition = new Vector3(tabMyWork.transform.localPosition.x, 15, tabMyWork.transform.localPosition.z);
    }

    public void SetTextItem(object a = null)
    {
        txtBomb.text = GameData.ItemBomb.ToString();
        txtStar.text = GameData.ItemStar.ToString();
        txtGem.text = GameData.Gem.ToString();
    }
    public void BtnPlayGame()
    {
        GameController.Instance.admobAds.ShowInterstitial(false, ActionWatchVideo.PlayHome.ToString(), SuccesPlay, "");
        GameController.Instance.musicManager.PlayBtnClick();
    }
    void SuccesPlay()
    {
        List<int> ids = new List<int>();
        int rad = -1;
        for (int i = 0; i < pictures.Count; i++)
        {
            if (pictures[i].Id == GameData.PicPainting && !pictures[i].Completed)
            {
                rad = i;
                if (panelHome.transform.childCount != 0)
                    panelHome.GetComponentInChildren<AreasPic>().SelectPicture(pictures[rad], rad);
                else
                    contentNewArea.GetComponentInChildren<AreasPic>().SelectPicture(pictures[rad], rad);
                return;
            }
            else
            {
                if (!pictures[i].Completed)
                {
                    ids.Add(i);
                }
            }
        }
        if (ids.Count != 0)
        {
            rad = ids[Random.Range(0, ids.Count - 1)];
            Debug.Log("rad " + rad);
            if (panelHome.transform.childCount != 0)
                panelHome.GetComponentInChildren<AreasPic>().SelectPicture(pictures[rad], rad);
            else
                contentNewArea.GetComponentInChildren<AreasPic>().SelectPicture(pictures[rad], rad);
        }
        else
        {
            Vector3 pos = new Vector3(btnPlay.transform.position.x - 1, btnPlay.transform.position.y, btnPlay.transform.position.z);
            GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
       (
         pos,
           "Choice New Picture",
           Color.blue,
           isSpawnItemPlayer: true
       );
        }
    }
    public void TapSetting()
    {
        GameController.Instance.musicManager.PlayBtnClick();
        GameController.Instance.admobAds.ShowInterstitial(actionIniterClose: () =>
        {
            SetActiveTab(3);
        }, actionWatchLog: "tap_setting");
    }
    public void CloseViewPic()
    {
        //GameController.Instance.admobAds.DestroyBanner();
    }
    public void ShowAddItemBomb()
    {
        PopupSuggestAdsBomb.Setup().Show();
    }
    public void ShowAddItemWand()
    {
        PopupSuggestAdsWand.Setup().Show();
    }
    public void ShowPopupNoel()
    {
        popupNoel.SetActive(true);
    }

    public void CheatBtn()
    {
        GameData.Gem += 1000;
        GameData.ItemBomb += 100;
        GameData.ItemFind += 100;
        GameData.ItemPen += 100;
        GameData.ItemStar += 100;
        GameData.CountPicComplete += 6;
        /*UseProfile.IsRemoveAds = true;
        UseProfile.IsVip = true;
        //GameData.ClaimResourceItems(resourceItemColl.items);
        //GameAnalytics.LogEarnVirtualCurrency("shop_iap_" + product.definition.id, resourceItemColl.items);
        //GameData.ItemBomb += giftVip.gifts[0].itemBomb;
        //GameData.ItemStar += giftVip.gifts[0].itemStar;
        //GameData.ItemFind += giftVip.gifts[0].itemFind;
        //GameAnalytics.LogBuyIAP(product.definition.id);
        HomeController.instance.ActiveBtnNoAds();
        GameController.Instance.admobAds.DestroyBanner();
        StartCoroutine(Helper.StartAction(() => { PopupVip.Setup().Close(); }, 1));
        this.PostEvent(EventID.BUY_VIP_SUB);
        PopupClaimVip.Setup().Show();*/

        SetTextItem();
    }
}
