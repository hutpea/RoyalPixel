using DG.Tweening;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompletePicController : MonoBehaviour
{
    [SerializeField] Image imgPic;
    [SerializeField] Image fillProgress;
    [SerializeField] GameObject btnClaim;
    [SerializeField] GameObject btnClaimx2;
    Coroutine coroutine;
    private Texture2D texGray;
    [SerializeField] Text txtProgress;
    int index;
    Color[] dataColor;
    int width;
    int height;
    [SerializeField] GameObject fxStar, fxBomb;
    [SerializeField] GameObject showItem;
    [SerializeField] Transform posShow;
    [SerializeField] Text txtBomb;
    [SerializeField] Text txtStar;
    [SerializeField] DataGift dataGift;
    List<GameObject> stars;
    List<GameObject> bombs;
    [SerializeField] Transform posGift;
    List<HistoryStep> historySteps;
    List<HistoryDrawStep> historyDrawSteps;
    private short[] historyIntSteps;

    [SerializeField] GameObject imgGift;
    bool isClaim;
    bool claimed;
    [SerializeField] GameObject btnContinue;
    Coroutine coroutinePlayAnim;
    Coroutine coroutinePlayBomb;
    [SerializeField] Transform pig;
    Coroutine playPic;
    private void OnEnable()
    {
        GameController.Instance.musicManager.PlayWinSound();
        width = GameData.grayScale.width;
        height = GameData.grayScale.height;
        dataColor = new Color[width * height];
        playPic = StartCoroutine(PlayPaintPic());
        btnContinue.SetActive(false);
        if (GameData.isReciveGemPig)
        {
            GameData.isReciveGemPig = false;
            StartCoroutine(Helper.StartAction(() => { pig.gameObject.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "eat", false); }, 1.2f));
            GameController.Instance.moneyEffectController.SpawnEffect_GoDestination(transform.position, Item.Gem, 10,
                null, pig.position);
            StartCoroutine(Helper.StartAction(() => { pig.gameObject.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "idle", true); }, 3));
        }
        if (GameData.CountPicComplete >= 3 && !GameData.ShowFarmEvent)
        {
            PopupSuggestFarmEvent.Setup().Show();
            GameData.ShowFarmEvent = true;
        }
        //else if (GameData.CountPicComplete >= 5 && !GameData.ShowEvent)
        //{
        //    PopupSuggestEvent.Setup().Show();
        //    GameData.ShowEvent = true;
        //}
        //GamePlayControl.Instance.numberColoring.SavePainting();
        if (GameData.GetUnlockArea(GameData.picChoice.IdArea) && GameData.GetReciveGift(GameData.picChoice.IdArea) || GameData.picChoice.SinglePic)
        {
            imgGift.SetActive(false);
            fillProgress.transform.parent.gameObject.SetActive(false);
            return;
        }

        Debug.Log("id " + GameData.picChoice.IdArea);
        float currentPic = GameData.GetCurrentPicInAreas(GameData.picChoice.IdArea);
        float totalPic = GameData.totalPicInAreas;
        txtProgress.text = currentPic + "/" + totalPic;
        float currentFill = (float)(currentPic - 1) / totalPic;
        fillProgress.fillAmount = currentFill;
        fillProgress.DOFillAmount(currentFill + (1f / (float)GameData.totalPicInAreas), 0.2f).OnComplete(() => CheckClaim(fillProgress.fillAmount));
        txtBomb.text = GameData.ItemBomb.ToString();
        txtStar.text = GameData.ItemStar.ToString();

    }

    void CheckClaim(float fill)
    {
        if (fill == 1 && !GameData.GetReciveGift((GameData.picChoice.IdArea)))
        {
            isClaim = true;
            GameData.SetReciveGift(GameData.picChoice.IdArea, true);
            btnClaim.SetActive(true);
            btnClaimx2.SetActive(true);
        }
    }
    public void ShowPigGem()
    {
        PopupPigGem.Setup().Show();
    }
    IEnumerator PlayPaintPic()
    {
        texGray = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texGray.filterMode = FilterMode.Point;
        ResetPic();
        yield return new WaitForSeconds(0.2f);
        History history = GameData.HistoryForPictureId(GameData.picChoice.Id);
        if (GameData.isDrawPixel)
        {
            historyDrawSteps = history.DrawSteps;
        }
        else
            historySteps = history.Steps;
        //historyIntSteps = history.IntSteps;
        int step = 0; float maxStep = 0; int stepResult;
        float frameDuration = 0;
        float result = ((float)(GameData.isDrawPixel ? historyDrawSteps.Count : historySteps.Count - 4)) / 50;
        stepResult = (int)Math.Max(result, 1);
        if (stepResult < 35)
        {
            maxStep = stepResult;
            frameDuration = 0.05f;
        }
        else
        {
            maxStep = 35;
            frameDuration = 0.045f;
        }
        index = 0;
        Texture2D texColorLocal = GameData.CurColorTexture;

        //Debug.LogError("historySteps" + historySteps.Count);
        while (index < (GameData.isDrawPixel ? historyDrawSteps.Count : historySteps.Count))
        {
            //Debug.Log("xy " + xy);
            int x = GameData.isDrawPixel ? historyDrawSteps[index].x : historySteps[index].x;
            int y = GameData.isDrawPixel ? historyDrawSteps[index].y : historySteps[index].y;
            Color color;
            if (GameData.isDrawPixel)
                color = new Color32(historyDrawSteps[index].colorR, historyDrawSteps[index].colorG, historyDrawSteps[index].colorB, 255);
            else
                color = texColorLocal.GetPixel(x, y);
            texGray.SetPixel(x, y, color);
            Rect rect = new Rect(0, 0, width, height);
            imgPic.sprite = Sprite.Create(texGray, rect, new Vector2(0, 0));
            index++;
            step++;
            texGray.Apply();
            if (step == maxStep)
            {
                step = 0;
                yield return new WaitForSeconds(frameDuration / 3);
            }
        }
        yield return null;

    }
    private void OnDisable()
    {
        if (coroutinePlayAnim != null)
            StopCoroutine(coroutinePlayAnim);
        if (coroutinePlayBomb != null)
            StopCoroutine(coroutinePlayBomb);
        GameData.isDrawPixel = false;
    }
    public void PlayBtn()
    {
        if (playPic != null)
            StopCoroutine(playPic);
        playPic = StartCoroutine(PlayPaintPic());
    }
    void ResetPic()
    {
        Rect rect = new Rect(0, 0, width, height);
        Color[] colors = GameData.grayScale.GetPixels();
        int index1 = 0;
        for (int i = 0; i < width * height; i++)
        {
            Color c = new Color(colors[i].r, colors[i].g, colors[i].b, 0.6f);
            dataColor[index1] = c;
            index1++;
        }
        texGray.SetPixels(dataColor);
        texGray.Apply();
        texGray.filterMode = FilterMode.Point;
        imgPic.sprite = Sprite.Create(texGray, rect, new Vector2(0, 0));
        index = 0;
    }
    public void ClaimGiftx2()
    {
        //#if UNITY_EDITOR
        //        ClaimGift(2);
        //#endif
        GameController.Instance.admobAds.ShowVideoReward(() => ClaimGift(2), () => NotLoadVideo(), () => { }, ActionWatchVideo.Claimx2, GameData.picChoice.Id);
    }
    public void NotLoadVideo()
    {
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
   (
      btnClaimx2.transform.position,
       "No Video Ads",
       Color.red,
       isSpawnItemPlayer: true
   );
    }
    public void ClaimGift(int number)
    {
        if (!isClaim || claimed)
            return;
        claimed = true;
        btnContinue.SetActive(true);
        btnClaim.gameObject.SetActive(false);
        btnClaimx2.gameObject.SetActive(false);
        GameController.Instance.musicManager.PlayBtnClaim();
        int giftStar = (int)(GameData.totalPicInAreas * number * 0.75f);
        int giftBomb = (int)(giftStar * 0.75f);
        //GiftItem gift = dataGift.gifts[GameData.picChoice.IdArea - 1];
        stars = new List<GameObject>();
        bombs = new List<GameObject>();
        for (int i = 0; i < giftStar; i++)
        {
            GameObject ob = Instantiate(fxStar, transform);
            ob.SetActive(false);
            stars.Add(ob);
            ob.transform.position = posGift.transform.position;
        }
        for (int i = 0; i < giftBomb; i++)
        {
            GameObject ob = Instantiate(fxBomb, transform);
            ob.SetActive(false);
            ob.transform.position = posGift.transform.position;
            bombs.Add(ob);
        }
        if (gameObject.activeInHierarchy)
        {
            coroutinePlayAnim = StartCoroutine(PlayAnim());
            coroutinePlayBomb = StartCoroutine(PlayAnimBomb());
        }
    }
    IEnumerator PlayAnim()
    {
        int oldStar = GameData.ItemStar;
        GameData.ItemStar = oldStar + stars.Count;
        Vector3 startPos = showItem.transform.position;
        showItem.transform.DOMove(posShow.position, 0.5f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.5f);
        foreach (GameObject child in stars)
        {
            child.SetActive(true);
            child.transform.localScale = new Vector3(0.4f, 0.4f, 1);
            child.transform.DOScale(new Vector3(1, 1, 1), 1);
            child.transform.DOMove(showItem.transform.GetChild(0).position, 0.5f).OnComplete(() =>
            {
                oldStar++;
                txtStar.text = oldStar.ToString();
                child.SetActive(false);
            });
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1f);
        showItem.transform.DOMove(startPos, 1).SetEase(Ease.InBack);
        yield return new WaitForFixedUpdate();
    }
    IEnumerator PlayAnimBomb()
    {
        int oldBomb = GameData.ItemBomb;
        GameData.ItemBomb = oldBomb + bombs.Count;
        Vector3 posStart = btnClaim.transform.position;
        Vector3 startPos = showItem.transform.position;
        yield return new WaitForSeconds(0.5f);
        foreach (GameObject child in bombs)
        {
            child.SetActive(true);
            child.transform.DOMove(showItem.transform.GetChild(1).position, 0.5f).OnComplete(() =>
            {
                oldBomb++;
                txtBomb.text = oldBomb.ToString();
                child.SetActive(false);
            });
            yield return new WaitForSeconds(0.1f);
        }
    }
}
