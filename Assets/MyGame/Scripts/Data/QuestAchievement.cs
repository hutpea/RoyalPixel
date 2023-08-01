using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "QuestAchievement", menuName = "ScriptableObjects/QuestAchievement", order = 1)]
public class QuestAchievement : ScriptableObject
{
    public List<Achivement> questAchievement;
}
[System.Serializable]
public class Achivement
{
    public string quest;
    public TypeQuest type;
    public int total;
    public GiftQuest giftQuest;
    public Sprite spArmorial;
    public Sprite spBorderArmorial;
    public Sprite ribbon;
    public int Progress
    {
        get
        {
            return PlayerPrefs.GetInt("quest_achivement" + type.ToString(), 0);
        }
        set
        {
            PlayerPrefs.SetInt("quest_achivement" + type.ToString(), value);
        }
    }
    public int Claimed
    {
        get
        {
            return PlayerPrefs.GetInt("claim_achievement" + type.ToString(), 0);
        }
        set
        {
            PlayerPrefs.SetInt("claim_achievement" + type.ToString(), value);
        }
    }
}
