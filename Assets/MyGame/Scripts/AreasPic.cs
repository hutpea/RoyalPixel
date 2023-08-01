using BizzyBeeGames.ColorByNumbers;
using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AreasPic : MonoBehaviour
{
    public Transform posObjects;
    Texture2D[] pics;
    public Image bg;
    int idArea;
    bool show;
    public Sprite startSprites;
    PictureInformation[] tempPics;
    public bool isJigsaw;
    private void Start()
    {
        if (!show)
        {
            StartCoroutine(ShowChoice(0.5f));
            show = true;
        }
    }
    private void OnEnable()
    {
        for (int i = 0; i < posObjects.childCount; i++)
        {
            posObjects.GetChild(i).transform.GetChild(0).DOKill();
            posObjects.GetChild(i).transform.GetChild(0).localScale = Vector3.zero;
        }

        if (show)
            StartCoroutine(ShowChoice(0));
        if (tempPics != null)
        {
            InitArea(tempPics);
            if (GameData.CompleteArea)
            {
                if (HomeController.instance.contentViewNormalArea.transform.childCount != 0)
                    Destroy(HomeController.instance.contentViewNormalArea.transform.GetChild(0).gameObject);
                if (HomeController.instance.contentViewNewArea.childCount != 0)
                {
                    Destroy(HomeController.instance.contentViewNewArea.GetChild(0).gameObject);
                }
                HomeController.instance.panelViewArea.SetActive(true);
                bg.material = null;
                if (isJigsaw)
                {
                    HomeController.instance.contentViewNewArea.transform.parent.gameObject.SetActive(true);
                    HomeController.instance.contentViewNormalArea.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    HomeController.instance.contentViewNewArea.transform.parent.gameObject.SetActive(false);
                    HomeController.instance.contentViewNormalArea.transform.parent.gameObject.SetActive(true);
                }
                AreasPic pic = (Instantiate(Resources.Load("Areas/" + idArea), isJigsaw ? HomeController.instance.contentViewNewArea : HomeController.instance.contentViewNormalArea) as GameObject).GetComponent<AreasPic>();
                pic.InitArea(GameController.Instance.dataAreas.cateItems[GameData.PaintingCateAreas].dataPic.CategoryInfos[idArea.ToString()].PictureInfos.ToArray());
                pic.bg.sprite = AreaController.instance.dataArena.colorSprite[idArea - 1];
                GameData.IdAreaChoice = idArea;
                pic.ViewPic();
                foreach (Transform child in pic.posObjects)
                {
                    child.GetComponent<Button>().enabled = false;
                }
                GameData.CompleteArea = false;
            }
        }
    }
    IEnumerator ShowChoice(float timer)
    {
        yield return new WaitForSeconds(timer);
        for (int i = 0; i < posObjects.childCount; i++)
        {
            posObjects.GetChild(i).transform.GetChild(0).DOScale(new Vector3(1, 1, 1), 0.6f);
        }
    }
    public void InitArea(PictureInformation[] pictures)
    {
        Debug.Log(GameData.isDrawPixel + "draw");
        this.tempPics = pictures;
        bg = transform.GetChild(0).GetComponent<Image>();
        startSprites = bg.sprite;
        pics = new Texture2D[pictures.Length];
        for (int i = 0; i < pictures.Length; i++)
        {
            int index = i;
            pics[i] = TextureController.Instance.GenerateGrayscaleTexture(pictures[i], 0.8f, true);
            posObjects.GetChild(i).GetComponent<RawImage>().texture = pics[i];
            posObjects.GetChild(index).GetComponent<Button>().onClick.RemoveAllListeners();
            posObjects.GetChild(index).GetComponent<Button>().onClick.AddListener(delegate { SelectPicture(pictures[index], index); });
            if (pictures[i].Completed)
            {
                posObjects.GetChild(i).transform.GetChild(0).gameObject.SetActive(false);
            }
            idArea = pictures[i].IdArea;
        }
        HomeController.instance.pictures = pictures.ToList();
        GameData.pictures = pictures.ToList();
        if ((!GameData.GetUnlockArea(idArea) && !GameData.CompleteArea) || (GameData.GetUnlockArea(idArea) && GameData.CompleteArea))
            bg.material.SetFloat("_Saturation", 0);
        else
        {
            bg.material.SetFloat("_Saturation", 1);
        }
        if (GameData.GetUnlockArea(idArea) && !GameData.CompleteArea)
        {
            bg.material = null;
        }
        Tutorial();
    }
    public void Tutorial()
    {
        //if (GameController.Instance.useProfile.NewUser)
        //{
        //    StartCoroutine(ShowSequenceImage());
        //}
    }
    IEnumerator ShowSequenceImage()
    {
        yield return new WaitForSeconds(0.15f);
        foreach (Transform child in posObjects)
        {
            StartCoroutine(child.GetComponent<Pulsing>().StartPulsing());
            yield return new WaitForSeconds(0.15f);
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
        if (!picture.DrawPic)
            GameData.isDrawPixel = false;
        GameController.Instance.admobAds.ShowInterstitial(false, ActionWatchVideo.SelectPicInArea.ToString(), () => Success(picture, index));

        //}
    }
    void Success(PictureInformation picture, int index)
    {
        Debug.Log(picture.Id + "|" + picture.IdArea);
        GameData.picChoice = picture;
        Texture2D tex = pics[index];
        Texture2D texture = TextureController.Instance.GenerateColorPicture(picture);
        GameData.CurColorTexture = texture;
        GameData.curGrayTexture = tex;
        GameData.grayScale = TextureController.Instance.GenerateGrayscaleTexture(picture, 0.2f, false);
        SceneManager.LoadScene(SceneName.GAME_PLAY, LoadSceneMode.Additive);
        Debug.Log("loaddd game");
        HomeController.instance.EnableHomeScene(false);
    }
    public void ViewPic()
    {
        bg.material.SetFloat("_Saturation", 0);
        if (isJigsaw)
        {
            transform.GetChild(transform.childCount - 1).gameObject.SetActive(false);
            HomeController.instance.contentViewNewArea.transform.parent.gameObject.SetActive(true);
            HomeController.instance.contentViewNormalArea.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            HomeController.instance.contentViewNewArea.transform.parent.gameObject.SetActive(false);
            HomeController.instance.contentViewNormalArea.transform.parent.gameObject.SetActive(true);
        }
        foreach (Transform child in posObjects)
        {
            child.gameObject.SetActive(false);
            child.GetChild(0).gameObject.SetActive(false);
            Material material = new Material(Shader.Find("Sprite Shaders Ultimate/Standard/Fading/Source Alpha Dissolve"));
            material.SetFloat("_SourceAlphaDissolveFade", 0);
            child.GetComponent<RawImage>().material = material;
        }
        bg = transform.GetChild(0).GetComponent<Image>();
        bg.material = new Material(Shader.Find("Sprite Shaders Ultimate/Standard/Color/Saturation"));
        bg.material.SetFloat("_Saturation", 0);
        StartCoroutine(IEShowPic());
    }
    private IEnumerator IEShowPic()
    {
        foreach (Transform child in posObjects)
        {
            child.gameObject.SetActive(true);
            StartCoroutine(DoColorImage(child.GetComponent<RawImage>().material, "_SourceAlphaDissolveFade", 2));
            yield return new WaitForSecondsRealtime(0.5f);
        }
        yield return new WaitForSecondsRealtime(0.5f);
        StartCoroutine(DoColorImage(bg.material, "_Saturation", 1));
        bg.sprite = startSprites;
        GameController.Instance.musicManager.PlayWinSound();
        yield return new WaitForSecondsRealtime(0.5f);
        HomeController.instance.panelViewArea.GetComponent<ViewPic>().particle.Play();
        foreach (Transform child in posObjects)
        {
            child.GetComponent<Button>().enabled = true;
        }
    }
    IEnumerator DoColorImage(Material material, string name, float number)
    {
        float color = 0;
        while (color <= number)
        {
            color += 0.02f;
            material.SetFloat(name, color);
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }
}
