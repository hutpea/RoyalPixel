using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupGetMoreBomb : BaseBox
{
    private static PopupGetMoreBomb instance;
    public int bombs = 2;
    [SerializeField] Transform btnClaim;
    [SerializeField] GameObject btnGoVip;
    public static PopupGetMoreBomb Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupGetMoreBomb>(PathPrefabs.POPUP_GET_MORE_BOMB));
        }
        instance.InitPopup();
        return instance;
    }
    void InitPopup()
    {
        if (GameData.BuyPackIAP(PackIAP.SUBSCRIPTION))
            btnGoVip.SetActive(false);
        else
            btnGoVip.SetActive(true);
    }
    public void GetMore()
    {
        //#if UNITY_EDITOR
        //        Success();
        //#endif
        GameController.Instance.admobAds.ShowVideoReward(Success, ShowFail, null, ActionWatchVideo.GetMoreBomb, GameData.picChoice.Id);
    }
    void Success()
    {
        Close();
        GameData.ItemBomb += bombs;
        GameController.Instance.moneyEffectController.SpawnEffect_FlyUp(transform.position, Item.Bomb, bombs.ToString(), Color.yellow);
        UIGameController.instance.SetTotalBomb();
    }
    void ShowFail()
    {
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
    (
       btnClaim.transform.position,
        "Video is not available!",
        Color.red,
        isSpawnItemPlayer: true
    );
    }
    public void ShowVip()
    {
        PopupVip.Setup().Show();
    }
}
