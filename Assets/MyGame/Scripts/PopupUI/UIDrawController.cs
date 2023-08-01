using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UIDrawController : MonoBehaviour
{
    [SerializeField] Transform contentPic;
    [SerializeField] ElementPic element;
    public static UIDrawController instance;
    public string oldContent;
    public Texture2D colorTexture;

    void Start()
    {
        instance = this;
        InitPic();
    }
    private void InitPic()
    {
        GameData.isDrawPixel = true;
        if (System.IO.Directory.Exists(GameData.DRAW_PATH))
        {
            foreach (var path in System.IO.Directory.GetFiles(GameData.DRAW_PATH))
            {
                string content = File.ReadAllText(path);
                PictureInformation pictureInfo = new PictureInformation(content);
                ElementPic elementPic = Instantiate(element, contentPic);
                elementPic.InitPic(pictureInfo, true);
            }
        }
    }
    private void OnEnable()
    {
        GameData.isDrawPixel = true;
        if (GameData.isEdit && oldContent != "")
        {
            AddItem(oldContent);
            oldContent = "";
            GameData.isEdit = false;
        }
        GameData.peletteColor = colorTexture;
    }
    private void OnDisable()
    {
        //GameData.isDrawPixel = false;
    }
    public void OpenSelectCanvas()
    {
        PopupSelectDrawPixel.Setup().Show();
    }
    public void AddItem(string content)
    {
        PictureInformation pictureInfo = new PictureInformation(content);
        ElementPic elementPic = Instantiate(element, contentPic);
        elementPic.InitPic(pictureInfo, true);
        elementPic.transform.SetSiblingIndex(1);
    }
}
