using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public class PopupPigGem : BaseBox
{
    private static PopupPigGem instance;
    public Canvas canvas;
    [SerializeField] Text txtGem;
    [SerializeField] Image image;
    [SerializeField] float maxGem;
    [SerializeField] Transform btnBuy;
    [SerializeField] Text txtComplete;
    [SerializeField] SkeletonGraphic pig;
    IEnumerator coroutine;
    [SerializeField] Text txtFull;
    public static PopupPigGem Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupPigGem>(PathPrefabs.POPUP_PIG_GEM));
        }
        GameController.Instance.musicManager.PlaySoundPigGem();
        instance.InitPopup();
        return instance;
    }
    
    public void BuySucces()
    {
        GameController.Instance.musicManager.PlayProgress();
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp(btnBuy.transform.position, "BUY COMPLETE", Color.green);
        txtComplete.gameObject.SetActive(true);
        GameData.Gem += (int)GameData.CountGemPig;
    }
    public void InitPopup()
    {
        if(GameData.CountGemPig>=1000)
        {
            txtFull.gameObject.SetActive(true);
        }
        txtComplete.gameObject.SetActive(false);
        txtGem.text = GameData.CountGemPig.ToString();
        image.fillAmount = GameData.CountGemPig / maxGem;
    }
    public override void Show()
    {
        base.Show();
        instance.canvas.sortingLayerName = "Popup";
        if (instance.canvas.sortingOrder <= 25)
        {
            instance.canvas.sortingOrder = 60;
        }
        pig.AnimationState.SetAnimation(0, "anim1", false);
        coroutine = AutoShowAnim();
        StartCoroutine(coroutine);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        if (coroutine != null)
            StopCoroutine(coroutine);
    }
    IEnumerator AutoShowAnim()
    {
        yield return new WaitForSeconds(1.6f);
        pig.AnimationState.SetAnimation(0, "anim2", true);
    }    
}
