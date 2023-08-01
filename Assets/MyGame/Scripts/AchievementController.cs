using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementController : MonoBehaviour
{
    //[SerializeField] GameObject notiMyWork;
    [SerializeField] GameObject notiMyProfile;
    [SerializeField] GameObject notiMyProfile2;
    [SerializeField] GameObject notiMyProfile3;
    public QuestAchievement quests;
    public static AchievementController instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void Start()
    {
        CloseNoti();
    }

    public void UpdateProgressQuest(TypeQuest typeQuest)
    {
        foreach (Achivement item in quests.questAchievement)
        {
            if (item.type == typeQuest && item.Progress < item.total)
            {
                item.Progress++;
                if (item.Progress == item.total)
                {
                    ShowNoti();
                }
            }
        }
    }
    public void ShowNoti()
    {
        //notiMyWork.SetActive(true);
        notiMyProfile.SetActive(true);
        notiMyProfile2.SetActive(true);
        notiMyProfile3.SetActive(true);
        if (!TutorialHand.FinishTutAchiviement)
        {
            TutorialHand.instance.handAchiviement.SetActive(true);
            TutorialHand.FinishTutAchiviement = true;
        }
    }
    public void CloseNoti()
    {
        //notiMyWork.SetActive(false);
        notiMyProfile.SetActive(false);
        notiMyProfile2.SetActive(false);
        notiMyProfile3.SetActive(false);
        TutorialHand.instance.handAchiviement.SetActive(false);
        foreach (Achivement item in quests.questAchievement)
        {
            if (item.Progress == item.total)
            {
                ShowNoti();
                if (!TutorialHand.FinishTutAchiviement)
                {
                    TutorialHand.instance.handAchiviement.SetActive(true);
                    
                }
            }
        }
    }
}
