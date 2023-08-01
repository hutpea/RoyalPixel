using EventDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingController : MonoBehaviour
{
    [SerializeField] GameObject onMusic, offMusic;
    [SerializeField] GameObject onSound, offSound;
    [SerializeField] GameObject onVibrate, offVibrate;
    [SerializeField] GameObject onNoAds, offNoads;
    private System.Action<object> actionOnNoAds;
    private void OnEnable()
    {
    }
    private void Start()
    {
        onSound.SetActive(GameController.Instance.useProfile.OnSound);
        offSound.SetActive(!GameController.Instance.useProfile.OnSound);
        onMusic.SetActive(GameController.Instance.useProfile.OnMusic);
        offMusic.SetActive(!GameController.Instance.useProfile.OnMusic);
        onVibrate.SetActive(GameController.Instance.useProfile.OnVibration);
        offVibrate.SetActive(!GameController.Instance.useProfile.OnVibration);
        onNoAds.SetActive(UseProfile.IsRemoveAds || UseProfile.IsVip);
        offNoads.SetActive(!UseProfile.IsRemoveAds && !UseProfile.IsVip);
        actionOnNoAds = (sender) => OnNoAds();
        this.RegisterListener(EventID.BUY_VIP_SUB, actionOnNoAds);
    }
    public void SetupSound(bool onSound)
    {
        GameController.Instance.useProfile.OnSound = onSound;
        this.onSound.SetActive(onSound);
        offSound.SetActive(!onSound);
    }
    public void SetupMusic(bool onMusic)
    {
        GameController.Instance.useProfile.OnMusic = onMusic;
        this.onMusic.SetActive(onMusic);
        offMusic.SetActive(!onMusic);
    }
    public void SetupVibrate(bool vibrate)
    {
        GameController.Instance.useProfile.OnVibration = vibrate;
        this.onVibrate.SetActive(vibrate);
        offVibrate.SetActive(!vibrate);
    }
    public void LinkPrivacy()
    {
        Application.OpenURL("https://sites.google.com/view/global-play-policy/");
    }
    public void LinkTerm()
    {
        Application.OpenURL("https://sites.google.com/view/global-play-terms-of-use");
    }
    public void Rate()
    {
        DialogueRate.Setup().Show();
    }

    public void ShowLicense()
    {
        LicenseBox.Setup().Show();
    }
    public void ShowPopupVip()
    {
        PopupVip.Setup().Show();
    }
    public void OnNoAds()
    {
        UseProfile.IsRemoveAds = true;
        onNoAds.SetActive(UseProfile.IsRemoveAds);
        offNoads.SetActive(!UseProfile.IsRemoveAds);
    }
    public void Click()
    {
        AnalyticsController.LogClickVIP("noads_setting");
    }
}
