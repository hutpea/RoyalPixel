using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupHeadPhones : BaseBox
{
    private static PopupHeadPhones instance;
    public static PopupHeadPhones Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupHeadPhones>(PathPrefabs.POPUP_HEAD_PHONE));
        }
        return instance;
    }
    public override void Close()
    {
        base.Close();
    }
}
