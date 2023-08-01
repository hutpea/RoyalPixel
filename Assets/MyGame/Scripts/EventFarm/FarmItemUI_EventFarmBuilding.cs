using System;
using System.Collections;
using System.Collections.Generic;
using BizzyBeeGames.ColorByNumbers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FarmItemUI_EventFarmBuilding : MonoBehaviour
{
    private const float PULSING_DISTANCE = 4.5f;
    private const float PULSING_TIME = 0.55f;

    public ElementPic elementPic;
    public int itemTimeLimit;
    public List<RectTransform> itemRectList;

    public void ElementPicButtonAddListener()
    {
        elementPic.GetComponent<Button>().onClick.AddListener(PlayButtonClicked);
    }

    public void ToggleElementPicButton(bool value)
    {
        elementPic.GetComponent<Button>().interactable = value;
        Image circleBackground = elementPic.GetComponent<Image>();
        if (value)
        {
            //circleBackground.color = new Color(204f / 255f, 1f, 230f / 255f);
            circleBackground.color = Color.white;
            elementPic.GetComponent<RectTransform>().sizeDelta = new Vector2(90, 90);
            StartCoroutine(PulsingEffectCoroutine());
        }
        else
        {
            circleBackground.color = Color.white;
            elementPic.GetComponent<RectTransform>().sizeDelta = new Vector2(40f, 40f);
        }
    }

    public void ShowRealItem(bool isImmediate = true, bool isDOFade = false)
    {
        //Debug.Log("ShowRealItem " + gameObject.name);
        //Texture2D texture2D = TextureController.Instance.GenerateGrayscaleTexture(pictureInformation, 1f, true);
        //Texture2D texture2D = TextureController.Instance.LoadCompletedTexture(pictureInformation);
        /*Rect rect = new Rect(0, 0, pictureInformation.XCells, pictureInformation.YCells);
            var texPic =
                TextureController.Instance.GenerateGrayscaleTexture(pictureInformation, 0.8f, true, isDraw: false);
            iRectTransform.GetComponent<Image>().sprite = Sprite.Create(texPic, rect, new Vector2(0, 0));*/
        if (isImmediate)
        {
            if (isDOFade)
            {
                foreach (var iRectTransform in itemRectList)
                {
                    if (iRectTransform.GetComponent<CanvasGroup>() != null)
                    {
                        iRectTransform.GetComponent<CanvasGroup>().alpha = 0;
                        iRectTransform.GetComponent<CanvasGroup>().DOFade(1, 1.5f);
                    }
                    else
                    {
                        Image _img = iRectTransform.GetComponent<Image>();
                        _img.color = new Color(1, 1, 1, 0);
                        _img.DOFade(1f, 1.5f);
                    }
                    iRectTransform.gameObject.SetActive(true);
                }
            }
            else
            {
                foreach (var iRectTransform in itemRectList)
                {
                    iRectTransform.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            StartCoroutine(ShowItemEffect());
        }
        Canvas.ForceUpdateCanvases();
    }

    private IEnumerator ShowItemEffect()
    {
        if (itemRectList.Count > 0)
        {
            float timeElapse = 2f / itemRectList.Count;
            foreach (var iRectTransform in itemRectList)
            {
                iRectTransform.gameObject.SetActive(true);
                Image image = iRectTransform.GetComponent<Image>();
                image.color = new Color(1, 1, 1, 0);
                image.DOFade(1f, timeElapse / 2f).OnComplete(delegate
                {

                });
                yield return new WaitForSeconds(timeElapse);
            }
        }
    }

    public void HideAllItemImages()
    {
        foreach (var iRectTransform in itemRectList)
        {
            iRectTransform.gameObject.SetActive(false);
        }
    }

    public void PlayButtonClicked()
    {
        GameData.isSelectInEventFarm = true;
        Debug.Log("PlayButtonClicked");
        GameData.DisableHandInFarm = true;
        //SceneManager.UnloadSceneAsync(1);
        elementPic.SelectPic();
    }

    private IEnumerator PulsingEffectCoroutine()
    {
        RectTransform itemRect = this.gameObject.GetComponent<RectTransform>();
        var originalPointY = itemRect.anchoredPosition.y;
        var targetPointY = itemRect.anchoredPosition.y + PULSING_DISTANCE;
        while (true)
        {
            itemRect.DOAnchorPosY(targetPointY, PULSING_TIME);
            yield return new WaitForSeconds(PULSING_TIME);
            itemRect.DOAnchorPosY(originalPointY, PULSING_TIME + 0.1f);
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}