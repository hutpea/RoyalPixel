using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupDailyQuest : BaseBox
{
    private static PopupDailyQuest instance;
    public QuestItem questItem;

    [SerializeField] Transform content;
    public static PopupDailyQuest Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupDailyQuest>(PathPrefabs.POPUP_DAILY_QUEST));
        }
        instance.InitQuest();
        return instance;
    }
    void InitQuest()
    {
        int day = GameData.NumberDailyQuest;
        if (content.childCount != 0)
        {
            foreach (Transform item in content)
            {
                Destroy(item.gameObject);
            }
        }
        for (int i = 0; i < DailyQuestController.instance.questDaily.dailyQuests[day].quests.Count; i++)
        {
            QuestItem quest = Instantiate(questItem, content);
            quest.InitQuest(DailyQuestController.instance.questDaily.dailyQuests[day].quests[i], day + "-" + i);
        }
    }
    public override void Close()
    {
        base.Close();
        foreach (Quest item in DailyQuestController.instance.questDaily.dailyQuests[GameData.NumberDailyQuest].quests)
        {
            if (item.Progress == item.total && !item.Claimed)
            {
                HomeController.instance.notiDaily.SetActive(true);
            }
        }
    }
}


