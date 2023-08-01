using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventFarmTutorialController : MonoBehaviour
{
    public EventFarmDialogueController dialogueController;
    public Guide guide;

    public void TryShowTutorial()
    {
        if (!GameData.EventFarmTutorial_1_Done)
        {
            Debug.Log(PopupEventFarmBuilding.instance);
            Debug.Log(PopupEventFarmBuilding.instance.preventClickLayer);
            PopupEventFarmBuilding.instance.preventClickLayer.SetActive(false);
            dialogueController.Show();
            dialogueController.RemoveAction();
            
            List<DialogueItemData> dialogueDataList = new List<DialogueItemData>();
            //DIALOGUE 1
            DialogueItemData dialogueData_welcome = new DialogueItemData();
            dialogueData_welcome.message =
                "Welcome to the new land . I'm <color=red>Yuna</color>. I will help you to build your own land.";
            dialogueData_welcome.eventID = EventID.EVENT_FARM_GIRL_BACKGROUND_CLICKED;
            dialogueData_welcome.onStartAction = delegate
            {
                dialogueController.ToggleImage(true); 
                PopupEventFarmBuilding.instance.zoomButton.gameObject.SetActive(false);
            };
            dialogueData_welcome.onEndAction = delegate { };
            //DIALOGUE 2
            DialogueItemData dialogueData_locateBtn = new DialogueItemData();
            dialogueData_locateBtn.message =
                "You can come back to current item by click this button!";
            dialogueData_locateBtn.eventID = EventID.EVENT_FARM_LOCATE_BUTTON_CLICKED;
            dialogueData_locateBtn.onStartAction = delegate
            {
                dialogueController.ToggleImage(false);
                PopupEventFarmBuilding.instance.DOScroll(PopupEventFarmBuilding.instance.farItemTutorialRect,
                    duration: 0.1f, isShowHand: false);
                guide.ToggleGuideMask(true);

                Button btn = PopupEventFarmBuilding.instance.locateButton.GetComponent<Button>();
                guide.Target = btn.GetComponent<RectTransform>();
                guide.Canvas = PopupEventFarmBuilding.instance.eventCanvas;
                guide.SetGuideMask(btn, delegate
                {
                    PopupEventFarmBuilding.instance.DOScroll(
                        PopupEventFarmBuilding.instance.currentFarmItem.GetComponent<RectTransform>(),
                        duration: 0.5f, isShowHand: false, isSimulateClick: true);
                }, isShowHand: true);
            };
            dialogueData_locateBtn.onEndAction = delegate { };
            //DIALOGUE 3
            DialogueItemData dialogueData_progressReward = new DialogueItemData();
            dialogueData_progressReward.message =
                "This is reward gift, you must complete some items to get it, try your best because the reward is very interesting!";
            dialogueData_progressReward.eventID = EventID.EVENT_FARM_GUIDE_MASK_BACKGROUND_CLICKED;
            dialogueData_progressReward.onStartAction = delegate
            {
                Debug.Log("Start dialogue 3");
                dialogueController.ToggleImage(false);
                guide.ToggleGuideMask(true);
                Image frame = PopupEventFarmBuilding.instance.progressFrameFake;
                guide.Target = frame.GetComponent<RectTransform>();
                guide.Canvas = PopupEventFarmBuilding.instance.eventCanvas;
                guide.SetGuideMask(frame, delegate { dialogueController.OnGuideMaskBackgroundClicked(); },
                    isShowHand: true);
            };
            dialogueData_progressReward.onEndAction = delegate { guide.ToggleGuideMask(false); };
            //DIALOGUE 4
            DialogueItemData dialogueData_letsbuild = new DialogueItemData();
            dialogueData_letsbuild.message =
                "Now! Let's building!";
            dialogueData_letsbuild.eventID = EventID.EVENT_FARM_GIRL_BACKGROUND_CLICKED;
            dialogueData_letsbuild.onStartAction = delegate
            {
                dialogueController.ToggleImage(true);
                guide.hand.gameObject.SetActive(false);
            };
            dialogueData_letsbuild.onEndAction = delegate { };
            //DIALOGUE 5
            DialogueItemData dialogueData_tapElementPicToStart = new DialogueItemData();
            dialogueData_tapElementPicToStart.message =
                "Tap here to build your first item!";
            dialogueData_tapElementPicToStart.eventID = EventID.EVENT_FARM_GUIDE_MASK_BACKGROUND_CLICKED;
            dialogueData_tapElementPicToStart.onStartAction = delegate
            {
                dialogueController.ToggleImage(false);
                guide.ToggleGuideMask(true);

                PopupEventFarmBuilding.instance.DOScroll(
                    PopupEventFarmBuilding.instance.currentFarmItem.GetComponent<RectTransform>(),
                    duration: 0.4f, isShowHand: false, onEndAction: delegate
                    {
                        /*PopupEventFarmBuilding.instance.ZoomInAtItem(PopupEventFarmBuilding.instance.currentFarmItem);
                        PopupEventFarmBuilding.instance.isZoomIn = true;*/
                    });
                
                Button btn = PopupEventFarmBuilding.instance.currentFarmItem.elementPic.GetComponent<Button>();
                guide.Target = btn.GetComponent<RectTransform>();
                guide.Canvas = PopupEventFarmBuilding.instance.eventCanvas;
                
                /*PopupEventFarmBuilding.instance.ZoomInAtItem(PopupEventFarmBuilding.instance.currentFarmItem, zoomEndAction:
                    delegate
                    {
                        guide.guideButton.onClick.AddListener(delegate
                        {
                            Debug.Log("Play Mask btn clicked!");
                            dialogueController.RemoveAllPopupListener();
                            PopupEventFarmBuilding.instance.currentFarmItem.PlayButtonClicked();
                        });
                        StartCoroutine(PopupEventFarmBuilding.instance.DelayAction(delegate
                        {
                            guide.SetGuideMask(btn, delegate
                            {
                                
                            }, isShowHand: true, scaleMultiplier: 1.85f);
                        }, 0.2f));
                    });
                PopupEventFarmBuilding.instance.isZoomIn = true;*/
                
                guide.SetGuideMask(btn,
                    delegate
                    {
                        dialogueController.RemoveAllPopupListener();
                        PopupEventFarmBuilding.instance.currentFarmItem.PlayButtonClicked();
                    }, isShowHand: true);
            };
            dialogueData_tapElementPicToStart.onEndAction = delegate { };
            dialogueDataList.Add(dialogueData_welcome);
            dialogueDataList.Add(dialogueData_locateBtn);
            dialogueDataList.Add(dialogueData_progressReward);
            dialogueDataList.Add(dialogueData_letsbuild);
            dialogueDataList.Add(dialogueData_tapElementPicToStart);
            /*dialogueList.Add("- Firstly, let's to see your land with me ,so beautiful right ?");
            dialogueList.Add("- You can comeback by this way, tap the locate to come back game screen .");*/

            dialogueController.SetupDialogue(dialogueDataList, delegate
            {
                dialogueController.RemoveAllPopupListener();
                Debug.Log("END TUTORIAL 1");
                enabled = false;
            });

            dialogueController.DODialogue();
        }
        else
        {
            if (!GameData.EventFarmTutorial_2_Done && GameData.isBackFromEventFarmGameplay)
            {
                PictureInformation pictureInformation_2 =
                    GameController.Instance.dataEventFarm.CategoryInfos["FARM_ZONE_1"].PictureInfos[1];
                if (!pictureInformation_2.Completed)
                {
                    PopupEventFarmBuilding.instance.preventClickLayer.SetActive(false);
                    dialogueController.Show();
                    dialogueController.RemoveAction();

                    List<DialogueItemData> dialogueDataList = new List<DialogueItemData>();
                    //DIALOGUE 1
                    DialogueItemData dialogueData_goodjob = new DialogueItemData();
                    dialogueData_goodjob.message =
                        "Good job ! Now you can see this PLANT is completed!";
                    dialogueData_goodjob.eventID = EventID.EVENT_FARM_GUIDE_MASK_BACKGROUND_CLICKED;
                    dialogueData_goodjob.onStartAction = delegate
                    {
                        dialogueController.ToggleImage(false);
                        guide.ToggleGuideMask(false);
                        Image frame = PopupEventFarmBuilding.instance.zone1FarmItems[0].itemRectList[0]
                            .GetComponent<Image>();
                        frame.gameObject.SetActive(true);
                        guide.Target = frame.GetComponent<RectTransform>();
                        guide.Canvas = PopupEventFarmBuilding.instance.eventCanvas;
                        Debug.Log("Set guideMask bongLua");
                        StartCoroutine(guide.DelaySetGuideMaskImage(0.65f, frame,
                            delegate { dialogueController.OnGuideMaskBackgroundClicked(); },
                            isShowHand: true));
                        /*guide.SetGuideMask(frame, delegate { dialogueController.OnGuideMaskBackgroundClicked(); },
                            isShowHand: true);*/
                    };
                    dialogueData_goodjob.onEndAction = delegate { guide.ToggleGuideMask(false); };
                    //DIALOGUE 2
                    DialogueItemData dialogueData_goodbye = new DialogueItemData();
                    dialogueData_goodbye.message =
                        "That's all you need, I'm very happy to help you. I have to go now, see you again!";
                    dialogueData_goodbye.eventID = EventID.EVENT_FARM_GIRL_BACKGROUND_CLICKED;
                    dialogueData_goodbye.onStartAction = delegate
                    {
                        dialogueController.ToggleImage(true); 
                        PopupEventFarmBuilding.instance.scrollRect.horizontal = false;
                        PopupEventFarmBuilding.instance.zoomButton.gameObject.SetActive(true);
                    };
                    dialogueData_goodbye.onEndAction = delegate { };
                    dialogueDataList.Add(dialogueData_goodjob);
                    dialogueDataList.Add(dialogueData_goodbye);
                    dialogueController.SetupDialogue(dialogueDataList, delegate
                    {
                        PopupEventFarmBuilding.instance.scrollRect.horizontal = false;
                        GameData.EventFarmTutorial_2_Done = true;
                        dialogueController.RemoveAction();
                        dialogueController.RemoveAllPopupListener();
                        dialogueController.girlTutorialImage.gameObject.SetActive(false);
                        guide.guideMask.gameObject.SetActive(false);
                        guide.hand.gameObject.SetActive(false);
                        GameData.DisableHandInFarm = true;
                        PopupEventFarmBuilding.instance.GoToCurrentFarmItem();
                        StartCoroutine(PopupEventFarmBuilding.instance.Delay_EnableScroll());
                        Debug.Log("END TUTORIAL 2");
                    });

                    dialogueController.DODialogue();
                }
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
}