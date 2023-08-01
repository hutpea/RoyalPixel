using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewPic : MonoBehaviour
{
    public ParticleSystem particle;
    public SavePicture savePicture;
    [SerializeField] Transform btnSave;
    private void OnEnable()
    {
        //LoadTexture();
        //GameController.Instance.admobAds.DestroyBanner();
    }
    IEnumerator LoadTexture()
    {
        yield return new WaitForFixedUpdate();
        Image bg = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        GameData.areasColor = bg.mainTexture as Texture2D;
    }
    private void OnDisable()
    {
        //GameController.Instance.admobAds.ShowBanner();
        //HomeController.instance.SetActiveTab(1);
        particle.Stop();

    }
    public static Texture2D textureFromSprite(Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width)
        {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                         (int)sprite.textureRect.y,
                                                         (int)sprite.textureRect.width,
                                                         (int)sprite.textureRect.height);
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        }
        else
            return sprite.texture;
    }
    public void SavePic()
    {
        GameController.Instance.admobAds.ShowVideoReward(() => { DailyQuestController.instance.UpdateProgressQuest(TypeQuest.FinishArea); savePicture.SaveToGallery(); }, FailToLoad, null, ActionWatchVideo.SavePic, GameData.IdAreaChoice.ToString());
    }
    public void FailToLoad()
    {
        Vector3 pos = new Vector3(btnSave.transform.position.x - 1, btnSave.transform.position.y, btnSave.transform.position.z);
        //StartCoroutine(Helper.StartAction(() =>
        //{
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp
    (
       pos,
        "Video is not available!",
        Color.green,
        isSpawnItemPlayer: true
    );
        // }, 0.5f));
    }
    public void ShowVip()
    {
        PopupVip.Setup().Show();
    }
}
