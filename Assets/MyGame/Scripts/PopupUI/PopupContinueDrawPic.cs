using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupContinueDrawPic : BaseBox
{
    private static PopupContinueDrawPic instance;
    public UnityAction OnClickContinue;
    public UnityAction OnClickDelete;
    public Image imgPic;
    string Id;
    public static PopupContinueDrawPic Setup(string IDPic)
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupContinueDrawPic>(PathPrefabs.POPUP_CONTINUE_DRAW));
        }
        instance.Id = IDPic;
        return instance;
    }
    public void BtnContinue()
    {
        if (OnClickContinue != null)
            OnClickContinue.Invoke();
        Close();
    }
    public void DeletePic()
    {
        if (OnClickDelete != null)
            OnClickDelete.Invoke();
        string pathDraw = GameData.DRAW_PATH + "/" + Id + ".csv";
        string pathHistoryDraw = GameData.HISTORY_PATH + "/" + Id + ".bin";
        Debug.Log(pathDraw);
        if (File.Exists(pathDraw))
            File.Delete(pathDraw);
        if (File.Exists(pathHistoryDraw))
            File.Delete(pathHistoryDraw);
        Close();
    }
}
