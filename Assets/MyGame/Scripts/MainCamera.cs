using DG.Tweening;
using EventDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainCamera : MonoBehaviour
{
    private Vector3 dragDelta;
    private Vector3 beginTouchPosition;
    private Vector3 beginCamPosition;

    public Camera mainCamera;
    public Camera fixedCam;
    [HideInInspector]
    public float coef1, coef2, coef0;
    public bool dragging, isDragLocked;

    public float minOrthographicSize = 0.8f, maxOrthographicSize = 8;
    public float borderMax;
    private Vector3 startPos, startCam;
    public bool stopDraw, holdCam, mutilTouch, touchZoom;
    float minX, maxX, minY, maxY;
    float deltaMagnitudeDiff;
    float widthStartCam, heighStartCam;
    public bool up, brush, tap, hold;
    //[SerializeField] TutorialSuggest newTutorial;
    public enum TapState
    {
        None,
        Down,
        Drag,
        Multitouch,
        BrushMode
    }
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0.0f;
        float disX = magnitude * mainCamera.orthographicSize / maxOrthographicSize;
        while (elapsed < duration)
        {
            disX *= -1;
            //Debug.Log("disX " + disX);
            float x = this.transform.localPosition.x + disX;
            float y = /*Random.Range(-0.2f, 0.2f) * magnitude +*/ this.transform.localPosition.y;
            transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += 0.02f;
            yield return new WaitForSeconds(0.1f);
        }
        transform.position = originalPos;
    }
    public static TapState CurrentTapState
    {
        get;
        private set;
    }
    private void Start()
    {

    }
    private void Awake()
    {
        float aspectCam = GameData.aspect;
        if (aspectCam < 0.51f)
        {
            maxOrthographicSize = 2.4f;
        }
        else if (aspectCam < 0.57f)
        {
            maxOrthographicSize = 1.9f;
        }
        else if (aspectCam >= 0.69)
        {
            maxOrthographicSize = 1.6f;
        }
        else
        {
            maxOrthographicSize = 2.4f;
        }
        mainCamera.orthographicSize = maxOrthographicSize;
        startPos = gameObject.transform.position;
        startCam = startPos;
        Debug.Log("=============mainCamera " + mainCamera.orthographicSize);
    }
    private void OnEnable()
    {
        ResetCam();
    }
    public void ResetCam()
    {
        transform.position = startCam;
        autoZoom = false;
        heighStartCam = mainCamera.orthographicSize;
        widthStartCam = mainCamera.aspect * heighStartCam;
    }

    private void OnDisable()
    {
        transform.position = startCam;
        mainCamera.orthographicSize = maxOrthographicSize;
        fixedCam.orthographicSize = maxOrthographicSize;
    }
    bool checkFinishZoom;
    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("zoom auto");
        //if (GameData.newPlayer && !newTutorial.finishZoomTut && GameData.newTutorial) {
        //    if (mainCamera.orthographicSize <= minOrthographicSize + 1.2f) {
        //        mainCamera.DOOrthoSize(minOrthographicSize, 0.2f).OnComplete(() => { fixedCam.orthographicSize = mainCamera.orthographicSize; FishZoomTut(); });
        //    }
        //}
        //if (checkFinishZoom)
        //{

        //    //FishZoomTut();
        //}
        if (autoZoom)
            autoZoom = false;
        timer = 0;
        stopDraw = false;
        //Debug.Log("drag up");
        dragging = false;
        holdCam = false;
    }
    //void FishZoomTut() {
    //    if (mainCamera.orthographicSize <= minOrthographicSize && !newTutorial.finishZoomTut) {
    //        newTutorial.finishZoomTut = true;
    //        newTutorial.FinishZoomTut();
    //        newTutorial.MoveCamFinishZoom(transform.position);
    //    }
    //}
    public void OnBeginDrag()
    {
        //Debug.Log("begin drag" + "stopDraw" + stopDraw + "dragging" + dragging);
        if (autoZoom)
            return;
        //if (isDragLocked) return;
        bool touchCheck = Input.touchCount == 1 && Application.isMobilePlatform || !Application.isMobilePlatform;
        if (Input.touchCount == 2)
        {
            beginTouchPosition = (fixedCam.ScreenToWorldPoint(Input.GetTouch(0).position) + fixedCam.ScreenToWorldPoint(Input.GetTouch(1).position)) / 2;
            beginCamPosition = transform.position;
            //dragging = true;
        }
        else if (touchCheck)
        {
            beginTouchPosition = fixedCam.ScreenToWorldPoint(Input.mousePosition);
            beginCamPosition = transform.position;
        }

    }

    public void TapPixel(Vector3 posMouseDown)
    {
        //Debug.Log("tap pixel 1" + dragging + "||" + stopDraw + "||" + mutilTouch + "||" + CheckOrthographicSizeZoom() + "||" + IsPointerOverUIObject(GameController.instance.eventHandler));
        if (!dragging && !stopDraw && !mutilTouch && !CheckOrthographicSizeZoom() && !IsPointerOverUIObject(GamePlayControl.Instance.eventHandler))
        {
            brush = false;
#if UNITY_EDITOR
            if (true/*((GameData.newTutorial && newTutorial.finishZoomTut && !tap) || (!GameData.newTutorial || !GameData.newPlayer))*/)
            {
                tap = true;
                GamePlayControl.Instance.numberColoring.TryClickPixel(posMouseDown);
            }
#else
            if (true/*Input.touchCount == 1 && ((GameData.newTutorial && newTutorial.finishZoomTut) || (!GameData.newTutorial || !GameData.newPlayer))*/)
            {
                tap = true;
                GamePlayControl.Instance.numberColoring.TryClickPixel(posMouseDown); 
            }
#endif
        }
        mutilTouch = false;
        dragging = false;
        stopDraw = false;
        holdCam = false;
    }
    bool onDrag;
    public void OnDrag(PointerEventData eventData)
    {
        if (autoZoom || holdCam)
            return;
        float timeCondition = 0;
#if UNITY_ANDROID
        timeCondition = 0.15f;
#else
        timeCondition = 0.1f;
#endif
        if (Time.time - timer < timeCondition && !onDrag)
        {
            stopDraw = true;
            dragging = true;
            onDrag = true;
        }
        else if (!onDrag)
        {
            dragging = false;
            if (!mutilTouch)
            {
                stopDraw = false;
                if (!GamePlayControl.Instance.UseItem())
                {
                    brush = true;
#if UNITY_EDITOR
                    if (true/*((GameData.newTutorial && newTutorial.finishTapTut) || (!GameData.newPlayer || !GameData.newTutorial))*/)
                    {
                        hold = true;
                        Vector2 vector = eventData.position - eventData.delta;
                        float[] obj = new float[3]
                        {
                                    1f,
                                    0f,
                                    0f
                        };
                        Vector2 delta = eventData.delta;
                        obj[1] = Mathf.Abs(delta.x);
                        Vector2 delta2 = eventData.delta;
                        obj[2] = Mathf.Abs(delta2.y);
                        float num = Mathf.Max(obj);
                        Vector2 a = eventData.delta / num;
                        Vector2 a2 = vector;
                        for (int i = 1; (float)i < num; i++)
                        {
                            GamePlayControl.Instance.numberColoring.TryClickPixel(a2 + i * a);
                        }

                    }
#else
                                        if (Input.touchCount == 1 /*&& ((GameData.newTutorial && newTutorial.finishTapTut) || (!GameData.newPlayer || !GameData.newTutorial))*/)
                                                {
                                                    hold = true;
                                                     Vector2 vector = eventData.position - eventData.delta;
                                            float[] obj = new float[3]
                                            {
                                    1f,
                                    0f,
                                    0f
                                            };
                                            Vector2 delta = eventData.delta;
                                            obj[1] = Mathf.Abs(delta.x);
                                            Vector2 delta2 = eventData.delta;
                                            obj[2] = Mathf.Abs(delta2.y);
                                            float num = Mathf.Max(obj);
                                            Vector2 a = eventData.delta / num;
                                            Vector2 a2 = vector;
                                            for (int i = 1; (float)i < num; i++)
                                            {
                                                GamePlayControl.Instance.numberColoring.TryClickPixel(a2 + i * a);
                                            }
                                                }
#endif
                }
            }
        }
        if (!stopDraw && !dragging || mainCamera.orthographicSize >= maxOrthographicSize) return;
        Vector3 mouse = fixedCam.ScreenToWorldPoint(Input.mousePosition);
        dragDelta = fixedCam.ScreenToWorldPoint(Input.mousePosition) - beginTouchPosition;
        dragDelta.z = 0;
        Vector3 temp = new Vector3((beginCamPosition.x - dragDelta.x), beginCamPosition.y - dragDelta.y, startCam.z);
        Vector2 move = new Vector2(temp.x * dragSpeed, temp.y * dragSpeed);
        if (vt != temp)
            dragSpeed = 1;
        //else  
        //{
        //    dragSpeed -= Time.deltaTime;
        //    if (dragSpeed < Time.deltaTime)
        //        dragSpeed = 0;
        //}
        vt = temp;
        //Debug.Log(move + "|" + temp);
        //transform.Translate(move * Time.deltaTime);
        //transform.GetComponent<Rigidbody2D>().velocity = move;
        transform.position = new Vector3(beginCamPosition.x - dragDelta.x, beginCamPosition.y - dragDelta.y, startCam.z);
    }
    public void OnEndDrag()
    {
        onDrag = false;
        brush = false;
        dragSpeed = 3;
    }
    float dragSpeed = 3;
    Vector3 vt;
    private void LateUpdate()
    {
        if (GamePlayControl.Instance.isGameComplete)
            return;
        CheckZoomMinCam(GamePlayControl.Instance.numberColoring.m_grayRenderer.transform.position);
        Vector3 temp = transform.position;
        temp.x = Mathf.Clamp(temp.x, minX, maxX);
        temp.y = Mathf.Clamp(temp.y, minY, maxY);
        temp.z = startCam.z;
        transform.position = temp;
    }
    float timer;
    [SerializeField]
    private LayerMask m_raycastLayer;
    float countTime;
    bool show;
    void Update()
    {
        //if (autoZoom || (GameData.newTutorial && newTutorial.finishZoomTut && GameData.newPlayer))
        //    return;
        if (GamePlayControl.Instance.isGameComplete /*|| GamePlayControl.Instance.lockCamera*/)
            return;
        if (Input.touchCount == 1 /*&& !GameData.newTutorial*/)
        {

            countTime += Time.deltaTime;
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began && !IsPointerOverUIObject(GamePlayControl.Instance.eventHandler))
            {
                if (!TutorialHand.FinishTutItem && !show)
                {
                    show = true;
                    UIGameController.instance.handBomb2.SetActive(false);
                    //UIGameController.instance.handBomb1.SetActive(true);
                }
                StartCoroutine(AutoZoomStart());
                timer = Time.time;
            }
        }
        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            countTime = 0;
            mutilTouch = true;
            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);
            stopDraw = true;
            dragging = true;
            holdCam = false;
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            mainCamera.orthographicSize += deltaMagnitudeDiff * (mainCamera.orthographicSize / 10f) * Time.deltaTime * 0.7f;
            mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minOrthographicSize, maxOrthographicSize);
            fixedCam.orthographicSize = mainCamera.orthographicSize;
            if (t1.phase == TouchPhase.Ended || t2.phase == TouchPhase.Ended || t1.phase == TouchPhase.Canceled || t2.phase == TouchPhase.Canceled)
                holdCam = true;
            if (t1.phase == TouchPhase.Ended && t2.phase == TouchPhase.Ended || t1.phase == TouchPhase.Canceled && t2.phase == TouchPhase.Canceled)
            {
                timer = 0;
                dragSpeed = 3;
                stopDraw = false;
                dragging = false;
                onDrag = false;
                mutilTouch = false;
                //FishZoomTut();
            }
            if (mainCamera.orthographicSize >= maxOrthographicSize)
            {
                autoZoom = false;
            }
        }
        //if (mainCamera.orthographicSize <= minOrthographicSize && GameData.newPlayer && GameData.newTutorial) {

        //    checkFinishZoom = true;
        //}
    }
    public bool isZoom;
    public void ZoomOut()
    {
        if (!isZoom)
        {
            ZoomPos((minOrthographicSize + maxOrthographicSize) / 3, transform.position);
            UIGameController.instance.SetUIZoom(true);
        }
        else
        {
            ZoomIn();
        }
    }
    public void ZoomIn()
    {
        if (isZoom)
        {
            StartCoroutine(ZoomInFinish());
            UIGameController.instance.SetUIZoom(false);
        }
    }
    IEnumerator AutoZoomStart()
    {
        yield return new WaitForSeconds(0.05f);
        if (Input.touchCount == 1 && CheckOrthographicSizeZoom())
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 50f, m_raycastLayer))
            {
                stopDraw = true;
                StartCoroutine(ZoomCamFocus((minOrthographicSize + maxOrthographicSize) / 3, mainCamera.ScreenToWorldPoint(Input.mousePosition)));
            }
            //if (GameController.Instance.useProfile.NewUser)
            //{
            //    UIGameController.instance.tutHandStar.SetActive(true);
            //}

        }
    }
    public void ZoomCamFinish()
    {
        //if (GameData.picChoice.SinglePic )
        //{
        //    this.PostEvent(EventID.PAINTED_PIC, GameData.picChoice);
        //}
        UIGameController.instance.btnBack.interactable = false;
        StartCoroutine(ZoomInFinish());
        DailyQuestController.instance.UpdateProgressQuest(TypeQuest.FinishPic);
        GamePlayControl.Instance.UpdateCountPicComplete();
        StartCoroutine(Helper.StartAction(() => FinishLevel(), 1.2f));
        GameController.Instance.AnalyticsController.LogLevelComplete(GameData.picChoice.Id.ToString());
    }
    void FinishLevel()
    {
        if (!GameData.isDrawPixel)
        {
            PopupCompleteLevel completeLevel = PopupCompleteLevel.Setup();
            completeLevel.Show();
            completeLevel.OnCloseBox = () => { GamePlayControl.Instance.CloseInterFinishLevel(); };
        }
        else
            GamePlayControl.Instance.ShowFinishLevel();
        AnalyticsController.LogComplete(GameData.picChoice.Id);
        UIGameController.instance.btnBack.interactable = true;
    }

    //public void ShowRateGame() {
    //    if (AdsController.Instance.CanShowInterstitial) {
    //        bool interstitialReady = AdsController.Instance.IsInterstitialAvailable;
    //        if (interstitialReady) {
    //            AdsController.Instance.ShowInterstitial(ShowSuccesInterAds, ShowSuccesInterAds, "Rate_Game");
    //        } else {
    //            ShowSuccesInterAds();
    //        }
    //        GameAnalytics.InterstitialAdsImpression("complete_pic", interstitialReady, Application.internetReachability != NetworkReachability.NotReachable);
    //    } else
    //        ShowSuccesInterAds();
    //}
    //void ShowSuccesInterAds() {
    //    if (GameData.rateGame) {
    //        if (GameData.removeAds || GameData.vip) {
    //            UIGamePlay.instance.ShowPanel(CanvasControll.instance.panelShare);
    //            GameController.instance.transform.parent.gameObject.SetActive(false);
    //        } else {

    //            PopupKeepAds popup = PopupKeepAds.Setup();
    //            popup.Show();
    //            popup.OnCloseBox = () => {
    //                UIGamePlay.instance.ShowPanel(CanvasControll.instance.panelShare);
    //                GameController.instance.transform.parent.gameObject.SetActive(false);
    //            };
    //        }
    //    } else {
    //        PopupRate popupRate = CanvasControll.instance.popupRate;
    //        UIGamePlay.instance.ShowPanel(popupRate.gameObject);
    //        UIGamePlay.instance.ShowPanelAnim(UIGamePlay.instance.panelRate.transform.GetChild(0).gameObject);
    //        popupRate.InitAndShow();
    //    }
    //}
    IEnumerator ZoomInFinish()
    {
        float sizeCam = mainCamera.orthographicSize;
        mainCamera.transform.DOMove(startPos, 0.6f);
        while (sizeCam <= maxOrthographicSize)
        {
            sizeCam += 0.1f;
            if (sizeCam <= maxOrthographicSize)
            {
                mainCamera.orthographicSize = sizeCam;
                fixedCam.gameObject.SetActive(true);
                fixedCam.orthographicSize = mainCamera.orthographicSize;
            }
            yield return new WaitForEndOfFrame();
        }
    }
    bool autoZoom;
    public IEnumerator ZoomCamFocus(float endScale, Vector3 pos)
    {
        Debug.Log("zoom");
        autoZoom = true;
        ZoomPos(endScale, pos);
        yield return null;
    }
    public void ZoomPos(float endScale, Vector3 pos)
    {
        float height = 2f * (minOrthographicSize + 0.2f);
        float width = height * mainCamera.aspect;
        float minX1 = -widthStartCam + width / 2;
        float maxX1 = +widthStartCam - width / 2;
        float minY1 = -heighStartCam + height / 2;
        float maxY1 = +heighStartCam - height / 2;
        float tempX = Mathf.Clamp(pos.x, minX1, maxX1);
        float tempY = Mathf.Clamp(pos.y, minY1, maxY1);
        transform.DOMove(new Vector3(tempX, tempY, -startCam.z), 0.2f).SetEase(Ease.Linear).OnComplete(() => SetAutoZoom());
        mainCamera.DOOrthoSize(endScale, 0.2f).SetEase(Ease.Linear).OnComplete(() => fixedCam.orthographicSize = mainCamera.orthographicSize);
    }
    public bool CheckOrthographicSizeZoom()
    {
        return mainCamera.orthographicSize >= maxOrthographicSize;
    }
    public static bool IsPointerOverUIObject(GameObject exceptObject)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        foreach (var go in results)
        {
            if (go.gameObject != exceptObject) return true;
        }
        return false;
    }
    public void CheckZoomMinCam(Vector2 grayTex)
    {
        //grayTex 
        float height = 2f * mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;
        float heighTex = GenerateMask.instance.m_grayRenderer.bounds.size.y;
        //x = grayTex.x - widthStart + width / 2
        minX = grayTex.x - widthStartCam + width / 2 - 0.5f;
        maxX = grayTex.x + widthStartCam - width / 2 + 0.5f;
        minY = Mathf.Min(startCam.y, grayTex.y - (heighTex / 2) + (height / 2) - 0.25f * (1 / mainCamera.aspect) - 0.5f);
        maxY = Mathf.Max(startCam.y, grayTex.y + heighTex / 2 - (height / 2) + 0.25f * (1 / mainCamera.aspect));
    }
    void SetAutoZoom()
    {
        if (Input.touchCount == 0)
            autoZoom = false;
    }
}
