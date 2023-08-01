using DG.Tweening;
using EventDispatcher;
using MoreMountains.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif


public class GamePlayControl : MonoBehaviour
{
    public static GamePlayControl Instance;
    [SerializeField] Transform contentNumberColor;
    [SerializeField] ButtonNumberColor preNumberColor;
    public bool isGameComplete;
    [SerializeField] Slider slider;
    float zoom;
    public GenerateMask numberColoring;
    public MainCamera mainCamera;
    [SerializeField] Camera main, fixedCam;
    public GameObject eventHandler;
    public Transform posChoice;
    public int selectedNumber;
    public int tempSelectNumber;
    public int totalPixelPainting;
    [SerializeField] Transform parentNumberColor;
    public bool useItemStar;
    public bool useItemBomb;
    public bool useFinishPic;
    public bool useItemPen;
    public bool lockCamera;
    public int percent;
    int totalPen = 300;
    public bool loseItem;
    public bool freeItemPen;
    bool freeItemFind;
    public bool useEraser;
    //public Transform currentButtonChoice;
    public int selectEraser = 0;

    //EVENT TIMER
    public EventFarmGameplayGuide eventFarmGameplayGuide;
    public GameObject eventTimerGameObject;
    public Text eventTimerText;
    public float eventTimerCounter;
    public bool isShowingPopupGameTimeout;
    public bool isTimeCounterFrozen;
    public bool isSetClockAnimation;
    public SkeletonGraphic clockSkeletonGraphic;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        GameController.Instance.musicManager.PlayBGMusic();
        Debug.Log("GamePlayControl Start");
        //musicManager.PlayBGMusic();
        if (GameData.picChoice.Completed && !GameController.Instance.isTest)
        {
            UIGameController.instance.panelComplete.SetActive(true);
            numberColoring.gameObject.SetActive(false);
        }
        else
        {
            SuggestGetMoreItem();
        }

        if (freeItemFind || freeItemPen)
        {
            StartCoroutine(CountTimeFreeItem(Item.FreeFind.ToString()));
            StartCoroutine(CountTimeFreeItem(Item.FreePen.ToString()));
        }
        if (GameData.isSelectInEventFarm && GameData.picChoice.IdArea == CategoryConst.FARM)
        {
            PopupEventFarmBuilding.instance.Close();
            isTimeCounterFrozen = false;
            isSetClockAnimation = false;
            eventTimerGameObject.SetActive(true);
            ResetEventTimer();
            StartCoroutine(DelayTryShowFarmTutorial());
        }
        else
        {
            isTimeCounterFrozen = true;
            eventTimerGameObject.SetActive(false);
        }

        var allAudioListener = FindObjectsOfType<AudioListener>();
        for (int i = 0; i < allAudioListener.Length; i++)
        {
            if (i == 0)
                allAudioListener[i].enabled = true;
            else
                allAudioListener[i].enabled = false;
        }

        Input.multiTouchEnabled = true;
    }

    private IEnumerator DelayTryShowFarmTutorial()
    {
        yield return new WaitForSeconds(0.5f);
        if (GameData.isSelectInEventFarm && GameData.picChoice.IdArea == CategoryConst.FARM)
        {
            Debug.Log(GameData.isSelectInEventFarm);
            ResetEventTimer();
            eventFarmGameplayGuide.TryShowTutorial();
        }
    }

    public void ResetEventTimer()
    {
        eventTimerGameObject.SetActive(true);
        isShowingPopupGameTimeout = false;
        eventTimerCounter = GameData.farmPicTimeLimit;
        eventTimerText.text = string.Format("{0:00}:{1:00}", Mathf.Floor(eventTimerCounter / 60), Mathf.Floor(eventTimerCounter % 60));
        eventTimerText.color = new Color(202f / 255f, 99f / 255f, 0f, 1);
    }

    void SuggestGetMoreItem()
    {
        if (UseProfile.IsVip)
        {
            return;
        }
        if (GameData.isShowHeadPhone && (!GameController.Instance.useProfile.OnSound || !GameController.Instance.useProfile.OnMusic))
        {
            PopupHeadPhones.Setup().Show();
            GameData.isShowHeadPhone = false;
        }
        else if (GameData.ItemStar <= 0 && GameData.ItemPen <= 0)
        {
            PopupGetMoreWand.Setup().Show();
        }
        else if (GameData.ItemBomb <= 0 && GameData.ItemPen <= 0)
        {
            PopupGetMoreBomb.Setup().Show();
        }
    }

    private void Update()
    {
        if (isGameComplete || !GenerateMask.instance.loadSuccess)
            return;
#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //OnMainBackClick();
        }
