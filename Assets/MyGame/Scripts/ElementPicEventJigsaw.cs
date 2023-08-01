using BizzyBeeGames.ColorByNumbers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ElementPicEventJigsaw : MonoBehaviour
{
    [SerializeField] Image status;
    private Texture2D texPic;
    private int id;
    [SerializeField] Text txtProgress;
    [SerializeField] Image bg;
    [SerializeField] Image imgTypePic;
    [SerializeField] Sprite spAds, spVip;
    PictureInformation picture;
    OncePice pice;
    [SerializeField] Material sourceAlpha;
    [SerializeField] Sprite icon_play;
    private void OnEnable()
    {
        if (pice != null)
            InitPicture(pice);
    }
    public void InitPicture(OncePice pice)
    {
        this.id = pice.ID;
        this.pice = pice;
        GameData.isDrawPixel = false;
        if (DataPictureJigsaw.Current == pice.ID)
        {
            PictureInformation pictureInfo = new PictureInformation(pice.textAsset.text);
            this.picture = pictureInfo;
            Rect rect = new Rect(0, 0, pictureInfo.XCells, pictureInfo.YCells);
            texPic = new Texture2D(pictureInfo.XCells, picture.YCells);
            texPic = TextureController.Instance.GenerateGrayscaleTexture(pictureInfo, 0.8f, true, false);
            status.sprite = icon_play;
            if (picture.TotalPixelPainted != 0)
            {
                txtProgress.transform.parent.gameObject.SetActive(true);
                txtProgress.text = (int)(((float)picture.TotalPixelPainted / picture.TotalPixel) * 100) + "%";
            }
            if (!UseProfile.IsVip)
            {
                if (picture.PictureType == PictureType.Ads && picture.CurrentAds > 0)
                {
                    imgTypePic.gameObject.SetActive(true);
                    imgTypePic.sprite = spAds;
                }
                if (picture.PictureType == PictureType.Vip)
                {
                    imgTypePic.gameObject.SetActive(true);
                    imgTypePic.sprite = spVip;
                }
            }
            else
                imgTypePic.gameObject.SetActive(false);
        }
        else if (id < DataPictureJigsaw.Current)
        {
            status.gameObject.SetActive(false);
            Material mt = new Material(sourceAlpha);
            bg.enabled = false;
            //mt.SetFloat("_SourceAlphaDissolveFade", 4);
            ////StartCoroutine(DoColorImage(mt, "_SourceAlphaDissolveFade", 2.5f));
            //bg.material = mt;
            imgTypePic.enabled = false;
            txtProgress.transform.parent.gameObject.SetActive(false);
        }
    }
    //IEnumerator DoColorImage(Material material, string name, float number)
    //{
    //    while (number >= 0)
    //    {
    //        number -= 0.04f;
    //        material.SetFloat(name, number);
    //        yield return new WaitForFixedUpdate();
    //    }

    //    yield return null;
    //}
    public void SelectPic()
    {
        //if (Application.internetReachability == NetworkReachability.NotReachable)
        //{
        //    PopupNoInternet.Setup().Show();
        //}
        GameData.isDrawPixel = false;
        if (picture == null || id != DataPictureJigsaw.Current)
            return;
        if (UseProfile.IsVip)
        {
            PlayGame();
        }
        else
        {
            //if (Application.internetReachability == NetworkReachability.NotReachable)
            //{
            //    PopupNoInternet.Setup().Show();
            //}
            //else
            //{
            switch (picture.PictureType)
            {
                case PictureType.Ads:
                    if (picture.TotalPixelPainted != 0 || picture.CurrentAds == 0)
                    {
                        PlayGame();
                    }
                    else
                        GameController.Instance.admobAds.ShowVideoReward(() => PlayGame(), () => ShowFail(), () => { }, ActionWatchVideo.UnlockPicEvent, id.ToString());
                    break;
                case PictureType.Gem:
                    PlayGame();
                    break;
                case PictureType.Free:
                    GameController.Instance.admobAds.ShowInterstitial(false, "select_pic_event", () => { PlayGame(); });
                    break;
                case PictureType.Vip:
                    PopupVip.Setup().Show();
                    break;
            }
        }
    }
    void PlayGame()
    {
        GameData.choicePicEvent = true;
        if (picture.CurrentAds != 0)
            picture.CurrentAds--;
        if (picture.PictureType == PictureType.Ads)
            DailyQuestController.instance.UpdateProgressQuest(TypeQuest.UnlockPic);
        GameData.picChoice = picture;
        Texture2D tex = texPic;
        Texture2D texture = TextureController.Instance.GenerateColorPicture(picture);
        GameData.CurColorTexture = texture;
        GameData.curGrayTexture = tex;
        GameData.grayScale = TextureController.Instance.GenerateGrayscaleTexture(picture, 0.2f, false);

        if (HomeController.instance != null && UIGameController.instance != null && UIGameController.instance.gameObject.activeInHierarchy)
        {
            SceneManager.UnloadScene(2);
            SceneManager.LoadScene(SceneName.GAME_PLAY, LoadSceneMode.Additive);
            HomeController.instance.EnableHomeScene(false);
        }
        else
        {
            SceneManager.LoadScene(SceneName.GAME_PLAY, LoadSceneMode.Additive);
        }
        HomeController.instance.EnableHomeScene(false);
        PopupEventJigsaw.Setup().Close();
        //}
    }
    void ShowFail()
    {
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
   (
      transform.position,
       "No Video Ads",
       Color.red,
       isSpawnItemPlayer: true
   );
    }
}
