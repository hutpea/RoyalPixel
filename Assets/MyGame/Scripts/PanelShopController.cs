using EventDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelShopController : BaseBox
{
    public static PanelShopController instance;
    public Canvas canvas;
    [SerializeField] Text txtGem;
    public GameObject lostGem;
    [SerializeField] Button btnBuyGemFree;
    [SerializeField] Button btnBuyGemIAP;
    List<GameObject> lostItems = new List<GameObject>();
    private System.Action<object> actionGem;

    public void ShowPigGem()
    {
        GameController.Instance.musicManager.PlayClickSound();
        PopupPigGem.Setup().Show();
    }
    public static PanelShopController Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PanelShopController>(PathPrefabs.PANEL_SHOP));
        }

        Debug.Log("Setup Shop");
        return instance;
    }
    private void Start()
    {
        actionGem = (sender) => InitShop();
        this.RegisterListener(EventID.CHANGE_VALUE_GEM, actionGem);
    }
    bool initBool;
    public override void Show()
    {
        base.Show();
        instance.InitShop();
        if (!initBool)
        {
            instance.InitPoolLoseGem();
            initBool = true;
        }
        instance.canvas.sortingLayerName = "Popup";
        if (instance.canvas.sortingOrder <= 25)
        {
            instance.canvas.sortingOrder = 60;
        }
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        foreach (GameObject a in lostItems)
            a.SetActive(false);
    }
    public void InitShop()
    {
        txtGem.text = GameData.Gem.ToString();
    }
    public void ClickBuyGem()
    {
#if UNITY_EDITOR
        BuySuccesGem(9, btnBuyGemFree.transform.position);
#else
        GameController.Instance.admobAds.ShowVideoReward(() => BuySuccesGem(9, btnBuyGemFree.transform.position), ShowFail, null, ActionWatchVideo.BuyGem, "");
#endif
    }
    public void BuySuccesGem(int value, Vector2 pos)
    {
        GameController.Instance.musicManager.PlaySoundGem();
        GameData.Gem += value;
        //GameController.Instance.musicManager.PlayClickSound()

        GameController.Instance.moneyEffectController.SpawnEffect_GoDestination(pos, Item.Gem, value >= 700 ? 20 : value / 2, () => { this.PostEvent(EventID.CHANGE_VALUE_GEM); }, txtGem.transform.position);
    }
    public void BuyItem()
    {
        BuySuccesGem(700, btnBuyGemIAP.transform.position);
    }
    void ShowFail()
    {
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
  (
     btnBuyGemFree.transform.position,
      "No Video Ads",
      Color.red,
      isSpawnItemPlayer: true
  );
    }
    void InitPoolLoseGem()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject a = Instantiate(lostGem, txtGem.transform.parent);
            a.SetActive(false);
            lostItems.Add(a);
        }
    }
    public GameObject GetPoolLoseItem()
    {
        foreach (GameObject obj in lostItems)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                StartCoroutine(Helper.StartAction(() => obj.SetActive(false), 1.5f));
                return obj;
            }
        }
        GameObject a = Instantiate(lostGem, txtGem.transform.parent);
        lostItems.Add(a);
        a.SetActive(true);
        StartCoroutine(Helper.StartAction(() => a.SetActive(false), 1.5f));
        return a;
    }
    public void ClickHackGem()
    {
        GameData.Gem += 100;
        this.PostEvent(EventID.CHANGE_VALUE_GEM);
    }
}
