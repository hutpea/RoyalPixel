using BizzyBeeGames.ColorByNumbers;
using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreasElement : EnhancedScrollerCellView
{
    [SerializeField] Image bg;
    [SerializeField] GameObject complete;
    [SerializeField] GameObject btnView;
    [SerializeField] GameObject progress;
    [SerializeField] Image fillProgress;
    [SerializeField] GameObject obLock;
    [SerializeField] Text txtProgress;
    [SerializeField] Text txtBanner;
    [SerializeField] Sprite banner1, banner2;
    [SerializeField] Text txtPercent;
    [SerializeField] Image fillPercent;
    public int ID;
    [SerializeField] GameObject gift;
    [SerializeField] Transform slider;
    public int totalPic;
    public PictureInformation[] tempPics;
    public bool isComplete;
    bool startView;
    int idView;
    public int IdCateArea;
    [SerializeField] GameObject iconAds;
    public CategoryInfo categoryInfo;
    [SerializeField] GameObject iconAdsMore;
    [SerializeField] Material gray;
    void OnEnable()
    {
        if (GameData.IdAreaChoice == ID && tempPics != null)
        {
            InitAreas(categoryInfo, IdCateArea);
        }
    }
    public void InitAreas(CategoryInfo category, int IdCateAreas)
    {
        if (category == null)
            return;
        this.IdCateArea = IdCateAreas;
        this.categoryInfo = category;
        this.tempPics = category.PictureInfos.ToArray();
        this.ID = int.Parse(category.displayName);
        //Texture2D texture = GameController.Instance.assetBundles.dataTexture.LoadAsset<Texture2D>("c_" + idAreas);
        //Texture2D texture = Resources.Load("ColorAreas/c_" + ID) as Texture2D;
        //Rect rect = new Rect(0, 0, texture.width, texture.height);
        //bg.sprite = Sprite.Create(texture, rect, new Vector2(0, 0));
        bg.sprite = AreaController.instance.dataArena.colorSprite[ID - 1];
        this.totalPic = category.PictureInfos.Count;
        float numberProgress = 0;
        float total = 0;
        for (int i = 0; i < tempPics.Length; i++)
        {
            numberProgress += tempPics[i].TotalPixelPainted;
        }
        if (numberProgress != 0)
            for (int i = 0; i < tempPics.Length; i++)
            {
                total += tempPics[i].TotalPixel;
            }

        if (categoryInfo.CurrentNumberAds == 0 || categoryInfo.typePic == PictureType.Free)
        {
            iconAds.SetActive(false);
            iconAdsMore.SetActive(false);
        }

        if (ID == 34 && categoryInfo.CurrentNumberAds != 0 && !UseProfile.IsVip)
        {
            iconAds.SetActive(true);
            if (numberProgress != 0)
                iconAds.SetActive(false);
        }
        else if (categoryInfo.typePic == PictureType.Ads && numberProgress == 0)
        {
            if (UseProfile.IsVip)
            {
                iconAds.SetActive(false);
                iconAdsMore.SetActive(false);
            }
            else
            {
                if (categoryInfo.CurrentNumberAds > 1)
                {
                    iconAds.SetActive(false);
                    iconAdsMore.SetActive(true);
                    iconAdsMore.GetComponentInChildren<Text>().text = "x" + categoryInfo.CurrentNumberAds;
                }
                else if (categoryInfo.CurrentNumberAds == 1)
                {
                    iconAds.SetActive(true);
                    iconAdsMore.SetActive(false);
                }
                else
                {
                    iconAds.SetActive(false);
                    iconAdsMore.SetActive(false);
                }
            }
        }
        ////Debug.Log("numberProgress" + numberProgress + "|" + total);
        if (GameData.GetUnlockArea(ID))
        {
            complete.SetActive(true);
            isComplete = true;
            gift.SetActive(false);
            fillPercent.transform.parent.gameObject.SetActive(false);
            bg.gameObject.transform.parent.GetComponent<RectTransform>().offsetMin = new Vector2(20, 25);
            //btnView.SetActive(true);
            progress.SetActive(false);
            obLock.SetActive(false);
            bg.material = null;
        }
        else
        {
            bg.material = gray;
            gift.SetActive(true);
            isComplete = false;
            bg.gameObject.transform.parent.GetComponent<RectTransform>().offsetMin = new Vector2(20, 60);
            if (numberProgress != 0)
            {
                fillPercent.transform.parent.gameObject.SetActive(true);
                fillPercent.fillAmount = numberProgress / total;
                //txtPercent.text = "1%";
                //if (fillPercent.fillAmount > 0.01f)
                //{
                //    txtPercent.gameObject.SetActive(true);
                //    txtPercent.text = (int)((numberProgress / total) * 100) + "%";
                //}
            }
            else
            {
                fillPercent.transform.parent.gameObject.SetActive(false);
            }
            int currentPic = GameData.GetCurrentPicInAreas(ID);
            complete.SetActive(false);
            btnView.SetActive(false);
            progress.SetActive(true);
            obLock.SetActive(false);
            float width = 10;
            if (currentPic != 0)
            {
                width = 280 * ((float)currentPic / (float)totalPic);
            }
            fillProgress.rectTransform.sizeDelta = new Vector2(width, fillProgress.rectTransform.rect.height);
            //fillProgress.fillAmount = (float)currentPic / (float)totalPic;
            txtProgress.text = currentPic + "/" + totalPic;
        }

        float aspect = GameData.aspect;
        if (aspect >= 0.68f)
        {
            slider.localScale = new Vector3(0.48f, 0.48f, 0.48f);
        }
        else
        {
            slider.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        }

        txtPercent.text = (int)((numberProgress / total) * 100) + "%";
    }
    public void ViewPic()
    {
        //if (ID < GameData.UnlockAreas)
        //{
        AnalyticsController.LogSelectArea(ID.ToString());
        if (GameData.GetUnlockArea(ID))
        {
            int oldAreas = GameData.PaintingCateAreas;
            GameData.PaintingCateAreas = IdCateArea;
            if (HomeController.instance.contentViewNormalArea.transform.childCount != 0)
                Destroy(HomeController.instance.contentViewNormalArea.transform.GetChild(0).gameObject);
            if (HomeController.instance.contentViewNewArea.childCount != 0)
            {
                Debug.Log("ID" + ID);
                Destroy(HomeController.instance.contentViewNewArea.GetChild(0).gameObject);
            }
            HomeController.instance.panelViewArea.SetActive(true);
            AreasPic originPic = Resources.Load<AreasPic>("Areas/" + ID);
            AreasPic pic = (Instantiate(originPic, originPic.isJigsaw ? HomeController.instance.contentViewNewArea : HomeController.instance.contentViewNormalArea));
            pic.InitArea(GameController.Instance.dataAreas.cateItems[GameData.PaintingCateAreas].dataPic.CategoryInfos[ID.ToString()].PictureInfos.ToArray());
            foreach (Transform child in pic.posObjects)
            {
                child.GetComponent<Button>().enabled = false;
            }
            pic.bg.sprite = bg.sprite;
            GameData.IdAreaChoice = ID;
            pic.ViewPic();
            GameData.PaintingCateAreas = oldAreas;
        }
        else
        {

            if (ID == 34)
            {
                if (iconAds.activeInHierarchy)
                {
                    GameController.Instance.admobAds.ShowVideoReward(SelectSuccesAreas, ShowFail, null, ActionWatchVideo.UnlockAreaEvent, null);
                }
                else
                    SelectSuccesAreas();
            }
            else
            {
                if (UseProfile.IsVip)
                {
                    SelectSuccesAreas();
                }
                else
                {
                    if (categoryInfo.CurrentNumberAds > 0)
                    {
                        GameController.Instance.admobAds.ShowVideoReward(SelectSuccesAreas, ShowFail, null, ActionWatchVideo.UnlockAreaEvent, null);
                    }
                    else
                        GameController.Instance.admobAds.ShowInterstitial(actionIniterClose: () =>
                        {
                            SelectSuccesAreas();
                        }, actionWatchLog: "select_area");
                }
            }
        }
        //}
    }
    void SelectSuccesAreas()
    {
        GameData.isDrawPixel = false;
        if (categoryInfo.CurrentNumberAds > 0)
        {
            categoryInfo.CurrentNumberAds--;
            iconAdsMore.GetComponentInChildren<Text>().text = "x" + categoryInfo.CurrentNumberAds;
            if (categoryInfo.CurrentNumberAds == 1)
            {
                iconAds.SetActive(true);
                iconAdsMore.SetActive(false);
            }
        }
        if (categoryInfo.CurrentNumberAds == 0)
        {
            iconAds.SetActive(false);
            if (categoryInfo.typePic == PictureType.Ads)
                DailyQuestController.instance.UpdateProgressQuest(TypeQuest.UnlockPic);
            GameData.PaintingAreas = ID;
            GameData.IdAreaChoice = ID;
            HomeController.instance.SetActiveTab(2);
            if (HomeController.instance.posBG.childCount != 0)
            {
                Destroy(HomeController.instance.posBG.GetChild(0).gameObject);
            }
            if (HomeController.instance.contentNewArea.childCount != 0)
            {
                Destroy(HomeController.instance.contentNewArea.GetChild(0).gameObject);
            }
            GameData.PaintingCateAreas = IdCateArea;
            HomeController.instance.LoadAreasHome();
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

