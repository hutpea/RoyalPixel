using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DataDailyQuest", menuName = "ScriptableObjects/QuestDaily", order = 1)]
public class QuestDaily : ScriptableObject
{
    public List<DailyQuest> dailyQuests;
}
[System.Serializable]
public class DailyQuest
{
    public List<Quest> quests;
}
[System.Serializable]
public class Quest
{
    public string quest;
    public TypeQuest type;
    public int total;
    public GiftQuest giftQuest;
    public int Progress
    {
        get
        {
            return PlayerPrefs.GetInt("quest_" + type.ToString(), 0);
        }
        set
        {
            PlayerPrefs.SetInt("quest_" + type.ToString(), value);
        }
    }
    public bool Claimed
    {
        get
        {
            return PlayerPrefs.GetInt("claim_" + type.ToString()) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("claim_" + type.ToString(), value ? 1 : 0);
        }
    }
}
[System.Serializable]
public class GiftQuest
{
    public Item item;
    public int numberItem;
    public Sprite sprite;
}
