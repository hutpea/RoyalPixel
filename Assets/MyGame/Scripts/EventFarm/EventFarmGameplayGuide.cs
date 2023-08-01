using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventFarmGameplayGuide : MonoBehaviour
{
    public EventFarmGameplayDialogueController dialogueController;
    public Guide guide;
    public Image fakeImageFrame;
    public void TryShowTutorial()
    {
        if (!GameData.EventFarmTutorial_1_Done)
        {
            GamePlayControl.Instance.isTimeCounterFrozen = true;
            dialogueController.Show();
            dialogueController.RemoveAction();

            List<DialogueItemData> dialogueDataList = new List<DialogueItemData>();
            //DIALOGUE 1
            DialogueItemData dialogueData_welcome = new DialogueItemData();
            dialogueData_welcome.message =
                "You will draw to complete this picture, but there is time limit!";
            dialogueData_welcome.eventID = EventID.EVENT_FARM_GAMEPLAY_GUIDE_MASK_BACKGROUND_CLICKED;
            dialogueData_welcome.onStartAction = delegate
            {
                dialogueController.ToggleImage(false);
                guide.ToggleGuideMask(true);
                Image frame = fakeImageFrame;
                guide.Target = frame.GetComponent<RectTransform>();
                guide.SetGuideMask(frame, delegate
                {
                    dialogueController.OnGameplayGuideMaskBackgroundClicked();
                }, isShowHand: false);
            };
            dialogueData_welcome.onEndAction = delegate { };
            //DIALOGUE 2
            DialogueItemData dialogueData_timerFocus = new DialogueItemData();
            dialogueData_timerFocus.message =
                "Because we don't have much time, this is time to play, if the time runs out you will have to play again! Good luck!";
            dialogueData_timerFocus.eventID = EventID.EVENT_FARM_GAMEPLAY_GUIDE_MASK_BACKGROUND_CLICKED;
            dialogueData_timerFocus.onStartAction = delegate
            {
                Debug.Log("Dialogue 2 onStartAction a");
                Image frame = GamePlayControl.Instance.eventTimerGameObject.transform.GetChild(0).GetComponent<Image>();
                Debug.Log("Dialogue 2 onStartAction b");
                guide.Target = frame.GetComponent<RectTransform>();
                Debug.Log("Dialogue 2 onStartAction c");
                guide.SetGuideMask(frame, delegate
                {
                    dialogueController.OnGameplayGuideMaskBackgroundClicked();
                }, isShowHand: true, isMaskShapeSquare: false);
                Debug.Log("Dialogue 2 onStartAction d");
            };
            dialogueData_timerFocus.onEndAction = delegate
            {
                GamePlayControl.Instance.isTimeCounterFrozen = false;
            };
            //
            dialogueDataList.Add(dialogueData_welcome);
            dialogueDataList.Add(dialogueData_timerFocus);

            dialogueController.SetupDialogue(dialogueDataList, delegate
            {
                Debug.Log("END GAMEPLAY TUTORIAL");
                dialogueController.Close();
                dialogueController.girlTutorialImage.gameObject.SetActive(false);
                guide.guideMask.gameObject.SetActive(false);
                guide.hand.gameObject.SetActive(false);
                GameData.EventFarmTutorial_1_Done = true;
                GamePlayControl.Instance.isTimeCounterFrozen = false;
            });

            dialogueController.DODialogue();
        }
        else
        {
            Debug.Log("Tutorial 1 done, no need to show tutorial again!");
            dialogueController.RemoveAction();
            dialogueController.RemoveAllPopupListener();
            dialogueController.girlTutorialImage.gameObject.SetActive(false);
            guide.guideMask.gameObject.SetActive(false);
            guide.hand.gameObject.SetActive(false);
        }
    }
}
