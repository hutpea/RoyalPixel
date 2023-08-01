using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using Firebase.RemoteConfig;

public class LuckySpin : BaseBox
{
    [SerializeField] private Transform diskRotation;
    [SerializeField] private Button btnFree;
    [SerializeField] private Button btnAds;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Image sliTimesSpin;
    [SerializeField] private Text txtAccumulate;
    [SerializeField] private Text txtRestTimeFree;
    //For replace
    //[SerializeField] private GameObject itmCharacter;
    [SerializeField] private Text txtSumItem;
    [SerializeField] private Text txtTurnAds;
    [SerializeField] private Text txtTurnFree;
    private Coroutine corColDownFreeSpin;
    private Coroutine corColDownAds;
    private ItemSpin[] itemSpins;
    private const float SUM_ROUND = 5f;
    private const float TIME_ROTATION = 5f;
    public static GameObject instance;
    [SerializeField] LuckySpinDataBase luckySpinData;
    ItemSpin itemCurrent;
    [SerializeField] GameObject hand;
    //[Header("Hack Spin")]
    //public Button btnResetLastTimeSpin;

    protected override void OnAwake()
    {
        base.OnAwake();

        itemSpins = GetComponentsInChildren<ItemSpin>();
        btnFree.onClick.AddListener(RotaFree);
        btnAds.onClick.AddListener(RoteAds);
        UpdateUI();
        //btnResetLastTimeSpin.onClick.AddListener(HackSpin);
    }



    public static LuckySpin Setup()
    {
        if (instance == null)
        {
            // Create popup and attach it to UI
            instance = Instantiate(Resources.Load(PathPrefabs.LUCKY_SPIN) as GameObject);
        }

        instance.SetActive(true);
        Debug.Log("show");
        return instance.GetComponent<LuckySpin>();
    }

    protected override void DoAppear()
    {
        UpdateUI();
        ShowLighting(false);
        PlayerPrefs.SetInt("is_tuted_wheel", 1);
        //GameController.Instance.admobAds.ShowBanner();
    }
    bool isLockEscape;
    public void MakeRotation()
    {
        isLockEscape = true;
        closeBtn.interactable = false;
        var radRw = luckySpinData.PickRandomReward();
        //Debug.LogError("rad " + radRw.item + "|" + radRw.amount);
        int idForward = -1;
        ItemSpin itemSpin = null;
        btnAds.interactable = false;
        //Search item on spin that math with current reward was picked above
        if (itemSpins.Length > 0)
        {
            for (int i = 0; i < itemSpins.Length; i++)
            {
                if (itemSpins[i].ItemName == radRw.item && itemSpins[i].amount == radRw.amount)
                {
                    idForward = itemSpins[i].Id;
                    //Debug.LogError("thang id = -1: " + idForward);
                    //Debug.LogError("itemSpin: " + itemSpins[i].Id);
                    itemSpin = itemSpins[i];
                    itemCurrent = itemSpin;
                }
            }
        }

        //Add reward

        if (idForward >= 0)
        {
            float curAngle = diskRotation.localEulerAngles.z;
            //Clockwise
            float angleTarget = idForward * 45 - SUM_ROUND * 360f - curAngle;
            //Anticlockwise
            //float angleTargetidForward * 45 + SUM_ROUND * 360f - curAngle;

            diskRotation.DOLocalRotate(new Vector3(0, 0, angleTarget), TIME_ROTATION, RotateMode.LocalAxisAdd).SetEase(Ease.InOutQuart).OnComplete(() =>
            {

                isLockEscape = false;
                closeBtn.interactable = true;
                ShowLighting(true, itemSpin.transform.GetChild(1));
                StartCoroutine(Helper.StartAction(() =>
                {
                    btnAds.gameObject.SetActive(true);
                    btnAds.interactable = true;
                    AddReward(radRw); ShowLighting(false, itemSpin.transform.GetChild(1));
                }, 1f));
                btnFree.gameObject.SetActive(false);
            });
        }
    }
    //Check a reward and do something with it
    protected void AddReward(RewardDatabase.Reward reward)
    {
        RewardDatabase.Claim(reward);
        ClaimFinish();
    }