#endif

        if (isGameComplete) return;

        //Event Time Counter
        if (GameData.isSelectInEventFarm && GameData.picChoice.IdArea == CategoryConst.FARM)
        {
            if (!isTimeCounterFrozen)
            {
                eventTimerCounter -= Time.deltaTime;
            }
            //set string equal second convert to minute and second format, put '0' if second < 10 and put ':' between minute and second and put '0' if minute < 10
            if (eventTimerCounter > 0f)
                eventTimerText.text = string.Format("{0:00}:{1:00}", Mathf.Floor(eventTimerCounter / 60), Mathf.Floor(eventTimerCounter % 60));
            if (eventTimerCounter < 15f)
            {
                if (!isSetClockAnimation)
                {
                    isSetClockAnimation = true;
                    clockSkeletonGraphic.AnimationState.SetAnimation(0, "anim", true);
                }
                eventTimerText.color = Color.red;
            }

            if (eventTimerCounter <= 0f && !isShowingPopupGameTimeout)
            {
                eventTimerText.text = "Run Out";
                isShowingPopupGameTimeout = true;
                PopupGameTimeout.Setup().Show();
            }
        }

        if (!Application.isMobilePlatform)
        {
            main.orthographicSize = slider.value * (mainCamera.minOrthographicSize - mainCamera.maxOrthographicSize) + mainCamera.maxOrthographicSize;
            fixedCam.orthographicSize = main.orthographicSize;
        }

        if (!Application.isMobilePlatform)
        {
            if (slider.value != zoom)
            {
                zoom = slider.value;
                numberColoring.ZoomUpdate(zoom);
            }
        }
        else
        {
            float newZoom = (main.orthographicSize - mainCamera.maxOrthographicSize) / (mainCamera.minOrthographicSize - mainCamera.maxOrthographicSize);
            if (newZoom != zoom)
            {
                zoom = newZoom;
                numberColoring.ZoomUpdate(zoom);
            }
        }
    }
    public void GenerateNumberColor()
    {
        List<Color32> colors = new List<Color32>();
        colors = numberColoring.m_colorDist;
        selectedNumber = numberColoring.GetSelectedNumber();
        for (int i = 0; i < colors.Count; i++)
        {
            ButtonNumberColor btn = Instantiate(preNumberColor, contentNumberColor);
            btn.Init(i + 1, colors[i]);
            if (i == selectedNumber - 1)
            {
                posChoice.position = btn.transform.position;
                //currentButtonChoice = btn.transform;
                posChoice.SetParent(btn.transform);
                //currentButtonChoice.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.3f);
            }
        }
    }
    public void SetSelectBtn()
    {
        posChoice.gameObject.SetActive(true);
        //parentNumberColor.GetChild(selectedNumber - 1).transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.3f);
        posChoice.position = parentNumberColor.GetChild(selectedNumber - 1).transform.position;
        posChoice.SetParent(parentNumberColor.GetChild(selectedNumber - 1).transform);
        posChoice.SetAsFirstSibling();
    }
    public void SelectItem(int item)
    {
        switch (item)
        {
            case (int)Item.Star:
                if (useItemStar && (GameData.ItemStar > 0 || UseProfile.IsVip))
                {
                    useItemStar = false;
                    posChoice.gameObject.SetActive(true);
                    UIGameController.instance.choiceItemStar.SetActive(false);

                }
                else
                if (GameData.ItemStar > 0 || UseProfile.IsVip)
                {
                    posChoice.gameObject.SetActive(false);
                    useItemStar = true;
                    useItemBomb = false;
                    useItemPen = false;
                    UIGameController.instance.choiceItemBomb.SetActive(false);
                    UIGameController.instance.choiceItemPen.SetActive(false);
                    UIGameController.instance.choiceItemStar.SetActive(true);
                    //UIGameController.instance.handBomb1.SetActive(false);
                    //if (UIGameController.instance.tutHandStar.activeInHierarchy)
                    //{
                    //    UIGameController.instance.tutHandNumberColor.SetActive(true);
                    //    lockCamera = true;
                    //    //UIGameController.instance.tutHandNumberColor.transform.position = 
                    //}
                }
                else
                {
                    //#if UNITY_EDITOR
                    //                    AddItem(0);
                    //#endif
                    if (GameData.Gem >= 6)
                    {
                        GameData.Gem -= 6;
                        this.PostEvent(EventID.CHANGE_VALUE_GEM);
                        AddItem(0);
                        posChoice.gameObject.SetActive(false);
                        useItemStar = true;
                        useItemBomb = false;
                        useItemPen = false;
                        UIGameController.instance.choiceItemBomb.SetActive(false);
                        UIGameController.instance.choiceItemPen.SetActive(false);
                        UIGameController.instance.choiceItemStar.SetActive(true);
                    }
                    else
                    {
                        Vector2 pos = new Vector2(UIGameController.instance.showAdsItemStar.transform.position.x - 0.1f, UIGameController.instance.showAdsItemStar.transform.position.y);
                        GameController.Instance.admobAds.ShowVideoReward(() => { AddItem(0); }, () => { FailLoadAdsReward(pos); }, () => { }, ActionWatchVideo.AddItemStar, GameData.picChoice.Id);
                        //Show ads
                    }
                }
                break;
            case (int)Item.Bomb:
                if (useItemBomb && (GameData.ItemBomb > 0 || UseProfile.IsVip))
                {
                    useItemBomb = false;
                    posChoice.gameObject.SetActive(true);
                    UIGameController.instance.choiceItemBomb.SetActive(false);

                }
                else if (GameData.ItemBomb > 0 || UseProfile.IsVip)
                {
                    posChoice.gameObject.SetActive(false);
                    useItemBomb = true;
                    useItemPen = false;
                    useItemStar = false;
                    UIGameController.instance.choiceItemBomb.SetActive(true);
                    UIGameController.instance.choiceItemStar.SetActive(false);
                    UIGameController.instance.choiceItemPen.SetActive(false);
                    if (!TutorialHand.FinishTutItem)
                    {
                        //UIGameController.instance.handBomb1.SetActive(false);
                        //UIGameController.instance.handBomb2.SetActive(true);
                        StartCoroutine(Helper.StartAction(() => UIGameController.instance.handBomb2.SetActive(false), 4));
                    }
                }
                else
                {
                    //#if UNITY_EDITOR
                    //                    AddItem(1);
                    //#endif
                    if (GameData.Gem >= 6)
                    {
                        GameData.Gem -= 6;
                        this.PostEvent(EventID.CHANGE_VALUE_GEM);
                        AddItem(1);
                        posChoice.gameObject.SetActive(false);
                        useItemBomb = true;
                        useItemPen = false;
                        useItemStar = false;
                        UIGameController.instance.choiceItemBomb.SetActive(true);
                        UIGameController.instance.choiceItemStar.SetActive(false);
                        UIGameController.instance.choiceItemPen.SetActive(false);
                    }
                    else
                    {
                        Vector2 pos = new Vector2(UIGameController.instance.showAdsItemBomb.transform.position.x - 0.5f, UIGameController.instance.showAdsItemStar.transform.position.y);
                        GameController.Instance.admobAds.ShowVideoReward(() => { AddItem(1); }, () => { FailLoadAdsReward(pos); }, () => { }, ActionWatchVideo.AddItemBomb, GameData.picChoice.Id);
                    }
                }
                break;
            case (int)Item.Find:
                if (GameData.ItemFind > 0 || freeItemFind || UseProfile.IsVip)
                {
                    if (!freeItemFind && !UseProfile.IsVip)
                        GameData.ItemFind--;
                    GenerateMask.instance.ZoomAndMoveCamLookAtTileSelected();
                    AnalyticsController.LogUseItem(Item.Find, GameData.picChoice.Id);
                    DailyQuestController.instance.UpdateProgressQuest(TypeQuest.UseItem);
                    DailyQuestController.instance.UpdateProgressQuest(TypeQuest.UseItemFind);
                    AchievementController.instance.UpdateProgressQuest(TypeQuest.PixelFinder);
                    UIGameController.instance.SetTotalFind();
                    //UIGameController.instance.handBomb1.SetActive(false);
                }
                else
                {
                    //#if UNITY_EDITOR
                    //                    AddItem(2);
                    if (GameData.Gem >= 6)
                    {
                        GameData.Gem -= 6;
                        this.PostEvent(EventID.CHANGE_VALUE_GEM);
                        AddItem(2);
                    }
                    else
                    {
                        Vector2 pos = new Vector2(UIGameController.instance.showAdsItemFind.transform.position.x + 0.3f, UIGameController.instance.showAdsItemStar.transform.position.y);
                        //#endif
                        GameController.Instance.admobAds.ShowVideoReward(() => { AddItem(2); }, () => { FailLoadAdsReward(pos); }, () => { }, ActionWatchVideo.AddItemFind, GameData.picChoice.Id);
                    }
                }
                break;
            case (int)Item.Pen:
                break;
            case (int)Item.Eraser:
                selectEraser++;
                if (selectEraser % 2 == 0)
                {
                    useEraser = false;
                    UIGameController.instance.choiceEraser.SetActive(false);
                }
                else
                {
                    useEraser = true;
                    UIGameController.instance.choiceEraser.SetActive(true);
                }
                break;
        }
        //UIGameController.instance.tutHandStar.SetActive(false);
    }
    public void ExitButtonPen()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        useItemPen = false;
        posChoice.gameObject.SetActive(true);
        UIGameController.instance.choiceItemPen.SetActive(false);
        loseItem = false;
        UIGameController.instance.SetFillItemPen(totalPen);
    }
    public void EnterButtonPen()
    {
        if (mainCamera.brush)
            return;
        if (GameData.ItemPen > 0 || freeItemPen || UseProfile.IsVip)
        {
            posChoice.gameObject.SetActive(false);
            useItemBomb = false;
            useItemStar = false;
            useItemPen = true;
            index = 0;
            UIGameController.instance.handBomb1.SetActive(false);
            UIGameController.instance.choiceItemBomb.SetActive(false);
            UIGameController.instance.choiceItemPen.SetActive(true);
            UIGameController.instance.choiceItemStar.SetActive(false);
            if (coroutine != null)
                StopCoroutine(coroutine);
            int width = GameData.picChoice.XCells;
            int hight = GameData.picChoice.YCells;
            checkUnPainteds = new List<CheckUnPainted>();
            for (int i = hight - 1; i >= 0; i--)
            {
                for (int j = 0; j < width; j++)
                {
                    if (numberColoring.checkPainted[j, i] == 0 && GameData.CurColorTexture.GetPixel(j, i) != Color.white)
                    {
                        CheckUnPainted checkUnPainted = new CheckUnPainted();
                        checkUnPainted.x = j;
                        checkUnPainted.y = i;
                        checkUnPainteds.Add(checkUnPainted);
                    }
                }
            }
        }
        else
        {
            if (GameData.Gem >= 6)
            {
                GameData.Gem -= 6;
                this.PostEvent(EventID.CHANGE_VALUE_GEM);
                AddItem(3);
            }
            else
            {
                Vector2 pos = new Vector2(UIGameController.instance.showAdsItemPen.transform.position.x + 0.5f, UIGameController.instance.showAdsItemPen.transform.position.y);
                GameController.Instance.admobAds.ShowVideoReward(() => { AddItem(3); }, () => { FailLoadAdsReward(pos); }, () => { }, ActionWatchVideo.AddItemPen, GameData.picChoice.Id);
            }
        }
        if (useItemPen)
        {
            coroutine = StartCoroutine(PaintPen());
            AnalyticsController.LogUseItem(Item.Pen, GameData.picChoice.Id);
        }
    }

    Coroutine coroutine;
    List<CheckUnPainted> checkUnPainteds = new List<CheckUnPainted>();
    public class CheckUnPainted
    {
        public int x;
        public int y;
    }
    int index = 0;
    IEnumerator PaintPen()
    {
        Debug.Log("index" + index + "heckUnPainteds.Count" + checkUnPainteds.Count);
        while (totalPen > 0 && index < checkUnPainteds.Count)
        {
            UIGameController.instance.handBomb2.SetActive(false);
            TutorialHand.FinishTutItem = true;
            totalPen--;
            UIGameController.instance.SetFillItemPen(totalPen);
            int x = checkUnPainteds[index].x;
            int y = checkUnPainteds[index].y;
            Color color = GameData.CurColorTexture.GetPixel(x, y);
            //Vector3 scale = numberColoring.transform.localScale;
            Vector3 boundsSize = numberColoring.m_grayRenderer.bounds.size;
            Vector2 posCenter = numberColoring.m_grayRenderer.transform.position;
            float posX = (posCenter.x - boundsSize.x / 2) + x * (boundsSize.x / GameData.picChoice.XCells);
            float posY = (posCenter.y - boundsSize.y / 2) + y * (boundsSize.y / GameData.picChoice.YCells);
            numberColoring.SetPixelColorByClick(x, y, color, new Vector2(posX, posY));
            index++;
            yield return new WaitForEndOfFrame();
        }
        GameData.ItemPen--;
        totalPen = 300;
        AchievementController.instance.UpdateProgressQuest(TypeQuest.PenPainter);
        UIGameController.instance.SetTotalPen();

        //if (totalPen <= 0)
        //{
        //    loseItem = false;
        //    useItemPen = false;
        //    UIGameController.instance.choiceItemPen.SetActive(false);
        //    totalPen = 150;
        //    UIGameController.instance.SetFillItemPen(totalPen);
        //}
    }
    private void OnDisable()
    {
        if (totalPen < 300 && totalPen > 0)
            GameData.ItemPen--;
    }
    public void FailLoadAdsReward(Vector2 pos)
    {
        Debug.Log("close");
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
    (
       pos,
        "Video is not available",
        Color.green,
        isSpawnItemPlayer: true
    );
        //UIGameController.instance.animNotVideo.gameObject.SetActive(true);
        //UIGameController.instance.animNotVideo.transform.position = pos.position;
        //UIGameController.instance.animNotVideo.enabled = true;
        //UIGameController.instance.animNotVideo.transform.GetChild(0).gameObject.SetActive(true);
        //UIGameController.instance.animNotVideo.Rebind();
    }
    public bool UseItem()
    {
        return useItemBomb || useItemStar || useFinishPic;
    }
    private readonly int[] dx = new int[] { -1, -1, 0, 1, 1, 1, 0, -1 };
    private readonly int[] dy = new int[] { 0, 1, 1, 1, 0, -1, -1, -1 };
    public IEnumerator PaintStar(int x, int y, int number)
    {
        DailyQuestController.instance.UpdateProgressQuest(TypeQuest.UseItemStar);
        DailyQuestController.instance.UpdateProgressQuest(TypeQuest.UseItem);
        AchievementController.instance.UpdateProgressQuest(TypeQuest.StarPainter);
        lockCamera = false;
        //UIGameController.instance.tutHandStar.SetActive(false);
        //UIGameController.instance.tutHandNumberColor.SetActive(false);
        if (GameData.ItemStar <= 0 && !UseProfile.IsVip)
        {
            PopupSuggestAdsWand.Setup().Show();
        }
        else
        {
            GameController.Instance.musicManager.PlayUseItem();
            int width = GameData.picChoice.XCells;
            int heigh = GameData.picChoice.YCells;
            //GameData.SetUseItem(GameData.keyImageChoice, 1);
            Color color1 = GameData.CurColorTexture.GetPixel(x, y);
            List<Tile> totalTileUnpaint = numberColoring.pixelPerNumber[number];
            if (!UseProfile.IsVip)
            {
                GameData.ItemStar--;
                this.PostEvent(EventID.CHANGE_VALUE_ITEM_WAND);
            }
            Vector3 boundsSize = numberColoring.m_grayRenderer.bounds.size;
            Vector2 posCenter = numberColoring.m_grayRenderer.transform.position;
            //int index = 0;
            //int current = 0;
            float posX = (posCenter.x - boundsSize.x / 2) + x * (boundsSize.x / GameData.picChoice.XCells);
            float posY = (posCenter.y - boundsSize.y / 2) + y * (boundsSize.y / GameData.picChoice.YCells);
            for (int i = 0; i < totalTileUnpaint.Count; i++)
            {
                int x1 = totalTileUnpaint[i].x;
                int y1 = totalTileUnpaint[i].y;
                //if (x == x1 && y==y1)
                //{
                //    current = o
                //}

                numberColoring.SetPixelItem(x1, y1, color1, new Vector2(posX, posY));
                //index++;
                //if (index >= 5)
                //{
                //    yield return null;
                //    index = 0;
                //}
            }
            float process = (float)(numberColoring.totalPainted + totalPixelPainting) / (float)numberColoring.totalPixel;
            UIGameController.instance.SetFillCombo(new Vector2(posX, posY), color1);
            UIGameController.instance.SetProgressComplete(process, totalPixelPainting, numberColoring.totalPixel);
            this.PostEvent(EventID.DOPAINT, numberColoring.numTileUnPainted[number]);
            ((Texture2D)numberColoring.m_resMaterial.mainTexture).Apply();
            AchievementController.instance.UpdateProgressQuest(TypeQuest.Pixels);
        }
        yield return null;
        AnalyticsController.LogUseItem(Item.Star, GameData.picChoice.Id);
    }
    public IEnumerator PaintByStar(int x, int y, int number)
    {
        DailyQuestController.instance.UpdateProgressQuest(TypeQuest.UseItemStar);
        DailyQuestController.instance.UpdateProgressQuest(TypeQuest.UseItem);
        AchievementController.instance.UpdateProgressQuest(TypeQuest.StarPainter);
        lockCamera = false;
        //UIGameController.instance.tutHandStar.SetActive(false);
        //UIGameController.instance.tutHandNumberColor.SetActive(false);
        if (GameData.ItemStar <= 0 && !UseProfile.IsVip)
        {
            PopupSuggestAdsWand.Setup().Show();
            yield return null;

        }
        else
        {
            GameController.Instance.musicManager.PlayUseItem();
            Queue<Tuple<int, int>> q = new Queue<Tuple<int, int>>();
            int width = GameData.picChoice.XCells;
            int heigh = GameData.picChoice.YCells;
            bool[,] passed = new bool[width, heigh];
            passed[x, y] = true;
            q.Enqueue(new Tuple<int, int>(x, y));
            //GameData.SetUseItem(GameData.keyImageChoice, 1);
            Color color1 = GameData.CurColorTexture.GetPixel(x, y);
            int index = 0;
            if (!UseProfile.IsVip)
            {
                GameData.ItemStar--;
                this.PostEvent(EventID.CHANGE_VALUE_ITEM_WAND);
            }
            while (q.Count > 0)
            {
                Tuple<int, int> start = q.Dequeue();
                Vector3 boundsSize = numberColoring.m_grayRenderer.bounds.size;
                Vector2 posCenter = numberColoring.m_grayRenderer.transform.position;
                float posX = (posCenter.x - boundsSize.x / 2) + start.Item1 * (boundsSize.x / GameData.picChoice.XCells);
                float posY = (posCenter.y - boundsSize.y / 2) + start.Item2 * (boundsSize.y / GameData.picChoice.YCells);
                numberColoring.SetPixelColorByClick(start.Item1, start.Item2, color1, new Vector2(posX, posY));
                for (int i = 0; i < dx.Length; i++)
                {
                    int u = start.Item1 + dx[i];
                    int v = start.Item2 + dy[i];
                    if (u >= 0 && u < width && v >= 0 && v < heigh && !passed[u, v] && numberColoring.colorsNumber[u, v] == number)
                    {
                        q.Enqueue(new Tuple<int, int>(u, v));
                        passed[u, v] = true;
                    }
                }
                index++;
                if (index == 15)
                {
                    yield return null;
                    index = 0;
                }
            }

        }
        AnalyticsController.LogUseItem(Item.Star, GameData.picChoice.Id);
    }
    public void BtnFinishPic()
    {
        //#if UNITY_EDITOR
        //        FinishPicReward();
        //#endif
        GameController.Instance.admobAds.ShowVideoReward(
            FinishPicReward, () => { FailLoadAdsReward(UIGameController.instance.btnFinishPic.transform.position); }, () => { }, ActionWatchVideo.FinishPic, GameData.picChoice.Id);
    }
    public void FinishPicReward()
    {
        useFinishPic = true;
        for (int i = 0; i < numberColoring.checkPainted.GetLength(0); i++)
        {
            for (int j = 0; j < numberColoring.checkPainted.GetLength(1); j++)
            {
                if (numberColoring.checkPainted[i, j] != 1)
                {
                    Color color = GameData.CurColorTexture.GetPixel(i, j);
                    numberColoring.SetPixelColorByClick(i, j, color, GameData.vtDefault);
                    UIGameController.instance.btnFinishPic.SetActive(false);
                }
            }
        }
    }
    float posX = 0;
    float posY = 0;

    public void PaintBomb(int x, int y, int bombRadius)
    {
        //if (GameController.Instance.useProfile.NewUser)
        //{
        //    lockCamera = false;
        //    UIGameController.instance.tutHandStar.SetActive(false);
        //    UIGameController.instance.tutHandNumberColor.SetActive(false);
        //}
        UIGameController.instance.handBomb2.SetActive(false);
        TutorialHand.FinishTutItem = true;
        DailyQuestController.instance.UpdateProgressQuest(TypeQuest.UseItemBomb);
        DailyQuestController.instance.UpdateProgressQuest(TypeQuest.UseItem);
        AchievementController.instance.UpdateProgressQuest(TypeQuest.BoomPainter);
        if (GameData.ItemBomb <= 0 && !UseProfile.IsVip)
        {
            PopupSuggestAdsBomb.Setup().Show();
            return;
            //useItemBomb = false;
            //posChoice.gameObject.SetActive(true);
        }
        GameController.Instance.musicManager.PlayUseItem();
        int xStart = Mathf.Max(x - bombRadius, 0);
        int xEnd = Mathf.Min(x + bombRadius, GameData.picChoice.XCells - 1);

        int yStart = Mathf.Max(y - bombRadius, 0);
        int yEnd = Mathf.Min(y + bombRadius, GameData.picChoice.YCells - 1);

        for (int i = xStart; i <= xEnd; i++)
        {
            for (int j = yStart; j <= yEnd; j++)
            {
                float distance = Vector2.Distance(new Vector2(i, j), new Vector2(x, y)) + 0.5f;

                if (distance <= (float)bombRadius)
                {
                    int colorNumber = GameData.picChoice.ColorNumbers[j][i];
                    if (colorNumber < GameData.picChoice.Colors.Count && colorNumber != -1)
                    {
                        Color color = GameData.picChoice.Colors[colorNumber];
                        bool playParticle = false;
                        if (!playParticle)
                        {
                            Vector3 boundsSize = numberColoring.m_grayRenderer.bounds.size;
                            Vector2 posCenter = numberColoring.m_grayRenderer.transform.position;
                            posX = (posCenter.x - boundsSize.x / 2) + x * (boundsSize.x / GameData.picChoice.XCells);
                            posY = (posCenter.y - boundsSize.y / 2) + y * (boundsSize.y / GameData.picChoice.YCells);
                            playParticle = true;
                        }
                        numberColoring.SetPixelColorByClick(i, j, color, new Vector2(posX, posY));
                    }
                    //ColorCell(i, j, ActivePictureInfo.ColorNumbers[j][i]);
                }
            }
        }
        float process = (float)(numberColoring.totalPainted + totalPixelPainting) / (float)numberColoring.totalPixel;
        UIGameController.instance.SetProgressComplete(process, totalPixelPainting, numberColoring.totalPixel);
        ((Texture2D)numberColoring.m_resMaterial.mainTexture).Apply();
        if (!UseProfile.IsVip)
        {
            GameData.ItemBomb--;
            this.PostEvent(EventID.CHANGE_VALUE_ITEM_BOMB);
        }
        AnalyticsController.LogUseItem(Item.Bomb, GameData.picChoice.Id);
    }
    public void AddItem(int item)
    {
        if (item == (int)Item.Star)
        {
            GameData.ItemStar += 1;
            AnimEffectItem(Item.Star, 1, mainCamera.transform.position);
            useItemStar = true;
            useItemBomb = false;
            useItemPen = false;
            this.PostEvent(EventID.CHANGE_VALUE_ITEM_WAND);
            UIGameController.instance.choiceItemBomb.SetActive(false);
            UIGameController.instance.choiceItemStar.SetActive(true);
        }
        else if (item == (int)Item.Bomb)
        {
            GameData.ItemBomb += 1;
            AnimEffectItem(Item.Bomb, 1, mainCamera.transform.position);
            useItemBomb = true;
            useItemStar = false;
            useItemPen = false;
            this.PostEvent(EventID.CHANGE_VALUE_ITEM_BOMB);
            UIGameController.instance.choiceItemBomb.SetActive(true);
            UIGameController.instance.choiceItemStar.SetActive(false);
        }
        else if (item == (int)Item.Find)
        {
            GameData.ItemFind += 1;
            AnimEffectItem(Item.Find, 1, mainCamera.transform.position);
            UIGameController.instance.SetTotalFind();
        }
        else if (item == (int)Item.Pen)
        {
            GameData.ItemPen += 1;
            //useItemPen = true;
            //useItemBomb = false;
            //useItemStar = false;
            AnimEffectItem(Item.Pen, 1, mainCamera.transform.position);
            UIGameController.instance.SetTotalPen();
        }
        posChoice.gameObject.SetActive(false);
    }
    void AnimEffectItem(Item item, int value, Vector3 pos)
    {
        GameController.Instance.moneyEffectController.SpawnEffect_FlyUp(pos, item, value.ToString(), Color.black);
    }
    public void ShowFinishLevel()
    {
        GameController.Instance.admobAds.ShowInterstitial(actionIniterClose: () =>
        {
            CloseInterFinishLevel();
        }, actionWatchLog: "complete_level");
    }
    public void CloseInterFinishLevel()
    {
        UIGameController.instance.panelComplete.SetActive(true);
        numberColoring.gameObject.SetActive(false);
        GameData.CountPicCompleteShowIAP++;
        if (UseProfile.IsRemoveAds)
            return;
        if (GameData.CountPicCompleteShowIAP >= 1)
        {
            //PopupRemoveAdsPlus.Setup().Show();
            GameData.CountPicCompleteShowIAP = 0;
        }
    }

    public void UpdateCountPicComplete()
    {
        if (!GameData.isDrawPixel)
            GameData.CountPicComplete++;
        if (GameData.choicePicEvent)
            DataPictureJigsaw.Current++;
        Debug.Log("GameData.CountPicComplete " + GameData.CountPicComplete);
    }
    int isPause = 0;
    private void OnApplicationPause(bool pause)
    {
        if (pause && GameData.CountPicCompleteShowIAP >= 3)
        {
            isPause++;
            if (isPause > 2 && !UseProfile.IsRemoveAds)
            {
                //PopupRemoveAdsPlus.Setup().Show();
                GameData.CountPicCompleteShowIAP = 0;
            }
            else if (isPause > 3 && !GameData.BuyPackIAP(PackIAP.SUBSCRIPTION) && isPause < 4)
            {
                PopupVip.Setup().Show();
            }
        }
    }
    IEnumerator CountTimeFreeItem(string item)
    {
        DateTime nextTime = GameData.GetDateTimeLastFreeItem(item);
        int value = GameData.GetTotalTimeItem(item);
        if (UnbiasedTime.Instance.Now() < nextTime)
        {
            if (item == Item.FreePen.ToString())
            {
                UIGameController.instance.unlimtedPen.SetActive(true);
                freeItemPen = true;
            }
            if (item == Item.FreeFind.ToString())
            {
                UIGameController.instance.unlimtedFind.SetActive(true);
                freeItemFind = true;
            }
            while (UnbiasedTime.Instance.Now() < nextTime)
            {
                yield return new WaitForSeconds(1f);
                value--;
                GameData.SetTotalTimeItem(item, value);
            }
            GameData.SetTotalTimeItem(item, 0);
        }
        else
        {
            GameData.SetTotalTimeItem(item, 0);
            if (item == Item.FreePen.ToString())
            {
                UIGameController.instance.unlimtedPen.SetActive(false);
                freeItemPen = false;
            }
            if (item == Item.FreeFind.ToString())
            {
                freeItemFind = false;
                UIGameController.instance.unlimtedFind.SetActive(false);
            }
        }
    }
}
