using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopupSuggestFarmEvent : BaseBox
{
    private static PopupSuggestFarmEvent instance;
    
    public static PopupSuggestFarmEvent Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupSuggestFarmEvent>(PathPrefabs.POPUP_SUGGEST_FARM_EVENT));
        }
        instance.InitPopupSuggest();
        return instance;
    }
    
    void InitPopupSuggest()
    {
        
    }
    
    public void ClickButton()
    {
        if (GameData.CountPicComplete < 3)
        {
            Close();
        }
        else
        {
            Close();
            PopupEventFarmBuilding.Setup().OnShow();
        }
    }
}