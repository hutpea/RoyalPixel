using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using EventDispatcher;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EventFarmDialogueController : MonoBehaviour
{
    private const float DO_TEXT_TIME = 1.25f;
    private const float DO_DIALOGUE_BOX_SCALE_TIME = 0.25f;
    public Transform girlTutorialTransform;
    public Image girlTutorialImage;
    public Text dialogueText;

    public int currentIndex;
    public List<DialogueItemData> currentDialogueDataList;

    Action<object> currentConditionAction;
    public Action onDialogueEnd;

    public bool isBusy;
    
    private RectTransform dialogueBoxRect;

    private void Awake()
    {
        dialogueBoxRect = dialogueText.transform.parent.GetComponent<RectTransform>();
        isBusy = false;
    }

    public void Show()
    {
        girlTutorialTransform.gameObject.SetActive(true);
    }

    public void Close()
    {
        girlTutorialTransform.gameObject.SetActive(false);
    }

    public void ToggleImage(bool isShow)
    {
        girlTutorialImage.enabled = isShow;
    }

    public void DODialogue()
    {
        Debug.Log("current dialogue index: " + currentIndex);
        if (currentIndex < currentDialogueDataList.Count)
        {
            currentDialogueDataList[currentIndex].onStartAction?.Invoke();
            currentConditionAction = (sender) => OnConditionAction();
            this.RemoveListener(EventID.EVENT_FARM_GIRL_BACKGROUND_CLICKED, currentConditionAction);
            this.RemoveListener(EventID.EVENT_FARM_PAN_DETECTION, currentConditionAction);
            this.RemoveListener(EventID.EVENT_FARM_ZOOM_DETECTION, currentConditionAction);
            this.RemoveListener(EventID.EVENT_FARM_LOCATE_BUTTON_CLICKED, currentConditionAction);
            this.RemoveListener(EventID.EVENT_FARM_GUIDE_MASK_BACKGROUND_CLICKED, currentConditionAction);
            this.RegisterListener(currentDialogueDataList[currentIndex].eventID, currentConditionAction);
            Debug.Log($"Event ID {currentDialogueDataList[currentIndex].eventID} of {currentIndex} is registered");
            
            dialogueBoxRect.DOScaleY(1f, DO_DIALOGUE_BOX_SCALE_TIME).OnComplete(delegate
            {
                DOVirtual.DelayedCall(0.2f, delegate
                {
                    DOTextRun(currentDialogueDataList[currentIndex].message);
                    Invoke("DelayCallSetBusyFalse", DO_TEXT_TIME);
                });
            });
        }
        else
        {
            onDialogueEnd?.Invoke();
            Debug.Log("There no more tutorials in this data now!");
            Close();
        }
    }

    public void JumpToNextDialogue()
    {
        Debug.Log("Check isBusy = " + isBusy);
        if (isBusy) return;
        isBusy = true;
        currentDialogueDataList[currentIndex].onEndAction?.Invoke();
        currentIndex++;
        dialogueText.text = "";
        dialogueBoxRect.DOScaleY(0f, DO_DIALOGUE_BOX_SCALE_TIME).OnComplete(delegate
        {
            ToggleImage(false);
            DODialogue();
            GameController.Instance.musicManager.PlayDialoguePopupSound();
        });
    }

    private void DelayCallSetBusyFalse()
    {
        isBusy = false;
    }

    public void DOTextRun(string msg)
    {
        dialogueText.text = "";
        GameController.Instance.musicManager.PlayTextTypingSound();
        dialogueText.DOText(msg, DO_TEXT_TIME, richTextEnabled: true);
    }

    public void SetupDialogue(List<DialogueItemData> dialogueDataList, Action onDialogueEnd)
    {
        currentDialogueDataList = dialogueDataList;
        this.onDialogueEnd = onDialogueEnd;
        currentIndex = 0;
    }

    public void OnConditionAction()
    {
        Debug.Log($"OnConditionAction{currentIndex} triggered!");
        JumpToNextDialogue();
    }

    public void OnMainTutorialBackgroundClicked()
    {
        Debug.Log("OnMainTutorialBackgroundClicked PostEvent");
        this.PostEvent(EventID.EVENT_FARM_GIRL_BACKGROUND_CLICKED);
    }
    
    public void OnGuideMaskBackgroundClicked()
    {
        Debug.Log("OnGuideMaskBackgroundClicked PostEvent");
        this.PostEvent(EventID.EVENT_FARM_GUIDE_MASK_BACKGROUND_CLICKED);
    }
    
    public void OnPan()
    {
        Debug.Log("OnPan PostEvent");
        this.PostEvent(EventID.EVENT_FARM_PAN_DETECTION);
    }
    
    public void OnZoom()
    {
        Debug.Log("OnZoom PostEvent");
        this.PostEvent(EventID.EVENT_FARM_ZOOM_DETECTION);
    }
    
    public void OnClickLocateButton()
    {
        Debug.Log("OnClickLocateButton PostEvent");
        this.PostEvent(EventID.EVENT_FARM_LOCATE_BUTTON_CLICKED);
    }
    

    public void RemoveAction()
    {
        currentDialogueDataList.Clear();
        currentIndex = 0;
        onDialogueEnd = null;
    }

    public void RemoveAllPopupListener()
    {
        Debug.Log("DialogueController RemoveAllPopupListener");
        this.RemoveListener(EventID.EVENT_FARM_GIRL_BACKGROUND_CLICKED, currentConditionAction);
        this.RemoveListener(EventID.EVENT_FARM_PAN_DETECTION, currentConditionAction);
        this.RemoveListener(EventID.EVENT_FARM_ZOOM_DETECTION, currentConditionAction);
        this.RemoveListener(EventID.EVENT_FARM_LOCATE_BUTTON_CLICKED, currentConditionAction);
        this.RemoveListener(EventID.EVENT_FARM_GUIDE_MASK_BACKGROUND_CLICKED, currentConditionAction);
    }

    public void NullMethod()
    {
        
    }
}

[Serializable]
public class DialogueItemData
{
    public string message;
    public EventID eventID;
    public Action onStartAction;
    public Action onEndAction;
}