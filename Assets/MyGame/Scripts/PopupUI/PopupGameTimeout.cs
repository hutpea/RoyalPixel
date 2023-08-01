using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupGameTimeout : BaseBox
{
    private static PopupGameTimeout instance;

    public Image fillImg;
    public Text fillAmountTxt;
    public Button btnYes;
    
    public static PopupGameTimeout Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupGameTimeout>(PathPrefabs.POPUP_GAMETIMEOUT));
        }
        instance.Init();
        return instance;
    }

    public void Init()
    {
        Debug.Log(GameData.FarmTreasureFillAmount);
        fillImg.fillAmount = GameData.gameplayProgress;
        fillAmountTxt.text = ((float)GameData.gameplayProgress * 100f).ToString("F0") + "%";
    }
    
    public void OnClickYes()
    {
        GameController.Instance.admobAds.ShowVideoReward(Success, ShowFail, null, ActionWatchVideo.None, GameData.picChoice.Id);
    }

    public void OnClickNo()
    {
        //Debug.Log("GameData isFarm = " + GameData.isSelectInEventFarm);
        UIGameController.instance.BackHome();
        Close();
    }
    
    public void Success()
    {
        GamePlayControl.Instance.ResetEventTimer();
        Close();
    }

    public void ShowFail()
    {
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
        (
            btnYes.transform.position,
            "Video is not available!",
            Color.red,
            isSpawnItemPlayer: true
        );
    }
}