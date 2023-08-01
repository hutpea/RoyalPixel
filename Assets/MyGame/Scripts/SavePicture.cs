using BizzyBeeGames.ColorByNumbers;
using EventDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static NativeGallery;

public class SavePicture : MonoBehaviour
{
    public static int sizeTex = 540;
    int m_size = 640;
    private static ShareType _typeShare;
    public Sprite logo;
    [SerializeField] CheckPermission checkPermission;
    [SerializeField] GameObject saveSuccess;
    public bool isAreas;
    [SerializeField] RawImage rawImage;
    [SerializeField] GameObject loadSave;
    [SerializeField] GameObject logoShareVip1, logoShareVip2;
    private System.Action<object> actionBuyVip;
    bool removeLogo;
    private void OnEnable()
    {
        removeLogo = false;
        actionBuyVip = (sender) => ActiveLogo();
        ActiveLogo();
        this.RegisterListener(EventID.BUY_VIP_SUB, actionBuyVip);
    }
    void ActiveLogo(object a = null)
    {
        if (UseProfile.IsVip)
        {
            logoShareVip1.SetActive(false);
            logoShareVip2.SetActive(false);
        }
    }
    private void OnDisable()
    {
        this.RemoveListener(EventID.BUY_VIP_SUB, actionBuyVip);
    }
    public enum ShareType
    {
        Unidefined = -1,
        SaveToDisk = 1,
        Facebook = 2,
        Instagram = 3,
        ShareNative = 4
    }
    public IEnumerator CreateVideoCoroutine(Action<string> handler)
    {
        Texture2D texSave;
        Texture2D newTex;
        int maxWidth = 0;
        loadSave.SetActive(true);
        if (isAreas)
        {
            //Debug.Log("save" + GameData.areasColor.width);
            texSave = Resources.Load("ColorAreas/c_" + GameData.IdAreaChoice) as Texture2D;
            newTex = new Texture2D((int)(1280 * ((float)texSave.width / (float)texSave.height)), 1280);
            newTex = Resize(texSave, newTex.width, newTex.height);
            Texture2D texMerge = new Texture2D(newTex.width, newTex.height);
            rawImage.texture = newTex;
            if (UseProfile.IsVip || removeLogo)
            {
                SaveImageToGallery(newTex, "Royal Pixel", GameData.IdAreaChoice.ToString(), MediaSaveCallback);
            }
            else
            {
                int startX = newTex.width - logo.texture.width - 30;
                int startY = logo.texture.height / 2;
                Vector2 startPos = new Vector2(startX, startY);
                texMerge = MergeTextures(newTex, logo.texture, startPos);
                SaveImageToGallery(texMerge, "Royal Pixel", GameData.IdAreaChoice.ToString(), MediaSaveCallback);
            }
            DailyQuestController.instance.UpdateProgressQuest(TypeQuest.SaveArea);
        }
        else
        {
            if (!GameData.isDrawPixel)
                texSave = GameData.CurColorTexture;
            else
            {
                texSave = TextureController.Instance.GenerateGrayscaleTexture(GameData.picChoice, 0.8f, true, true);
            }
            if (texSave.width >= texSave.height)
            {
                newTex = new Texture2D(sizeTex, (int)(sizeTex * ((float)texSave.height / (float)texSave.width)));
                newTex = Resize(texSave, newTex.width, newTex.height);
            }
            else
            {
                newTex = new Texture2D((int)(sizeTex * ((float)texSave.width / (float)texSave.height)), sizeTex);
                newTex = Resize(texSave, newTex.width, newTex.height);
            }
            Texture2D bg = new Texture2D(m_size, m_size, TextureFormat.RGB24, false);
            for (int i = 0; i < m_size; i++)
            {
                for (int j = 0; j < m_size; j++)
                    bg.SetPixel(i, j, Color.white);
            }
            bg.Apply();
            Texture2D texMerge = MergeTextures(bg, newTex, Vector2.zero);
            Texture2D texMergeLogo = new Texture2D(texMerge.width, texMerge.height);
            if (UseProfile.IsVip || removeLogo)
            {
                SaveImageToGallery(texMerge, "Royal Pixel", GameData.picChoice.Id, MediaSaveCallback);
            }
            else
            {
                int startX = texMerge.width - logo.texture.width - 15;
                int startY = logo.texture.height / 2;
                Vector2 startPos = new Vector2(startX, startY);
                texMergeLogo = MergeTextures(texMerge, logo.texture, startPos);
                SaveImageToGallery(texMergeLogo, "Royal Pixel", GameData.picChoice.Id, MediaSaveCallback);
            }
            DailyQuestController.instance.UpdateProgressQuest(TypeQuest.SavePic);
        }
        yield return null;
    }
    void MediaSaveCallback(bool succes, string path = "")
    {
        loadSave.SetActive(false);
        if (succes)
        {
            saveSuccess.SetActive(true);
            StartCoroutine(Helper.StartAction(() => saveSuccess.SetActive(false), 2f));
        }

    }
    public void CreateVideo(ShareType typeShare, Action<string> handler)
    {
        _typeShare = typeShare;
        if (NativeGallery.CheckPermission(PermissionType.Write) != Permission.Granted)
        {
            Permission permission = RequestPermission(PermissionType.Write);
            if (permission == Permission.Granted)
            {
                StartCoroutine(CreateVideoCoroutine(handler));
            }
            else
            {
                Debug.Log("save");
                AndroidMediaSharingFailSave();
            }
        }
        else
        {
            Debug.Log("save");
            StartCoroutine(CreateVideoCoroutine(handler));
            //processing.SetActive(true);
        }
    }

