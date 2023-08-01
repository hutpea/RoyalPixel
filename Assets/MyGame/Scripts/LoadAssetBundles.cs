using BizzyBeeGames.ColorByNumbers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadAssetBundles : MonoBehaviour
{
    private AssetBundle dataBundlesImage;
    private AssetBundle dataBundlesPic;
    public AssetBundle dataTexture;
    const string DATA_IMAGE_FILENAME = "dataimage";
    const string DATA_PICTURE_FILENAME = "datapicture";
    const string TEXTURE_ASSET = "textureassetbundle";
    [SerializeField] TextureController textureController;

    private void Start()
    {
        string path1 = Path.Combine(Application.persistentDataPath, "StreamingAssets", DATA_IMAGE_FILENAME);
        string path2 = Path.Combine(Application.persistentDataPath, "StreamingAssets", DATA_PICTURE_FILENAME);
        string path3 = Path.Combine(Application.persistentDataPath, "StreamingAssets", TEXTURE_ASSET);
        StartCoroutine(LoadDataAssetImage(path1, DATA_IMAGE_FILENAME));
        StartCoroutine(LoadDataAssetPic(path2, DATA_PICTURE_FILENAME));
        StartCoroutine(LoadDataAssetTexture(path3, TEXTURE_ASSET));
    }
    public IEnumerator LoadDataAssetImage(string path, string name)
    {
        Debug.Log("path " + path);
        if (dataBundlesImage == null)
        {
            AssetBundleCreateRequest createRequest = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(path));
            yield return createRequest;
            dataBundlesImage = createRequest.assetBundle;
            //GameController.Instance.dataImage = dataBundlesImage.LoadAsset<LoadDataPic>(name);
        }
        else
        {
            //GameController.Instance.dataImage = dataBundlesImage.LoadAsset<LoadDataPic>(name);
        }
        LoadFirstGame();
    }
    public IEnumerator LoadDataAssetPic(string path, string name)
    {
        if (dataBundlesPic == null)
        {
            AssetBundleCreateRequest createRequest = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(path));
            yield return createRequest;
            dataBundlesPic = createRequest.assetBundle;
            GameController.Instance.dataPic = dataBundlesPic.LoadAsset<LoadDataPic>(name);
        }
        else
        {
            GameController.Instance.dataPic = dataBundlesPic.LoadAsset<LoadDataPic>(name);
        }
    }
    public IEnumerator LoadDataAssetTexture(string path, string name)
    {
        if (dataTexture == null)
        {
            AssetBundleCreateRequest createRequest = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(path));
            yield return createRequest;
            dataTexture = createRequest.assetBundle;
        }
    }
    void LoadFirstGame()
    {
        if (GameController.Instance.useProfile.NewUser)
        {
            //for (int j = 0; j < GameController.Instance.dataImage.CategoryInfos["13"].pictureFiles.Count; j++)
            //{
            //    PictureInformation pictureInfo = new PictureInformation(GameController.Instance.dataImage.CategoryInfos["13"].pictureFiles[j].text);
            //    GameData.pictures.Add(pictureInfo);
            //}
            //GameData.totalPicInAreas = GameData.pictures.Count;
            //GameData.picChoice = GameData.pictures[3];
            //Debug.Log(" GameData.picChoice " + GameData.picChoice.Id);
            //Texture2D tex = textureController.GenerateGrayscaleTexture(GameData.picChoice, 0.8f, true);
            //Texture2D texture = textureController.GenerateColorPicture(GameData.picChoice);
            //GameData.CurColorTexture = texture;
            //GameData.curGrayTexture = tex;
            //GameData.grayScale = textureController.GenerateGrayscaleTexture(GameData.picChoice, 0.2f, false);
            //StartCoroutine(Helper.StartAction(() => GameController.Instance.LoadScene(SceneName.GAME_PLAY), 1));
        }
        else
            StartCoroutine(Helper.StartAction(() => GameController.Instance.LoadScene(SceneName.HOME_SCENE), 1));
    }
}
