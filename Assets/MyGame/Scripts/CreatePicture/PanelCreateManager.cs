using BizzyBeeGames.ColorByNumbers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tools.TextureOptimization;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelCreateManager : BaseBox
{
    public RawImage cameraVirtual, test;
    public Image preview, image;
    int frameSize = 230;
    public float scale = 1;
    Texture2D frameTexture, colorTexture;
    public Texture2D imageTexture;
    Color[] imageColors, colorTextureColors;
    int originalWidth;
    public Slider slider;
    public GameObject apply, selfie;
    public GameObject imgVideoAds;
    public bool webCam = true;
    public GameObject create;
    public WebCamera camTex;
    public GameObject panelNotAvaiable;
    private static PanelCreateManager instance;
    public GameObject btnUsePic;
    [SerializeField] CreatePicImportTexture importTexture;
    public UnityAction galleryAction;
    public bool selectPicGallery;
    public void OnClickGrallery()
    {
        //#if UNITY_EDITOR
        //        LoadImage(ReadAndWriteFile.ReadFile("Assets/Spirtes/plant.png"));
        //#else
        Debug.Log("click camera");
        Pickimage();
        //#endif
    }

    public static PanelCreateManager Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PanelCreateManager>(PathPrefabs.PANEL_CAMERA));
        }
        return instance;
    }
    void SetSolutionUI()
    {
        float aspectCam = GameData.aspect;
        float heigh = Camera.main.orthographicSize;
        RectTransform createRect = create.GetComponent<RectTransform>();
        if (aspectCam <= 828f / 1792)
        {
            float number = 3.7f;
            camTex.gameObject.GetComponent<RectTransform>().localScale = new Vector3(number, number, 1);
            create.GetComponent<RectTransform>().localScale = new Vector3(number, number, 1);
            create.GetComponent<RectTransform>().pivot = new Vector2(0f, 0f);
            Vector2 vt = camTex.gameObject.GetComponent<RectTransform>().localPosition;
            createRect.localPosition = new Vector2(vt.x - createRect.sizeDelta.x * number / 2, vt.y - createRect.sizeDelta.y * number / 2);

        }
        else if (aspectCam < 0.68f)
        {
            camTex.gameObject.GetComponent<RectTransform>().localScale = new Vector3(3.5f, 3.5f, 1);
            create.GetComponent<RectTransform>().localScale = new Vector3(3.5f, 3.5f, 1);
            create.transform.GetComponent<RectTransform>().pivot = new Vector2(0f, 0f);
            Vector2 vt = camTex.gameObject.GetComponent<RectTransform>().localPosition;
            createRect.localPosition = new Vector2(vt.x - createRect.sizeDelta.x * 3.5f / 2, vt.y - createRect.sizeDelta.y * 3.5f / 2);
        }
        else if (aspectCam >= 0.68f)
        {
            float number = 3.1f;
            camTex.gameObject.GetComponent<RectTransform>().localScale = new Vector3(number, number, 1);
            create.GetComponent<RectTransform>().localScale = new Vector3(number, number, 1);
            create.GetComponent<RectTransform>().pivot = new Vector2(0f, 0f);
            Vector2 vt = camTex.gameObject.GetComponent<RectTransform>().localPosition;
            createRect.localPosition = new Vector2(vt.x - createRect.sizeDelta.x * number / 2, vt.y - createRect.sizeDelta.y * number / 2);
        }
    }
    protected override void OnEnable()
    {
        selectPicGallery = false;
        SetSolutionUI();
        webCam = true;
        camTex.gameObject.SetActive(true);
        create.SetActive(false);
        apply.SetActive(false);
        selfie.SetActive(true);
        scale = 1;
        slider.value = (slider.minValue + slider.maxValue) / 2f;
        preview.sprite = null;
        if (/*GameData.firstUseCreatePic || */UseProfile.IsVip)
        {
            imgVideoAds.SetActive(false);
        }
        else
            imgVideoAds.SetActive(true);
        colorTexture = null;
        frameTexture = null;
        base.OnEnable();
    }
    bool askingPermission;
    private void OnApplicationFocus(bool focus)
    {
        if (askingPermission && focus)
        {
            askingPermission = false;
            if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                Pickimage();
            }
        }
    }
    public void Pickimage()
    {
        //#if UNITY_ANDROID
        //        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite)) {
        //            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        //            askingPermission = true;
        //            return;
        //        }
        //#endif
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Couldn't load texture from 123");
            if (path != null)
            {
                Debug.Log("Couldn't load texture from 1234");
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, -1, false, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    //Toast.instance.ShowMessage("Couldn't load texture");
                    return;
                }
                //createListItems.SetActive(false); 
                //createScreen.SetActive(true);
                if (galleryAction != null)
                {
                    galleryAction.Invoke();
                }
                LoadImage(texture);
                GameData.typeCreate = "gallery";
                //GameData.locationStart = "gallery";
            }
        }, "Select a PNG image", "image/png");
