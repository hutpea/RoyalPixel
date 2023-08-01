using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class PopupEventJigsaw : BaseBox
{
    private static PopupEventJigsaw instance;
    [SerializeField] DataPictureJigsaw dataPictureJigsaw;
    [SerializeField] PictureEventJigsaw pictureEvent;
    [SerializeField] Transform content;
    [SerializeField] HorizontalScrollSnap horizontalScrollSnap;
    List<GameObject> selectNote = new List<GameObject>();
    [SerializeField] GameObject preStep;
    [SerializeField] Transform contentStep;
    [SerializeField] GameObject panelTut;
    public static PopupEventJigsaw Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupEventJigsaw>(PathPrefabs.POPUP_EVENT_JIGSAW));
        }
        instance.InitPic();
        return instance;
    }
    private void Start()
    {
        if (content.childCount != 0)
        {
            StartCoroutine(Helper.StartAction(() => InitState(), 0.1f));
        }
    }
    public void ScrollTut()
    {
        if (selectNote.Count != 0)
        {
            selectNote[horizontalScrollSnap.CurrentPage].transform.GetChild(0).gameObject.SetActive(true);
            if (horizontalScrollSnap._previousPage != horizontalScrollSnap.CurrentPage)
                selectNote[horizontalScrollSnap._previousPage].transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    void InitPic()
    {
        if (!PlayerPrefs.HasKey("tut_event"))
        {
            panelTut.SetActive(true);
            PlayerPrefs.SetInt("tut_event", 1);
        }
        if (content.childCount != 0)
        {
            return;
        }
        int index = 0;
        Debug.Log(dataPictureJigsaw.picturePices);
        foreach (PicturePice picture in dataPictureJigsaw.picturePices)
        {
            PictureEventJigsaw jigsaw = Instantiate(pictureEvent, content);
            jigsaw.InitJigsaw(picture);
            if (picture.Completed)
                index++;
            GameObject step = Instantiate(preStep, contentStep);
            selectNote.Add(step);
        }
        horizontalScrollSnap.ChangePage(Mathf.Min(index, dataPictureJigsaw.picturePices.Count - 1));
        ScrollTut();
    }
    void InitState()
    {
        int index = 0;
        foreach (PicturePice picture in dataPictureJigsaw.picturePices)
        {
            Debug.Log("picture.Completed " + picture.Completed + "|" + picture.Unlock);
            if (picture.Completed)
            {
                index++;
            }
        }
        Debug.Log("index " + index);
        horizontalScrollSnap.ChangePage(Mathf.Min(index, dataPictureJigsaw.picturePices.Count - 1));
        ScrollTut();
    }
    public void ShowPig()
    {
        PopupPigGem.Setup().Show();
    }
}

