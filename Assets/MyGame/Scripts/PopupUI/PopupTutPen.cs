using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupTutPen : BaseBox
{
    private static PopupTutPen instance;
    public static PopupTutPen Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupTutPen>(PathPrefabs.POPUP_TUT_PEN));
        }
        return instance;
    }
    public override void Close()
    {
        base.Close();
        if (GameData.isShowHeadPhone)
        {
            PopupHeadPhones.Setup().Show();
            GameData.isShowHeadPhone = false;
        }
    }
}
