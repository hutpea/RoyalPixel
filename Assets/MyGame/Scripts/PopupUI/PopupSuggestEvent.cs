using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupSuggestEvent : BaseBox
{
    private static PopupSuggestEvent instance;
    [SerializeField] string strSuggest;
    [SerializeField] string strCondition;
    [SerializeField] Text txtContent;
    [SerializeField] Text txtBtn;
    [SerializeField] GameObject lockObj;
    public static PopupSuggestEvent Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupSuggestEvent>(PathPrefabs.POPUP_SUGGEST_EVENT));
        }
        if (GameData.CountPicComplete >= 5)
        {
            instance.InitPopupSuggest();
        }
        else
            instance.InitPopupCondition();
        return instance;
    }
    void InitPopupCondition()
    {
        txtContent.text = strCondition;
        lockObj.SetActive(true);
        txtBtn.text = "OK";
    }
    void InitPopupSuggest()
    {
        txtContent.text = strSuggest;
        txtBtn.text = "GO";
        lockObj.SetActive(false);
    }
    public void ClickButton()
    {
        if (GameData.CountPicComplete < 5)
        {
            Close();
        }
        else
        {
            Close();
            PopupEventJigsaw.Setup().Show();
        }
    }
}
