using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Android;

public class CreateController : MonoBehaviour
{
    [SerializeField] CheckPermission checkPermission;
    [SerializeField] Transform contentPic;
    [SerializeField] ElementPic element;
    public static CreateController instance;
    void Start()
    {
        instance = this;
        InitPic();
    }
    private void InitPic()
    {
        if (System.IO.Directory.Exists(GameData.CREATE_PATH))
        {
            foreach (var path in System.IO.Directory.GetFiles(GameData.CREATE_PATH))
            {
                string content = File.ReadAllText(path);
                PictureInformation pictureInfo = new PictureInformation(content);
                ElementPic elementPic = Instantiate(element, contentPic);
                elementPic.InitPic(pictureInfo);
            }
        }
    }
    public void AddItem(string content)
    {
        PictureInformation pictureInfo = new PictureInformation(content);
        ElementPic elementPic = Instantiate(element, contentPic);
        elementPic.InitPic(pictureInfo);
    }
    public void ShowCamera()
    {
        checkPermission.finishPermission = ShowPanelCamera;
        checkPermission.RequestPermission();
    }
    void ShowPanelCamera()
    {
        PanelCreateManager.Setup().Show();
    }
    public void OnClickGralley()
    {
        //checkPermission.finishPermission = () =>
        {
            PanelCreateManager.Setup().OnClickGrallery();
            PanelCreateManager.Setup().galleryAction = ShowPanelCamera;
        };
        //checkPermission.RequestPermission();
    }
}