#if UNITY_IOS
        if (permission == NativeGallery.Permission.Denied) {
                    GameController.Instance.ShowPanelPermissionSetting(1);
        }
#endif
    }
    public void LoadImage(Texture2D texture2D)
    {
        webCam = false;
        apply.SetActive(true);
        selfie.SetActive(false);
        camTex.gameObject.SetActive(false);
        create.gameObject.SetActive(true);
        Color32[] importColor = new Color32[texture2D.width * texture2D.width];
        imageTexture = texture2D;
        int width = imageTexture.width;
        int height = imageTexture.height;
        int newWidth, newHeight;
        if (height > width)
        {
            newWidth = frameSize;
            newHeight = Mathf.RoundToInt(height / (float)width * newWidth);
        }
        else
        {
            newHeight = frameSize;
            newWidth = Mathf.RoundToInt(width / (float)height * newHeight);
        }

        TextureScale.Point(imageTexture, newWidth, newHeight);
        originalWidth = imageTexture.width;
        imageColors = imageTexture.GetPixels();
        for (int i = 0; i < imageColors.Length; i++)
        {
            if (imageColors[i].a != 1)
                imageColors[i] = new Color(imageColors[i].r, imageColors[i].g, imageColors[i].b, 225);
        }
        Rect rec = new Rect(0, 0, imageTexture.width, imageTexture.height);
        image.sprite = Sprite.Create(imageTexture, rec, new Vector2(0.5f, 0.5f), 100);
        image.SetNativeSize();

        if (height > width)
        {
            var pos = image.transform.localPosition;
            image.transform.localPosition = new Vector3(0, -(newHeight - frameSize) / 2, 0);
        }
        else
        {
            var pos = image.transform.localPosition;
            image.transform.localPosition = new Vector3(-(newWidth - frameSize) / 2, 0, 0);
        }
        UpdateTexture();
        selectPicGallery = true;
    }

    public void UpdateTexture()
    {
        int size = Mathf.RoundToInt(frameSize / scale);
        frameTexture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        int index = 0;
        Color[] colors = new Color[size * size];

        int offsetY = Mathf.RoundToInt(-image.transform.localPosition.y / scale);
        int offsetX = Mathf.RoundToInt(-image.transform.localPosition.x / scale);
        for (int i = offsetY; i < size + offsetY; i++)
        {
            for (int j = offsetX; j < size + offsetX; j++)
            {
                if (originalWidth * i + j < imageColors.Length)
                {
                    Color c = imageColors[originalWidth * i + j];
                    colors[index] = c;
                    index++;
                }
            }
        }
        frameTexture.SetPixels(colors);
        UpdateColorTexture();
    }
    private void UpdateColorTexture()
    {
        colorTexture = Instantiate(frameTexture);
        float scaleValue = (slider.minValue + slider.maxValue) - slider.value;
        if (scaleValue != 1)
        {
            int newSize = (int)(frameTexture.width / scaleValue);
            TextureScale.Point(colorTexture, newSize, newSize);
        }
        colorTexture.filterMode = FilterMode.Point;
        Rect rec = new Rect(0, 0, colorTexture.width, colorTexture.height);
        preview.sprite = Sprite.Create(colorTexture, rec, new Vector2(0.5f, 0.5f), 100);
    }
    public void UpdateColorCameraVirtual(Texture2D tex)
    {
        if (tex == null)
            return;
        //test.texture = texture;
        float scaleValue = (slider.minValue + slider.maxValue) - slider.value;
        if (scaleValue != 1)
        {
            int newSize = (int)(frameSize / scaleValue);
            TextureScale.Point(tex, newSize, newSize);
        }
        tex.filterMode = FilterMode.Point;
        camTex.display.texture = tex;
    }
    public void UpdateColorCameraVirtual()
    {
        Texture2D tex = Instantiate(camTex.texStop);
        if (tex == null)
            return;
        float scaleValue = (slider.minValue + slider.maxValue) - slider.value;
        if (scaleValue != 1)
        {
            int newSize = (int)(frameSize / scaleValue);
            TextureScale.Point(tex, newSize, newSize);
        }
        tex.filterMode = FilterMode.Point;
        camTex.display.texture = tex;
    }
    public void OnSliderChanged()
    {
        if (camTex.stop)
            UpdateColorCameraVirtual();
        else
        {
            if (imageTexture == null) return;
            if (!webCam)
                UpdateColorTexture();
        }
    }
    public void UsePicture()
    {
        if (UseProfile.IsVip)
        {
            LoadGame();
            return;
        }
        GameController.Instance.admobAds.ShowVideoReward(() => { LoadGame(); }, () => { ShowFail(); }, () => { }, ActionWatchVideo.UsePicCamera, "-1");
    }

    void ShowFail()
    {
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
   (
      btnUsePic.transform.position,
       "No Video Ads",
       Color.green,
       isSpawnItemPlayer: true
   );
    }
    void LoadGame()
    {
        DailyQuestController.instance.UpdateProgressQuest(TypeQuest.UsePicCamera);
        Texture2D texture;
        if (webCam)
        {
            float orient = -camTex.tex.videoRotationAngle;
            texture = ApplyPic((Texture2D)camTex.display.texture);
            Debug.Log("orient" + orient);
            switch (orient)
            {
                case 0:
                    Texture2D oldTexture = texture;
#if UNITY_ANDROID
                    if (camTex.currentCamindex == 0)
                    {
                        texture = camTex.Rotate(texture, clockwise: true);
                    }
                    else
                    {
                        texture = camTex.Rotate(texture, clockwise: false);
                    }
#else
                    texture = camTex.Rotate(texture, clockwise: true);
#endif
                    Destroy(oldTexture);
                    break;
            }
        }
        else
            texture = ApplyPic(colorTexture);
        importTexture.ImportTexture(texture);
        StartCoroutine(ReadFilePic());
    }
    IEnumerator ReadFilePic()
    {
        PictureInformation pictureInformation = new PictureInformation(GameData.content);
        CreateController.instance.AddItem(GameData.content);
        GameData.picChoice = pictureInformation;
        //GameData.SaveCratePic(pictureInformation.Id);
        Texture2D tex = TextureController.Instance.GenerateGrayscaleTexture(pictureInformation, 0.8f, false); ;
        Texture2D texture = TextureController.Instance.GenerateColorPicture(pictureInformation);
        GameData.CurColorTexture = texture;
        GameData.curGrayTexture = tex;
        GameData.grayScale = TextureController.Instance.GenerateGrayscaleTexture(pictureInformation, 0.2f, false);
        SceneManager.LoadScene(SceneName.GAME_PLAY, LoadSceneMode.Additive);
        HomeController.instance.EnableHomeScene(false);
        gameObject.SetActive(false);
        yield return null;
    }
    public Texture2D ApplyPic(Texture2D texture)
    {
        Texture2D texture2D = texture;
        int num = (int)Mathf.Lerp(15f, 60f, (slider.value - slider.minValue) / (slider.maxValue - slider.minValue));
        if (ColoringTexture.CheckToNeedConverColor(texture2D, num))
        {
            texture2D = TextureColorsReducer.Process(texture2D, num);
        }
        return texture2D;
    }
    private bool IsColorSimilar(Color32 color1, Color32 color2)
    {
        return Mathf.Abs(color1.r - color2.r) + Mathf.Abs(color1.g - color2.g) + Mathf.Abs(color1.b - color2.b) <= 60;
    }

}
