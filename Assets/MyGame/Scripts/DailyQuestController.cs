using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyQuestController : MonoBehaviour
{
    public QuestDaily questDaily;
    public static DailyQuestController instance;
    [SerializeField] GameObject noti;
    [SerializeField] GameObject noti2;
    private void Start()
    {
        instance = this;
        System.TimeSpan timeSpanDaily = TimeManager.ParseTimeStartDay(UnbiasedTime.Instance.Now()) - TimeManager.ParseTimeStartDay(GameData.GetDateTimeDailyQuest());
        Debug.Log(timeSpanDaily.Days);
        if (timeSpanDaily.TotalDays >= 1)
        {
            ResetQuest();
            GameData.isShowHeadPhone = true;
            AchievementController.instance.UpdateProgressQuest(TypeQuest.DailyWorking);
        }
        if (GameData.NumberDailyQuest >= 3 || GameData.NumberDailyQuest == -1)
            GameData.NumberDailyQuest = 0;
        Debug.Log("quest" + GameData.NumberDailyQuest);
        if (GameData.NumberDailyQuest > 1)
        {
            GameData.NewUpdate = false;
        }
        CloseNoti();
    }
    public void ResetQuest()
    {
        Debug.Log("reset quest");
        foreach (var item in questDaily.dailyQuests)
        {
            foreach (var quest in item.quests)
            {
                quest.Progress = 0;
                quest.Claimed = false;
            }
        }
        GameData.NumberDailyQuest++;
        GameData.SetDateTimeDailyQuest(UnbiasedTime.Instance.Now());
        if (GameData.NumberDailyQuest >= 3 || GameData.NumberDailyQuest == -1)
            GameData.NumberDailyQuest = 0;
    }
    public void UpdateProgressQuest(TypeQuest typeQuest)
    {
        foreach (Quest item in questDaily.dailyQuests[GameData.NumberDailyQuest].quests)
        {
            if (item.type == typeQuest && item.Progress < item.total && !item.Claimed)
            {
                item.Progress++;
                if (item.Progress == item.total)
                {
                    HomeController.instance.notiDaily.SetActive(true);
                    ShowNotiBanner(item);
                    noti.SetActive(true);
                    noti2.SetActive(true);
                }
            }
        }
    }
    public void ShowNotiBanner(Quest quest)
    {
        NotiClaimQuestDaily.Setup(quest).Show();
    }
    public void CloseNoti()
    {
        noti.SetActive(false);
        noti2.SetActive(false);
        foreach (Quest item in questDaily.dailyQuests[GameData.NumberDailyQuest].quests)
        {
            if (item.Progress == item.total && !item.Claimed)
            {
                noti.SetActive(true);
                noti2.SetActive(true);
            }
        }
    }
}
