using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupNotiEvent : BaseBox
{
    private static PopupNotiEvent instance;
    [SerializeField] Text txtTimeLeft;
    public static PopupNotiEvent Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupNotiEvent>(PathPrefabs.POPUP_NOTI_EVENT));
        }
        instance.InitPopup();
        return instance;
    }
    void InitPopup()
    {
        string strDefault = "08-01-2023";
        DateTime time = DateTime.Parse(RemoteConfigController.GetStringConfig(FirebaseConfig.TIME_EVENT, strDefault));
        TimeSpan timeSpan = (time - UnbiasedTime.Instance.Now());
        int day = (int)timeSpan.TotalDays;
        Debug.Log("time " + time);
        if (day > 1)
            txtTimeLeft.text = "Time left: " + ((int)timeSpan.TotalDays).ToString() + "days";
        if (day <= 1)
            txtTimeLeft.text = "Time left: " + "1" + "days";
    }
    public void Play()
    {
        HomeController.instance.ShowPopupNoel();
        Close();
    }
}
