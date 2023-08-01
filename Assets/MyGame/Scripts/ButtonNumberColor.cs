using EventDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class ButtonNumberColor : MonoBehaviour
{
    [SerializeField] Text txtNumber;
    [SerializeField] Image imgBtn;
    [SerializeField] Image slider;
    int number;
    float totalPixel;
    [SerializeField] GameObject tick;
    private Action<object> setValue;
    public void Init(int number, Color color)
    {
        txtNumber.text = number.ToString();
        txtNumber.color = color.grayscale < 0.5f ? Color.white : Color.black;
        imgBtn.color = color;
        this.number = number;
        slider.color = color;
        if (!GameData.isDrawPixel)
        {
            slider.gameObject.SetActive(true);
            txtNumber.gameObject.SetActive(true);
            totalPixel = GamePlayControl.Instance.numberColoring.numTileStart[number];
            if (GamePlayControl.Instance.numberColoring.numTileUnPainted.ContainsKey(number))
            {
                float totalPixedUnPaint = GamePlayControl.Instance.numberColoring.numTileUnPainted[number];
                float progess = 1 - totalPixedUnPaint / totalPixel;
                slider.fillAmount = progess;
            }
            else
            {
                tick.SetActive(true);
                txtNumber.gameObject.SetActive(false);
                StartCoroutine(Helper.StartAction(() => gameObject.SetActive(false), 0.1f));
            }
        }
        else
        {
            slider.gameObject.SetActive(false);
            txtNumber.gameObject.SetActive(false);
        }
    }
    private void Start()
    {
        if (!GameData.isDrawPixel)
        {
            setValue = (sender) => SetSliderBtnColor((int)sender);
            EventDispatcher.EventDispatcher.Instance.RegisterListener(EventID.DOPAINT, setValue);
        }
    }
    private void OnDisable()
    {
        EventDispatcher.EventDispatcher.Instance.RemoveListener(EventID.DOPAINT, setValue);
    }
    public void SelectButton()
    {
        //if (GameController.Instance.useProfile.NewUser)
        //{
        //    UIGameController.instance.tutHandStar.SetActive(false);
        //    UIGameController.instance.tutHandNumberColor.SetActive(false);
        //    GamePlayControl.Instance.lockCamera = false;
        //}
        //GamePlayControl.Instance.currentButtonChoice.localScale = new Vector3(1, 1, 1);
        //GamePlayControl.Instance.currentButtonChoice = transform;
        GamePlayControl.Instance.posChoice.position = gameObject.transform.position;
        GamePlayControl.Instance.posChoice.SetParent(gameObject.transform);
        GamePlayControl.Instance.posChoice.SetAsFirstSibling();
        GamePlayControl.Instance.selectedNumber = number;
        GamePlayControl.Instance.useEraser = false;
        UIGameController.instance.choiceEraser.SetActive(false);
        GamePlayControl.Instance.selectEraser = 0;
        if (!GameData.isDrawPixel)
        {
            GamePlayControl.Instance.numberColoring.SetHighLight(number);
            GamePlayControl.Instance.useItemBomb = false;
            GamePlayControl.Instance.useItemStar = false;
            GamePlayControl.Instance.useItemPen = false;
            GamePlayControl.Instance.posChoice.gameObject.SetActive(true);
            UIGameController.instance.choiceItemBomb.SetActive(false);
            UIGameController.instance.choiceItemStar.SetActive(false);
            UIGameController.instance.choiceItemPen.SetActive(false);
        }
        //transform.DOScale(new Vector3(1.15f, 1.15f, 1.15f), 0.2f);
    }
    public void SetSliderBtnColor(int value)
    {
        if (number == GamePlayControl.Instance.tempSelectNumber)
        {
            float fill = 1 - (float)value / totalPixel;
            slider.fillAmount = fill;
            if (fill >= 1)
            {
                //GameController.Instance.musicManager.PlaySoundCompleteColor();
                gameObject.SetActive(false);
            }
        }
    }
}