    private void AndroidMediaSharingFinish()
    {
        saveSuccess.SetActive(false);
        Debug.Log("finish");
    }
    private void AndroidMediaSharingFailSave()
    {
        saveSuccess.SetActive(false);
        Debug.Log("fail");
    }
    public void SaveToGallery()
    {
        CreateVideo(ShareType.SaveToDisk, null);
    }
    Texture2D Resize(Texture2D texture2D, int targetX, int targetY)
    {
        Debug.Log(targetX + "|" + targetY);
        RenderTexture rt = new RenderTexture(targetX, targetY, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D, rt);
        Texture2D result = new Texture2D(targetX, targetY);
        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        result.filterMode = FilterMode.Point;
        result.Apply();
        return result;
    }
    public Texture2D MergeTextures(Texture2D background, Texture2D layer1, Vector2 startPos)
    {
        int startX = (int)startPos.x;
        int startY = (int)startPos.y;
        if (startPos.x == 0)
        {
            startX = background.width - layer1.width;
            startX = (int)(startX * .5f);
            startY = background.height - layer1.height;
            startY = (int)(startY * .5f);
        }
        Texture2D newTex = new Texture2D(background.width, background.height, background.format, false);
        for (int x = 0; x < background.width; x++)
        {
            for (int y = 0; y < background.height; y++)
            {
                if (x >= startX && y >= startY && x < layer1.width + startX && y < layer1.height + startY)
                {
                    Color bgColor = background.GetPixel(x, y);
                    Color wmColor = layer1.GetPixel(x - startX, y - startY);

                    Color final_color = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);

                    newTex.SetPixel(x, y, final_color);
                }
                else
                    newTex.SetPixel(x, y, background.GetPixel(x, y));
            }
        }

        newTex.Apply();
        return newTex;
    }
    public void ShowRemoveLogo()
    {
        PopupRemoveLogo popupRemove = PopupRemoveLogo.Setup();
        popupRemove.Show();
        popupRemove.actionReward = RemoveLogo;
    }
    void RemoveLogo()
    {
        logoShareVip1.SetActive(false);
        removeLogo = true;
    }
}
