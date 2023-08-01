using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopupRemoveLogo : BaseBox
{
    private static PopupRemoveLogo instance;
    [SerializeField] Transform btnAds;
    public UnityAction actionReward;
    public static PopupRemoveLogo Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupRemoveLogo>(PathPrefabs.POPUP_REMOVE_LOGO));
        }
        return instance;
    }
    public void ShowVip()
    {
        PopupVip.Setup().Show();
    }
    public void RewardRemoveLogo()
    {
        GameController.Instance.admobAds.ShowVideoReward(() =>
        {
            actionReward.Invoke();
            Close();
        },
          FailToLoad, null, ActionWatchVideo.RemoveLogo, "Popup_remove_logo");
    }
    public void FailToLoad()
    {
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
    (
       btnAds.transform.position,
        "Video is not available!",
        Color.red,
        isSpawnItemPlayer: true
    );
        // }, 0.5f));
    }
}
