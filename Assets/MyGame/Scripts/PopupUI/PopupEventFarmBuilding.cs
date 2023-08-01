using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BizzyBeeGames.ColorByNumbers;
using DG.Tweening;
using EventDispatcher;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Tween = DG.Tweening.Tween;

public class PopupEventFarmBuilding : BaseBox
{
    public static PopupEventFarmBuilding instance;
    [Header("UI")] [SerializeField] private GameObject elementPicPrefab;
    public Canvas eventCanvas;
    public Image progressFrame;
    public Image progressFrameFake;
    public Image progressFill;
    public Button chestButton;
    public Text gemText;
    public Slider slider;
    public RectTransform scrollRect_RectTransform;
    public ScrollRect scrollRect;
    public RectTransform mapContain;
    public Transform hand;
    public Button zoomButton;
    public Image zoomIcon;
    public Button locateButton;
    public ParticleSystem fxShowItemParticleSystem;
    public GameObject blockPopupGameObject;
    public GameObject preventClickLayer;
    public EventFarmTutorialController eventFarmTutorialController;
    [Header("Data")] public EventFarmRewardData eventFarmRewardData;
    public EventFarmBuilding_ProgressData progressData;
    public FarmItemUI_EventFarmBuilding currentFarmItem;
    public FarmItemUI_EventFarmBuilding lastShowFarmItem;
    public FarmItemUI_EventFarmBuilding dogItem;
    public RectTransform farItemTutorialRect;
    public List<FarmItemUI_EventFarmBuilding> zone1FarmItems;
    public List<FarmItemUI_EventFarmBuilding> zone2FarmItems;
    public List<FarmItemUI_EventFarmBuilding> zone3FarmItems;
    public List<FarmItemUI_EventFarmBuilding> zone4FarmItems;
    public List<FarmItemUI_EventFarmBuilding> zone5FarmItems;
    public List<Sprite> chestSprites;

    public List<Sprite> zoomSprites;

    //public FarmCameraGroup farmCameraGroup;
    private float zoom;
    private bool firstDetectUncompletedPic;
    private int rewardZone;
    private bool isFarmFinished;
    [HideInInspector] public bool locateButtonIsClickedLastTime;
    private Action onCompleteScrollAction = null;