    protected void ClaimFinish()
    {
        //Check wheather has chest can be opened or not show buttons for new turn spin
        //Update coin and gem at homescene
        //Update ui
        UpdateUI();
    }

    private void ShowLighting(bool isShow, Transform trf = null)
    {
        if (isShow) trf.gameObject.SetActive(true);
        else
        {
            if (trf != null)
                trf.gameObject.SetActive(false);
            else
            {
                if (itemCurrent != null)
                    itemCurrent.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    protected void UpdateUI()
    {
        btnAds.transform.DOKill();
        btnAds.transform.localScale = Vector3.one;
        btnAds.transform.DOScale(1.2f, 0.3f).SetLoops(-1, LoopType.Yoyo);

        closeBtn.interactable = true;

        //Check wheather show "btnFree" or not
        DateTime lastTimeFreeSpined = GameData.LuckySpinLastTimeFreeSpined;
        txtRestTimeFree.text = "";
        if (lastTimeFreeSpined.ToString() != "")
        {
            if (lastTimeFreeSpined < UnbiasedTime.Instance.Now())
            {
                btnFree.gameObject.SetActive(true);
                hand.SetActive(true);
                btnAds.gameObject.SetActive(false);
                txtRestTimeFree.gameObject.SetActive(false);
            }
            else
            {
                btnFree.gameObject.SetActive(false);
                btnAds.gameObject.SetActive(true);
                corColDownFreeSpin = StartCoroutine(CoolDownFreeRotation());
            }
        }
        else
        {
            btnFree.interactable = true;
            txtRestTimeFree.gameObject.SetActive(false);
        }
        //Check wheather "btnAds" or not
        //TODO
        //Debug.Log("A " + TimeManager.ParseTimeStartDay(DataManager.LastTimeSpinLuckyAds) + " B " + TimeManager.ParseTimeStartDay(UnbiasedTime.Instance.Now));
        //Debug.Log("timeRemainAds " + timeRemainAds);
        //For rest a turn
        //txtResetTimeAds.gameObject.SetActive(false);
        //btnAds.interactable = true;
    }
    //=========Rotation=========
    private void RotaFree()
    {

        hand.SetActive(false);
        MakeRotation();
        btnFree.interactable = false;
        GameData.LuckySpinLastTimeFreeSpined = UnbiasedTime.Instance.Now().AddDays(1);
        //UpdateUI();

        //MusicManager.Instance.PlaySfx(MusicManager.Instance.sfxClickButton);
    }

    private void RoteAds()
    {

        GameController.Instance.admobAds.ShowVideoReward(() =>
        {

            MakeRotation();

        }, ShowFail, () => { }, ActionWatchVideo.SpinADS, UseProfile.CurrentLevel.ToString());


        //MusicManager.Instance.PlaySfx(MusicManager.Instance.sfxClickButton);
    }
    void ShowFail()
    {
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
  (
     btnAds.transform.position,
      "No Video Ads",
      Color.red,
      isSpawnItemPlayer: true
  );
    }
    //=======Count Down========
    IEnumerator CoolDownFreeRotation()
    {
        //Check wheather show "btnFree" or not
        txtRestTimeFree.gameObject.SetActive(true);
        DateTime lastTimeFreeSpined = GameData.LuckySpinLastTimeFreeSpined;
        Debug.Log(lastTimeFreeSpined);
        if (lastTimeFreeSpined.ToString() != "")
        {
            while (UnbiasedTime.Instance.Now() < lastTimeFreeSpined)
            {
                yield return new WaitForSeconds(1f);

                txtRestTimeFree.text = TimeManager.ShowTime(TimeManager.CaculateTime(UnbiasedTime.Instance.Now(), lastTimeFreeSpined));
            }
        }

        txtRestTimeFree.gameObject.SetActive(false);
        btnFree.gameObject.SetActive(true);
        btnFree.interactable = true;
    }
}

