using BizzyBeeGames.ColorByNumbers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SuggestPic : MonoBehaviour
{
    [SerializeField] Transform contentPic;
    [SerializeField] GameObject elementPic;
    [SerializeField] ElementPic element;
    Texture2D[] pics;
    private void Start()
    {
        if (GameData.picChoice.IdArea != CategoryConst.FARM)
        {
            ShowPicSuggest();
        }
    }
    void ShowPicSuggest()
    {
        if (GameData.choicePicEvent)
            return;
        int totalPicSuggest = 0;
        if (GameData.picChoice.SinglePic)
        {
            CategoryInfo categoryInfo = new CategoryInfo();
            if (GameData.picChoice.SinglePicNoel)
            {
                //foreach (var item in GameController.Instance.noelEvent)
                //{
                //    if (item.Id == GameData.picChoice.IdArea)
                //    {
                //        categoryInfo = item;
                //        categoryInfo.PictureInfos = new List<PictureInformation>();
                //        for (int i = 0; i < categoryInfo.pictureFiles.Count; i++)
                //        {
                //            PictureInformation picture = new PictureInformation(categoryInfo.pictureFiles[i].text);
                //            categoryInfo.PictureInfos.Add(picture);
                //        }

                //        break;
                //    }
                //}
            }
            else
            {
                foreach (var item in GameController.Instance.dataPic.CategoryInfos.Values)
                {

                    Debug.Log("id" + GameData.picChoice.IdArea + "|" + item.Id);
                    if (GameData.picChoice.IdArea == CategoryConst.DAILY)
                    {
                        if (item.Id == CategoryConst.POPULAR)
                        {
                            categoryInfo = item;
                            break;
                        }
                    }
                    else
                        if (item.Id == GameData.picChoice.IdArea)
                    {
                        categoryInfo = item;
                        break;
                    }
                }
            }
            List<PictureInformation> pictures = new List<PictureInformation>();
            pictures = Helper.DisruptiveListObject(categoryInfo.PictureInfos);
            for (int i = 0; i < (GameData.picChoice.SinglePicNoel ? pictures.Count : pictures.Count / 3); i++)
            {
                totalPicSuggest++;
                //int index = i;
                //pics[i] = TextureController.Instance.GenerateGrayscaleTexture(pictures[i], 0.8f, true);
                ElementPic pic = Instantiate(element, contentPic);
                pic.InitPic(pictures[i]);
                //Rect rect = new Rect(0, 0, pics[i].width, pics[i].height);
                //pic.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(pics[i], rect, new Vector2(0, 0));
                //pic.GetComponent<Button>().onClick.AddListener(delegate { SelectPicture(pictures[index], index); });
                if (pictures[i].Completed)
                {
                    pic.gameObject.SetActive(false);
                    totalPicSuggest--;
                }
            }
        }
        else
        {
            pics = new Texture2D[GameData.pictures.Count];
            for (int i = 0; i < GameData.pictures.Count; i++)
            {
                totalPicSuggest++;
                if (GameData.GetUnlockArea(GameData.pictures[i].IdArea) || GameData.picChoice.IdArea == 0)
                {
                    gameObject.SetActive(false);
                    return;
                }
                int index = i;
                pics[i] = TextureController.Instance.GenerateGrayscaleTexture(GameData.pictures[i], 0.8f, true);
                GameObject pic = Instantiate(elementPic, contentPic);
                Rect rect = new Rect(0, 0, pics[i].width, pics[i].height);
                pic.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(pics[i], rect, new Vector2(0, 0));
                pic.GetComponent<Button>().onClick.AddListener(delegate { SelectPicture(GameData.pictures[index], index); });
                if (GameData.pictures[i].Completed)
                {
                    pic.SetActive(false);
                    totalPicSuggest--;
                }
            }
        }
        if (totalPicSuggest == 2)
        {
            contentPic.GetComponent<HorizontalLayoutGroup>().padding.left = 100;
        }
        if (totalPicSuggest == 1)
        {
            contentPic.GetComponent<HorizontalLayoutGroup>().padding.left = 250;
        }
    }
    public void SelectPicture(PictureInformation picture, int index)
    {
        //if (Application.internetReachability == NetworkReachability.NotReachable)
        //{
        //    PopupNoInternet.Setup().Show();
        //}
        //else
        //{
        //GameData.isEdit = false;
        //GameData.isDrawPixel = false;
        if (!picture.DrawPic)
            GameData.isDrawPixel = false;
        GameController.Instance.admobAds.ShowInterstitial(actionIniterClose: () =>
        {
            Debug.Log(picture.Id + "|" + picture.IdArea);
            GameData.picChoice = picture;
            Texture2D tex = pics[index];
            Texture2D texture = TextureController.Instance.GenerateColorPicture(picture);
            GameData.CurColorTexture = texture;
            GameData.curGrayTexture = tex;
            GameData.grayScale = TextureController.Instance.GenerateGrayscaleTexture(picture, 0.2f, false);
            LoadScene();
        }, actionWatchLog: "select_pic_suggest");
        //}
    }
    void LoadScene()
    {
        if (HomeController.instance != null)
        {
            SceneManager.UnloadScene(2);
            SceneManager.LoadScene(SceneName.GAME_PLAY, LoadSceneMode.Additive);
            HomeController.instance.EnableHomeScene(false);
        }
        else
        {
            SceneManager.LoadScene(SceneName.GAME_PLAY);
        }
    }
}