    public static PopupEventFarmBuilding Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupEventFarmBuilding>(PathPrefabs.POPUP_EVENT_FARMBUILDING));
        }

        /*if (!instance.farmCameraGroup)
        {
            Debug.Log("First attach FarmCameraGroup to PopupEventFarmBuilding");
            instance.farmCameraGroup = farmCameraGroup;
        }
        else
        {
            Debug.Log("FarmCameraGroup already attached to PopupEventFarmBuilding");
        }*/
        return instance;
    }
    bool isPlay;
    public void OnShow()
    {
        Show();
        Input.multiTouchEnabled = false;
        GameData.isInViewEventFarm = true;
        eventCanvas.sortingLayerName = "Popup";
        eventCanvas.sortingOrder = 25;

        if (GameData.CountPicComplete < 3)
        {
            Debug.Log(
                $"You have just win {GameData.CountPicComplete} pictures. You need to win at least 3 pictures to unlock this event");
            HideZone(1);
            HideZone(2);
            HideZone(3);
            HideZone(4);
            HideZone(5);
            blockPopupGameObject.SetActive(true);
            progressFill.transform.parent.gameObject.SetActive(false);
            chestButton.gameObject.SetActive(false);
            return;
        }

        currentFarmItem = null;
        lastShowFarmItem = null;
        rewardZone = 0;
        isFarmFinished = true;
        locateButtonIsClickedLastTime = false;
        blockPopupGameObject.SetActive(false);
        firstDetectUncompletedPic = false;
        GameData.EventFarmRewardClaimable = false;
        GameData.farmPicTimeLimit = 180;
        progressData = GameData.EventFarmBuilding_ProgressData;
        onCompleteScrollAction += () => { eventFarmTutorialController.dialogueController.OnClickLocateButton(); };
        Debug.Log("Show PopupEventFarmBuilding");
        Debug.Log($"Farm Progress Data: {progressData.zone} - {progressData.currentPic}");
        GameController.Instance.musicManager.PlayEventFarmBuilding_BackgroundMusic();
        //eventCanvas.worldCamera = HomeController.instance.mainCamera.GetComponent<Camera>();

        Debug.Log($"--- {GameData.EventFarmTutorial_1_Done} - {GameData.EventFarmTutorial_2_Done} ---");
        if (GameData.EventFarmTutorial_1_Done && GameData.EventFarmTutorial_2_Done)
        {
            Debug.Log("zoom in");
            mapContain.localScale = new Vector3(2f, 2f, 1);
            zoomIcon.sprite = zoomSprites[0];
            isZoomIn = true;
        }
        else
        {
            Debug.Log("zoom out");
            mapContain.localScale = new Vector3(.8f, .8f, 1);
            zoomIcon.sprite = zoomSprites[1];
            isZoomIn = false;
        }

        locateButton.gameObject.SetActive(true);
        zoomButton.gameObject.SetActive(true);

        if (GameData.EventFarmTutorial_2_Done)
        {
            scrollRect.content.anchoredPosition = new Vector2(-900f, scrollRect.content.anchoredPosition.y);
        }

        //CheatPictureHackToZone5();
        
        int index = 0;
        InitZone(1);
        InitZone(2);
        InitZone(3);
        InitZone(4);
        InitZone(5);

        if (isFarmFinished)
        {
            rewardZone = 4;
            if (GameData.EventFarmZone5Claimed == false)
            {
                Debug.Log($"Zone {progressData.zone - 1} is preparing to reward chest!");
                GameData.EventFarmRewardClaimable = true;
                TryShowRewardPopup(5);
            }
            else
            {
                GameData.EventFarmRewardClaimable = false;
            }

            preventClickLayer.SetActive(false);
            Debug.Log("Set false preventClickLayer");
            ToggleHand(false);
            locateButton.gameObject.SetActive(false);
            zoomButton.onClick.RemoveAllListeners();
            zoomButton.onClick.AddListener(delegate
            {
                EndGameZoomClicked(dogItem);
            });
            progressFrame.gameObject.SetActive(false);
            chestButton.gameObject.SetActive(false);
            GameData.FarmTreasureFillAmount = 1;
            DOScroll(dogItem.GetComponent<RectTransform>(), isShowHand: false);
            scrollRect.horizontal = true;
            scrollRect.vertical = false;
            return;
        }
        else
        {
            ShowZone();
            zoomButton.onClick.RemoveAllListeners();
            zoomButton.onClick.AddListener(delegate
            {
                ZoomClicked();
            });

            if (currentFarmItem != null && !GameData.isBackFromEventFarmGameplay)
            {
                //ScrollImmediatedlyToPosition(currentFarmItem.GetComponent<RectTransform>());
                StartCoroutine(Delay_GoToCurrentFarmItem());
            }

            InitFillAmount();
            UpdateUI();
            eventFarmTutorialController.TryShowTutorial();
            var allAudioListener = FindObjectsOfType<AudioListener>();
            for (int i = 0; i < allAudioListener.Length; i++)
            {
                if (i == 0)
                    allAudioListener[i].enabled = true;
                else
                    allAudioListener[i].enabled = false;
            }

            if (currentFarmItem != null && lastShowFarmItem == null)
            {
                if (GameData.EventFarmTutorial_1_Done && GameData.EventFarmTutorial_2_Done)
                {
                    ZoomInAtItem(currentFarmItem);
                    isZoomIn = true;
                    zoomIcon.sprite = zoomSprites[0];
                }
                else
                {
                    ZoomOutAtItem(currentFarmItem);
                    isZoomIn = false;
                    zoomIcon.sprite = zoomSprites[1];
                }

                preventClickLayer.SetActive(false);
            }

            if (currentFarmItem == null)
            {
                preventClickLayer.SetActive(false);
                Debug.Log("Current Item NULL");
            }
        }

        if (PopupDailyReward.instance != null)
            PopupDailyReward.instance.Close();

        GameData.isBackFromEventFarmGameplay = false;
        TutorialHand.instance.handAchiviement.SetActive(false);
        HomeController.instance.canvasMain.SetActive(false);
    }
    public override void Close()
    {
        base.Close();
    }
    public void EnableHomeMusic()
    {
        HomeController.instance.canvasMain.SetActive(true);
    }
    private IEnumerator Delay_DisablePreventClickLayer(float duration)
    {
        yield return new WaitForSeconds(duration);
        preventClickLayer.SetActive(false);
        Debug.Log($"Delay {duration} set prevent click false");
    }

    private void InitZone(int zoneID, bool zoneCompleted = false)
    {
        List<FarmItemUI_EventFarmBuilding> selectedZone = new List<FarmItemUI_EventFarmBuilding>();
        switch (zoneID)
        {
            case 1:
                selectedZone = zone1FarmItems;
                break;
            case 2:
                selectedZone = zone2FarmItems;
                break;
            case 3:
                selectedZone = zone3FarmItems;
                break;
            case 4:
                selectedZone = zone4FarmItems;
                break;
            case 5:
                selectedZone = zone5FarmItems;
                break;
        }

        for (int i = 0; i < selectedZone.Count; i++)
        {
            /*Debug.Log(GameController.Instance);
            Debug.Log(GameController.Instance.dataEventFarm);
            Debug.Log(GameController.Instance.dataEventFarm.CategoryInfos["FARM_ZONE_" + zoneID.ToString()]);
            Debug.Log(i);
            Debug.Log(GameController.Instance.dataEventFarm.CategoryInfos["FARM_ZONE_" + zoneID.ToString()].PictureInfos.Count);*/
            PictureInformation pictureInformation =
                GameController.Instance.dataEventFarm.CategoryInfos["FARM_ZONE_" + zoneID.ToString()].PictureInfos[i];
            if (pictureInformation.Completed)
            {
                lastShowFarmItem = selectedZone[i];
                selectedZone[i].ShowRealItem();
                var allElePicOfItem = selectedZone[i].GetComponentsInChildren<ElementPic>();
                if (allElePicOfItem.Length > 0)
                {
                    for (int d = 0; d < allElePicOfItem.Length; d++)
                    {
                        Destroy(allElePicOfItem[d].gameObject);
                    }
                }
            }
            else
            {
                //Not complete this Pic yet, pick first uncompleted pic
                if (!firstDetectUncompletedPic)
                {
                    isFarmFinished = false;
                    firstDetectUncompletedPic = true;
                    progressData.zone = zoneID;
                    progressData.currentPic = i;
                    if (progressData.currentPic == 0 && progressData.zone != 1)
                    {
                        rewardZone = progressData.zone - 2;
                        switch (progressData.zone)
                        {
                            case 2:
                                {
                                    if (GameData.EventFarmZone1Claimed == false)
                                    {
                                        GameData.EventFarmRewardClaimable = true;
                                        TryShowRewardPopup(1);
                                    }
                                    else
                                    {
                                        GameData.EventFarmRewardClaimable = false;
                                    }

                                    break;
                                }
                            case 3:
                                {
                                    if (GameData.EventFarmZone2Claimed == false)
                                    {
                                        GameData.EventFarmRewardClaimable = true;
                                        TryShowRewardPopup(2);
                                    }
                                    else
                                    {
                                        GameData.EventFarmRewardClaimable = false;
                                    }

                                    break;
                                }
                            case 4:
                                {
                                    if (GameData.EventFarmZone3Claimed == false)
                                    {
                                        GameData.EventFarmRewardClaimable = true;
                                        TryShowRewardPopup(3);
                                    }
                                    else
                                    {
                                        GameData.EventFarmRewardClaimable = false;
                                    }

                                    break;
                                }
                            case 5:
                                {
                                    if (GameData.EventFarmZone4Claimed == false)
                                    {
                                        GameData.EventFarmRewardClaimable = true;
                                        TryShowRewardPopup(4);
                                    }
                                    else
                                    {
                                        GameData.EventFarmRewardClaimable = false;
                                    }

                                    break;
                                }
                            default:
                                GameData.EventFarmRewardClaimable = false;
                                break;
                        }

                        if (GameData.EventFarmRewardClaimable)
                        {
                            Debug.Log($"Zone {progressData.zone - 2} is preparing to reward chest!");
                        }
                    }


                    currentFarmItem = selectedZone[i];

                    if (GameData.HistoryForPictureId(pictureInformation.Id) != null)
                    {
                        GameData.HistoryForPictureId(pictureInformation.Id).RemoveHistory();
                    }

                    var firstElePicOfItem = selectedZone[i].GetComponentsInChildren<ElementPic>().FirstOrDefault();
                    ElementPic itemElementPic;
                    itemElementPic = firstElePicOfItem != null
                        ? firstElePicOfItem
                        : Instantiate(elementPicPrefab, selectedZone[i].transform).GetComponent<ElementPic>();
                    selectedZone[i].elementPic = itemElementPic;
                    selectedZone[i].elementPic.InitPic(pictureInformation);
                    if (pictureInformation.TotalPixel > 0)
                    {
                        GameData.farmPicTimeLimit = Mathf.Max(pictureInformation.TotalPixel / 3, 60);
                    }
                    else
                    {
                        GameData.farmPicTimeLimit = 300;
                    }

                    Debug.Log("Item Time Limit: " + selectedZone[i].itemTimeLimit);
                    selectedZone[i].elementPic.GetComponent<Button>().onClick.RemoveAllListeners();
                    selectedZone[i].ElementPicButtonAddListener();
                    selectedZone[i].ToggleElementPicButton(true);
                    selectedZone[i].HideAllItemImages();
                }
                else
                {
                    selectedZone[i].HideAllItemImages();
                }
            }
        }
    }

    private void ShowZone()
    {
        for (int z = 1; z <= 5; z++)
        {
            List<FarmItemUI_EventFarmBuilding> selectedZone = new List<FarmItemUI_EventFarmBuilding>();
            switch (z)
            {
                case 1:
                    selectedZone = zone1FarmItems;
                    break;
                case 2:
                    selectedZone = zone2FarmItems;
                    break;
                case 3:
                    selectedZone = zone3FarmItems;
                    break;
                case 4:
                    selectedZone = zone4FarmItems;
                    break;
                case 5:
                    selectedZone = zone5FarmItems;
                    break;
            }

            for (int i = 0; i < selectedZone.Count; i++)
            {
                PictureInformation pictureInformation =
                    GameController.Instance.dataEventFarm.CategoryInfos["FARM_ZONE_" + z.ToString()].PictureInfos[i];
                if (pictureInformation.Completed && selectedZone[i] != lastShowFarmItem)
                {
                    selectedZone[i].ShowRealItem(isImmediate: true);
                }
                else
                {
                    goto End_Show_Zone;
                }
            }
        }

    End_Show_Zone:
        if (GameData.isBackFromEventFarmGameplay)
        {
            StartCoroutine(ShowLastItem());
        }
        else
        {
            if (lastShowFarmItem)
            {
                lastShowFarmItem.ShowRealItem(isImmediate: true);
            }
            Debug.Log("set prevent click = false");
            preventClickLayer.SetActive(false);
        }

        return;
    }

    private IEnumerator ShowLastItem()
    {
        if (lastShowFarmItem)
        {
            Debug.Log("SHOW LAST ITEM");
            lastShowFarmItem.HideAllItemImages();
            ToggleHand(false);
            if (GameData.EventFarmTutorial_2_Done)
            {
                ScrollImmediatedlyToPosition(lastShowFarmItem.GetComponent<RectTransform>());
            }

            Debug.Log("zone: " + progressData.zone + " pic: " + progressData.currentPic);
            if (progressData.zone == 1 && progressData.currentPic < 2)
            {
                lastShowFarmItem.ShowRealItem(isImmediate: true);
                //mapContain.localScale = new Vector3(0.8f, 0.8f, 1);
                DOScroll(lastShowFarmItem.GetComponent<RectTransform>());
                StartCoroutine(Delay_DisablePreventClickLayer(0.5f));
                yield break;
            }

            Debug.Log("(1) set prevent click = true");
            preventClickLayer.SetActive(true);

            yield return new WaitForSeconds(0.3f);

            lastShowFarmItem.ShowRealItem(isImmediate: true, isDOFade: true);
            ShowFXOnRect(lastShowFarmItem.GetComponent<RectTransform>());
            ZoomInAtItem(lastShowFarmItem, isShowHand: false);
            isZoomIn = true;
            zoomButton.interactable = false;

            yield return new WaitForSeconds(1.8f);

            Debug.Log("Current item = " + currentFarmItem.name);
            if (currentFarmItem != null && GameData.EventFarmTutorial_2_Done)
            {
                ZoomInAtItem(currentFarmItem, isShowHand: true);
                isZoomIn = true;
                StartCoroutine(Delay_DisablePreventClickLayer(1f));
            }
            else
            {
                Debug.Log("set prevent click = false");
                StartCoroutine(Delay_DisablePreventClickLayer(1f));
            }

            zoomButton.interactable = true;
            StartCoroutine(Delay_DisablePreventClickLayer(1f));
        }
    }

    public void HideZone(int id)
    {
        switch (id)
        {
            case 1:
                {
                    foreach (var _farmItemUI in zone1FarmItems)
                    {
                        _farmItemUI.HideAllItemImages();
                    }

                    break;
                }
            case 2:
                {
                    foreach (var _farmItemUI in zone2FarmItems)
                    {
                        _farmItemUI.HideAllItemImages();
                    }

                    break;
                }
            case 3:
                {
                    foreach (var _farmItemUI in zone3FarmItems)
                    {
                        _farmItemUI.HideAllItemImages();
                    }

                    break;
                }
        }
    }

    public void TryShowRewardPopup(int zoneID)
    {
        Debug.Log(
            $"Zone claimed: {GameData.EventFarmZone1Claimed} {GameData.EventFarmZone2Claimed} {GameData.EventFarmZone3Claimed} {GameData.EventFarmZone4Claimed} {GameData.EventFarmZone5Claimed}");
        Debug.Log("Try show reward popup ... GameData.EventFarmRewardClaimable = " + GameData.EventFarmRewardClaimable);
        if (GameData.EventFarmRewardClaimable && !CheckZoneIsClaimedReward(zoneID))
        {
            Debug.Log("Claiming reward farm...");
            GameData.EventFarmRewardClaimable = false;
            int rewardZoneIndex = zoneID - 1;
            rewardZoneIndex = Mathf.Clamp(rewardZoneIndex, 0, eventFarmRewardData.allZoneRewards.Count - 1);
            Debug.Log("Reward zone = " + rewardZone);
            var rewards = eventFarmRewardData.allZoneRewards[rewardZoneIndex];

            switch (zoneID)
            {
                case 1:
                    {
                        Debug.Log("Set zone 1 claimed");
                        GameData.EventFarmZone1Claimed = true;
                        break;
                    }
                case 2:
                    {
                        Debug.Log("Set zone 2 claimed");
                        GameData.EventFarmZone2Claimed = true;
                        break;
                    }
                case 3:
                    {
                        Debug.Log("Set zone 3 claimed");
                        GameData.EventFarmZone3Claimed = true;
                        break;
                    }
                case 4:
                    {
                        Debug.Log("Set zone 4 claimed");
                        GameData.EventFarmZone4Claimed = true;
                        break;
                    }
                case 5:
                    {
                        Debug.Log("Set zone 5 claimed");
                        GameData.EventFarmZone5Claimed = true;
                        break;
                    }
            }

            AddReward(rewards.zoneRewardList[0]);
            PopupRewardFarm.Setup(rewards.zoneRewardList[0]).OnShow();
        }
    }

    public bool CheckZoneIsClaimedReward(int zoneID)
    {
        switch (zoneID)
        {
            case 1:
                {
                    return GameData.EventFarmZone1Claimed;
                }
            case 2:
                {
                    return GameData.EventFarmZone2Claimed;
                }
            case 3:
                {
                    return GameData.EventFarmZone3Claimed;
                }
            case 4:
                {
                    return GameData.EventFarmZone4Claimed;
                }
            case 5:
                {
                    return GameData.EventFarmZone5Claimed;
                }
        }

        return true;
    }

    public void AddReward(RewardDatabase.Reward reward)
    {
        RewardDatabase.Claim(reward, isShowPopup: false);
        UpdateUI();
    }

    public void InitFillAmount()
    {
        progressFill.transform.parent.gameObject.SetActive(true);
        chestButton.gameObject.SetActive(true);
        switch (progressData.zone)
        {
            case 1:
                progressFill.fillAmount = (float)progressData.currentPic / zone1FarmItems.Count;
                break;
            case 2:
                progressFill.fillAmount = (float)progressData.currentPic / zone2FarmItems.Count;
                break;
            case 3:
                progressFill.fillAmount = (float)progressData.currentPic / zone3FarmItems.Count;
                break;
            case 4:
                progressFill.fillAmount = (float)progressData.currentPic / zone4FarmItems.Count;
                break;
            case 5:
                progressFill.fillAmount = (float)progressData.currentPic / zone5FarmItems.Count;
                break;
        }

        GameData.FarmTreasureFillAmount = progressFill.fillAmount;
    }

    public void UpdateUI()
    {
        gemText.text = GameData.Gem.ToString();
    }

    public void OnClickLocateButton()
    {
        locateButtonIsClickedLastTime = true;
        GoToCurrentFarmItem();
    }

    public IEnumerator Delay_EnableScroll()
    {
        yield return new WaitForSeconds(1f);
        scrollRect.horizontal = true;
    }

    public IEnumerator Delay_GoToCurrentFarmItem()
    {
        yield return new WaitForSeconds(0.1f);
        GoToCurrentFarmItem();
    }

    public bool isZoomIn;

    public void ZoomClicked()
    {
        isZoomIn = !isZoomIn;
        if (isZoomIn)
        {
            mapContain.DOKill();
            ZoomInAtItem(currentFarmItem, isShowHand: true);
            StartCoroutine(Delay_DisablePreventClickLayer(1f));
        }
        else
        {
            mapContain.DOKill();
            ZoomOutAtItem(currentFarmItem, isShowHand: true);
            StartCoroutine(Delay_DisablePreventClickLayer(1f));
        }
    }

    public void EndGameZoomClicked(FarmItemUI_EventFarmBuilding farmItem)
    {
        isZoomIn = !isZoomIn;
        if (isZoomIn)
        {
            mapContain.DOKill();
            ZoomInAtItem(farmItem, isShowHand: false);
            StartCoroutine(Delay_DisablePreventClickLayer(1f));
        }
        else
        {
            mapContain.DOKill();
            ZoomOutAtItem(farmItem, isShowHand: false);
            StartCoroutine(Delay_DisablePreventClickLayer(1f));
        }
    }

    [Button]
    public void ZoomInAtItem(FarmItemUI_EventFarmBuilding farmItemUI, bool isShowHand = false,
        UnityAction zoomEndAction = null)
    {
        Debug.Log("Zoom In At Item called");
        preventClickLayer.SetActive(true);
        ToggleHand(false);
        Debug.Log("prevent click = true");
        scrollRect.vertical = true;
        var farmItemRect = farmItemUI.GetComponent<RectTransform>();
        mapContain.DOScale(2f, 0.5f).OnUpdate(delegate
        {
            DOScroll(farmItemRect, 0.05f, isShowHand, false, onEndAction: delegate { zoomEndAction?.Invoke(); },
                is2D: true);
        }).OnComplete(delegate { });
        zoomIcon.sprite = zoomSprites[0];
    }

    [Button]
    public void ZoomOutAtItem(FarmItemUI_EventFarmBuilding farmItemUI, bool isShowHand = false,
        UnityAction zoomEndAction = null)
    {
        Debug.Log("Zoom Out At Item called");
        preventClickLayer.SetActive(true);
        ToggleHand(false);
        Debug.Log("prevent click = true");
        scrollRect.vertical = false;
        var farmItemRect = farmItemUI.GetComponent<RectTransform>();
        mapContain.DOScale(1f, 0.5f).OnUpdate(delegate
        {
            DOScroll(farmItemRect, 0.05f, isShowHand, false, onEndAction: delegate { zoomEndAction?.Invoke(); });
        }).OnComplete(delegate { });
        zoomIcon.sprite = zoomSprites[1];
    }

    public void GoToCurrentFarmItem()
    {
        if (currentFarmItem != null)
        {
            Debug.Log("Scroll to " + currentFarmItem.name);
            DOScroll(currentFarmItem.GetComponent<RectTransform>(), isShowHand: true, is2D: isZoomIn);
        }
    }

    private void ScrollImmediatedlyToPosition(RectTransform targetRect)
    {
        RectTransform scrollRectRectTransform = scrollRect.GetComponent<RectTransform>();
        var scrollRectContentPosition =
            (Vector2)scrollRect.transform.InverseTransformPoint(scrollRect.content.position);
        var targetPosition = (Vector2)scrollRect.transform.InverseTransformPoint(targetRect.position);
        var halfScreenVector2 = new Vector2(scrollRect.GetComponent<RectTransform>().rect.width / 2f, 0);
        var result = scrollRectContentPosition - targetPosition + halfScreenVector2;
        var contentLimitPos = -(scrollRect.content.rect.width - scrollRect.GetComponent<RectTransform>().rect.width);
        result.x = Mathf.Clamp(result.x, contentLimitPos, 0f);
        scrollRect.content.anchoredPosition = result;
    }

    [Button]
    public void DOScroll(RectTransform targetRect, float duration = 0.5f, bool isShowHand = true,
        bool isSimulateClick = false, UnityAction onStartAction = null, UnityAction onEndAction = null,
        bool is2D = false)
    {
        Debug.Log("DOSCroll to " + targetRect.gameObject.name);
        if (isSimulateClick)
        {
            locateButtonIsClickedLastTime = true;
        }

        Canvas.ForceUpdateCanvases();
        if (isShowHand)
        {
            hand.GetComponent<RectTransform>().localScale = Vector3.zero;
        }
        else
        {
            ToggleHand(false);
        }

        eventFarmTutorialController.dialogueController.isBusy = true;
        RectTransform scrollRectRectTransform = scrollRect.GetComponent<RectTransform>();
        var scrollRectContentPosition =
            (Vector2)scrollRect.transform.InverseTransformPoint(scrollRect.content.position);
        var targetPosition = (Vector2)scrollRect.transform.InverseTransformPoint(targetRect.position);
        if (!is2D)
        {
            scrollRectContentPosition.y = 0;
            targetPosition.y = 0;
        }

        var halfScreenVector2 = new Vector2(scrollRect.GetComponent<RectTransform>().rect.width / 2f, 0);
        var result = scrollRectContentPosition - targetPosition + halfScreenVector2;
        var contentLimitPos = -(scrollRect.content.rect.width - scrollRect.GetComponent<RectTransform>().rect.width);
        result.x = Mathf.Clamp(result.x, contentLimitPos, 0f);
        scrollRect.inertia = false;
        //Debug.Log("result " + result);

        Tweener moveTweener;
        moveTweener = scrollRect.content.DOAnchorPos(result, duration).SetEase(Ease.InOutQuad).OnStart(delegate
        {
            onStartAction?.Invoke();
        }).OnComplete(delegate
        {
            scrollRect.inertia = true;
            if (isShowHand)
            {
                hand.SetParent(targetRect, worldPositionStays: false);
                hand.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                hand.GetComponent<RectTransform>().DOScale(1, 0.1f);
                hand.DOMove(targetRect.position, 0.05f);
                ToggleHand(true);
            }

            eventFarmTutorialController.dialogueController.isBusy = false;
            if (locateButtonIsClickedLastTime)
            {
                onCompleteScrollAction?.Invoke();
            }

            locateButtonIsClickedLastTime = false;
            onEndAction?.Invoke();
        });
        /*moveTweener.OnUpdate(delegate
        {
            var newRes = result;
            newRes = scrollRectContentPosition - targetPosition + halfScreenVector2;
            var contentLimitPos = -(scrollRect.content.rect.width - scrollRectRectTransform.rect.width);
            newRes.x = Mathf.Clamp(newRes.x, contentLimitPos, 0f);
            Debug.Log(scrollRectContentPosition + " " + result);
            moveTweener.ChangeEndValue(newRes);
        });*/
    }

    public void ShowFXOnRect(RectTransform targetRect)
    {
        StopCoroutine(DelayFXCoroutine());
        fxShowItemParticleSystem.gameObject.SetActive(true);
        if (isZoomIn)
        {
            fxShowItemParticleSystem.transform.localScale = new Vector3(2, 2, 2);
        }
        else
        {
            fxShowItemParticleSystem.transform.localScale = new Vector3(1, 1, 1);
        }

        fxShowItemParticleSystem.transform.SetParent(targetRect.transform, worldPositionStays: false);
        fxShowItemParticleSystem.transform.position = targetRect.transform.position;
        StartCoroutine(DelayFXCoroutine());
    }

    private IEnumerator DelayFXCoroutine()
    {
        fxShowItemParticleSystem.Play();
        yield return new WaitForSeconds(3f);
        fxShowItemParticleSystem.Stop();
        fxShowItemParticleSystem.gameObject.SetActive(false);
    }

    /*private void Update()
    {
        if (farmCameraGroup == null) return;
        if (!Application.isMobilePlatform)
        {
            farmCameraGroup.mainCamera.orthographicSize =
                slider.value * (farmCameraGroup.mainCameraScript.minOrthographicSize -
                                farmCameraGroup.mainCameraScript.maxOrthographicSize) +
                farmCameraGroup.mainCameraScript.maxOrthographicSize;
            farmCameraGroup.fixedCamera.orthographicSize = farmCameraGroup.mainCamera.orthographicSize;
            //mapContain.localScale = Vector3.one * farmCameraGroup.mainCamera.orthographicSize;
        }

        if (!Application.isMobilePlatform)
        {
            if (slider.value != zoom)
            {
                zoom = slider.value;
            }
        }
        else
        {
            float newZoom =
                (farmCameraGroup.mainCamera.orthographicSize - farmCameraGroup.mainCameraScript.maxOrthographicSize) /
                (farmCameraGroup.mainCameraScript.minOrthographicSize -
                 farmCameraGroup.mainCameraScript.maxOrthographicSize);
            if (newZoom != zoom)
            {
                zoom = newZoom;
            }
        }
    }*/

    public void ShowShop()
    {
        GameController.Instance.musicManager.PlayBtnClick();
        PanelShopController.Setup().Show();
    }

    public IEnumerator DelayAction(UnityAction action, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        action?.Invoke();
    }

    private void ToggleHand(bool value)
    {
        if (value && GameData.DisableHandInFarm)
        {
            hand.gameObject.SetActive(false);
            return;
        }
        hand.gameObject.SetActive(value);
    }

    public void OnClose()
    {
        Debug.Log("PopupEventFarmBuilding Close");
        GameController.Instance.musicManager.PlayBGMusic();
        GameData.isInViewEventFarm = false;
        Input.multiTouchEnabled = true;
        AreaController.instance?.UpdateBannerTreasureFillImg();
        PanelShopController.instance?.Close();
        Close();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        UpdateUI();
    }

    [Button]
    public void CheatPictureHackToZone5()
    {
        for (int h = 1; h <= 5; h++)
        {
            List<FarmItemUI_EventFarmBuilding> selectedZone = new List<FarmItemUI_EventFarmBuilding>();
            switch (h)
            {
                case 1:
                    selectedZone = zone1FarmItems;
                    break;
                case 2:
                    selectedZone = zone2FarmItems;
                    break;
                case 3:
                    selectedZone = zone3FarmItems;
                    break;
                case 4:
                    selectedZone = zone4FarmItems;
                    break;
                case 5:
                    selectedZone = zone5FarmItems;
                    break;
            }
            if (h != 5)
            {
                for (int i = 0; i < selectedZone.Count; i++)
                {
                    /*Debug.Log(GameController.Instance);
                    Debug.Log(GameController.Instance.dataEventFarm);
                    Debug.Log(GameController.Instance.dataEventFarm.CategoryInfos["FARM_ZONE_" + zoneID.ToString()]);
                    Debug.Log(i);
                    Debug.Log(GameController.Instance.dataEventFarm.CategoryInfos["FARM_ZONE_" + zoneID.ToString()].PictureInfos.Count);*/
                    PictureInformation pictureInformation =
                        GameController.Instance.dataEventFarm.CategoryInfos["FARM_ZONE_" + h.ToString()]
                            .PictureInfos[i];
                    pictureInformation.Completed = true;
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    /*Debug.Log(GameController.Instance);
                    Debug.Log(GameController.Instance.dataEventFarm);
                    Debug.Log(GameController.Instance.dataEventFarm.CategoryInfos["FARM_ZONE_" + zoneID.ToString()]);
                    Debug.Log(i);
                    Debug.Log(GameController.Instance.dataEventFarm.CategoryInfos["FARM_ZONE_" + zoneID.ToString()].PictureInfos.Count);*/
                    PictureInformation pictureInformation =
                        GameController.Instance.dataEventFarm.CategoryInfos["FARM_ZONE_" + h.ToString()]
                            .PictureInfos[i];
                    pictureInformation.Completed = true;
                }
            }
        }
    }
}

[Serializable]
public class EventFarmBuilding_ProgressData
{
    public int zone;
    public int currentPic;
}

[Serializable]
public class EventFarmBuilding_ZoneClaimedData
{
    public int zone1Claimed;
    public int zone2Claimed;
    public int zone3Claimed;
    public int zone4Claimed;
    public int zone5Claimed;
}