using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupEventNoel : MonoBehaviour
{
    private static PopupEventNoel instance;
    private void Start()
    {
        if (instance == null)
            instance = this;
    }
    public void OpenAlbum(int ads)
    {

    }
    public void CreateCard()
    {
        GameData.isCreateCard = true;
    }
}
