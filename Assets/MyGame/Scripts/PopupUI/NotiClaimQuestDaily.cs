using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotiClaimQuestDaily : BaseBox
{
    private static NotiClaimQuestDaily instance;
    [SerializeField] Text txtQuest;
    Coroutine coroutine;
    [SerializeField] Animator anim;
    [SerializeField] GameObject hand;
    Quest quest;
    public static NotiClaimQuestDaily Setup(Quest quest)
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<NotiClaimQuestDaily>(PathPrefabs.NOTI_CLAIM_DAILY_QUEST));
        }
        instance.quest = quest;
        return instance;
    }
    public override void Show()
    {
        base.Show();
        instance.InitNoti(quest);
    }
    public void InitNoti(Quest quest)
    {
        anim.Rebind();
        if (coroutine != null)
            StopCoroutine(coroutine);
        txtQuest.text = quest.quest;
        if (!TutorialHand.FinishTutDailyQuest)
        {
            StartCoroutine(Helper.StartAction(() => hand.SetActive(true), 0.5f));
        }
        if (gameObject.activeInHierarchy)
        {
            coroutine = StartCoroutine(Helper.StartAction(() => Close(), 5.5f));
        }
    }
    public override void Close()
    {
        StopCoroutine(coroutine);
        base.Close();
    }
    public void ClickNoti()
    {
        Close();
        GameController.Instance.musicManager.PlayBtnClick();
        hand.SetActive(false);
        PopupDailyQuest.Setup().Show();
    }
}
