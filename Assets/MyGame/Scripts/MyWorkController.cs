using BizzyBeeGames.ColorByNumbers;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyWorkController : MonoBehaviour
{
    [SerializeField] Text txtInprogress;
    [SerializeField] Text txtCompleted;
    [SerializeField] Transform contentInProgress;
    [SerializeField] Transform contentFinished;
    [SerializeField] GameObject inProgress, inFinished;
    bool isLoad;
    [SerializeField] Image choice;
    [SerializeField] ElementPic elementPic;
    [SerializeField] AreasElement areasElement;
    string progress;
    [SerializeField] GameObject myProfile;
    private void Awake()
    {
        SelectTab(false);
        progress = PlayerPrefs.GetString(StringConstants.KEY.SAVE_INPROGRESS);
        Debug.Log("progress" + progress);
        LoadPic(progress);
    }
    private void OnEnable()
    {
        LoadPicInprogress();
        CheckCompletePic();
        Invoke("UploadNumberPic", 0.1f);
    }
    void CheckCompletePic()
    {
        foreach (Transform item in contentInProgress)
        {
            if (item.gameObject.GetComponent<ElementPic>() != null)
            {
                ElementPic elementPic = item.gameObject.GetComponent<ElementPic>();
                if (elementPic.picture.Completed)
                {
                    var it = Instantiate(elementPic, contentFinished);
                    it.InitPic(elementPic.picture);
                    Destroy(item.gameObject);
                }
            }
            else
            {
                AreasElement areasElement = item.gameObject.GetComponent<AreasElement>();
                if (GameData.GetUnlockArea(areasElement.ID))
                {
                    var it = Instantiate(areasElement, contentFinished);
                    it.InitAreas(areasElement.categoryInfo, areasElement.IdCateArea);
                    Destroy(item.gameObject);
                }
            }
        }
    }
    void LoadPicInprogress()
    {
        string progressNew = PlayerPrefs.GetString(StringConstants.KEY.SAVE_INPROGRESS);
        if (!string.IsNullOrEmpty(progressNew) && progressNew != progress)
        {
            string a = "";
            if (string.IsNullOrEmpty(progress))
            {
                a = progressNew.Substring(0, (progressNew.Length));
            }
            else
                a = progressNew.Substring(progress.Length, (progressNew.Length - progress.Length));
            LoadPic(a);
            progress = progressNew;
        }
    }
    void LoadPic(string progress)
    {
        if (!string.IsNullOrEmpty(progress))
        {
            string[] ids = progress.Split(',');
            for (int i = 0; i < ids.Length - 1; i++)
            {
                if (!ids[i].Contains("-"))
                {
                    if (ids[i] != "")
                    {


                        //Debug.Log(" sss " + ids[i]);
                        CategoryInfo pic = GameData.GetCateById(ids[i]);
                        if (pic != null)
                        {
                            if (GameData.GetUnlockArea(int.Parse(ids[i])))
                            {
                                var it = Instantiate(areasElement, contentFinished);
                                it.InitAreas(pic, pic.IdCateArea);
                            }
                            else
                            {
                                var it = Instantiate(areasElement, contentInProgress);
                                it.InitAreas(pic, pic.IdCateArea);
                            }
                        }

                    }
                }
                else
                {
                    if (ids[i] != "")
                    {
                        if (ids[i].Contains(CategoryConst.DAILY.ToString()))
                        {
                        }
                        else
                        {
                            PictureInformation picture = GameData.GetPictureInforById(ids[i]);
                            if (picture != null)
                            {
                                if (picture.Completed)
                                {
                                    var it = Instantiate(elementPic, contentFinished);
                                    it.InitPic(picture);
                                }
                                else
                                {
                                    var it = Instantiate(elementPic, contentInProgress);
                                    it.InitPic(picture);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    void UploadNumberPic()
    {
        int total = contentInProgress.childCount;
        if (total != 0)
            txtInprogress.text = "In Progress (" + total + ")";
        if (total == 1)
        {
            contentInProgress.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.UpperLeft;
        }
        else
            contentInProgress.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.UpperCenter;

        int total1 = contentFinished.childCount;
        if (total1 != 0)
            txtCompleted.text = "Finished (" + total1 + ")";
        if (total1 == 1)
        {
            contentFinished.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.UpperLeft;
        }
        else
            contentFinished.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.UpperCenter;
    }
    public void SelectTab(bool finish)
    {
        if (!finish)
        {
            txtInprogress.color = choice.color;
            txtCompleted.color = new Color32(159, 159, 159, 255);
            choice.transform.position = new Vector2(txtInprogress.transform.position.x, choice.transform.position.y);
            inFinished.SetActive(false);
            inProgress.SetActive(true);
        }
        else
        {
            txtInprogress.color = new Color32(159, 159, 159, 255);
            txtCompleted.color = choice.color;
            choice.transform.position = new Vector2(txtCompleted.transform.position.x, choice.transform.position.y);
            inFinished.SetActive(true);
            inProgress.SetActive(false);
        }
    }
    Vector2 start;
    public void ShowProfile()
    {
        GameController.Instance.admobAds.ShowInterstitial(false, "show_profile", () =>
        {
            GameController.Instance.musicManager.PlayBtnClick();
            myProfile.transform.DOKill();
            start = new Vector2(transform.localPosition.x + HomeController.instance.gameObject.GetComponent<RectTransform>().rect.width, transform.localPosition.y); ;
            myProfile.transform.localPosition = start;
            myProfile.gameObject.SetActive(true);
            myProfile.transform.DOLocalMoveX(transform.localPosition.x, 0.5f);
        });
    }
    public void CloseProfile()
    {
        GameController.Instance.musicManager.PlayBtnClick();
        myProfile.transform.DOKill();
        float distance = start.x - myProfile.transform.localPosition.x;
        myProfile.transform.DOLocalMoveX(start.x + 40, distance / 1200).OnComplete(() => myProfile.SetActive(false));
    }


}
