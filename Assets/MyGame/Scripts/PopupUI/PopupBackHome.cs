using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupBackHome : BaseBox
{
    private static PopupBackHome instance;
    [SerializeField] Text txtContent;
    public static PopupBackHome Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupBackHome>(PathPrefabs.POPUP_BACK_HOME));
        }
        instance.InitText();
        return instance;
    }
    public void OnClickYes()
    {
        //Back();
        Invoke("Back", 0.1f);
        GamePlayControl.Instance.numberColoring.history.RemoveHistory();

    }
    void Back()
    {
        UIGameController.instance.BackHome();
        Close();
    }

    void InitText()
    {
        txtContent.text = "You are doing very well, you have complete " + GamePlayControl.Instance.percent + "% of the picture";
    }
}
