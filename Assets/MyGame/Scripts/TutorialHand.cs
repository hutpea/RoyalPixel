using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialHand : MonoBehaviour
{
    public GameObject handAchiviement;
    public GameObject handDaily;
    public GameObject handItemBomb;
    public GameObject handItemStar;
    public GameObject handItemPen;
    public GameObject handItemFind;
    public static TutorialHand instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public static bool FinishTutAchiviement
    {
        get
        {
            return PlayerPrefs.GetInt("finish_tut_achiviement") == 1;
        }
        set
        {
            PlayerPrefs.SetInt("finish_tut_achiviement", value ? 1 : 0);
        }
    }
    public static bool FinishTutDailyQuest
    {
        get
        {
            return PlayerPrefs.GetInt("finish_tut_daily_quest") == 1;
        }
        set
        {
            PlayerPrefs.SetInt("finish_tut_daily_quest", value ? 1 : 0);
        }
    }
    public static bool FinishTutItem
    {
        get
        {
            return PlayerPrefs.GetInt("finish_tut_item") == 1;
        }
        set
        {
            PlayerPrefs.SetInt("finish_tut_item", value ? 1 : 0);
        }
    }
    
}
