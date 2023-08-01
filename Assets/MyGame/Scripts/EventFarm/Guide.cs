using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Guide : MonoBehaviour
{
    public RectTransform guideButtonRect;
    public Button guideButton;
    public Transform hand;
    public RectTransform Target;

    [SerializeField] private List<Sprite> sprites;
    
    public Canvas Canvas;

    private Vector4 center;

    private Material material;

    private float diameter;

    private float current;

    private Vector3[] corners = new Vector3[4];

    private float yVelocity;

    public Transform guideMask;

    public Transform guideArrow;

    private void Setup()
    {
        Target.GetWorldCorners(corners);
        diameter = Vector2.Distance(WordToCanvasPos(Canvas, corners[0]), WordToCanvasPos(Canvas, corners[2])) / 2f;
        float x = corners[0].x + (corners[3].x - corners[0].x) / 2f;
        float y = corners[0].y + (corners[1].y - corners[0].y) / 2f;
        Vector3 v = new Vector3(x, y, 0f);
        Vector2 localPoint = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(Canvas.transform as RectTransform, v, Canvas.GetComponent<Camera>(), out localPoint);
        v = new Vector4(localPoint.x, localPoint.y, 0f, 0f);
        material = GetComponent<Image>().material;
        material.SetVector("_Center", v);
        (Canvas.transform as RectTransform).GetWorldCorners(corners);
        for (int i = 0; i < corners.Length; i++)
        {
            current = Mathf.Max(Vector3.Distance(WordToCanvasPos(Canvas, corners[i]), v), current);
        }
        material.SetFloat("_Silder", current);
    }
    
    private void Update()
    {
        float a = Mathf.SmoothDamp(current, diameter, ref yVelocity, 0.2f);
        if (!Mathf.Approximately(a, current))
        {
            current = a;
            material.SetFloat("_Silder", current);
        }
    }

    private Vector2 WordToCanvasPos(Canvas canvas, Vector3 world)
    {
        Vector2 localPoint = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, world, canvas.GetComponent<Camera>(), out localPoint);
        return localPoint;
    }

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (Target == null || Target.gameObject == null || !Target.gameObject.activeInHierarchy)
        {
            return false;
        }
        bool flag = RectTransformUtility.RectangleContainsScreenPoint(Target, sp, eventCamera);
        return !flag;
    }
    
    public void SetGuideMask(Button button, UnityAction buttonAction, bool isShowHand = true, float scaleMultiplier = 1f)
    {
        guideMask.GetComponent<Image>().sprite = sprites[0];
        UnityEngine.Canvas canvas = button.GetComponent<Graphic>().canvas;
        Vector2 vector = canvas.transform.InverseTransformPoint(button.transform.position);
        Debug.Log("Guide mask inverse point: " + vector);
        RectTransform guideRect = guideMask.GetComponent<RectTransform>();
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        var maxSizeDelta = Mathf.Max(buttonRect.sizeDelta.x, buttonRect.sizeDelta.y);
        guideRect.sizeDelta = new Vector2(maxSizeDelta + 25f, maxSizeDelta + 25f) * scaleMultiplier;

        var pivot = buttonRect.pivot;
        var buttonSizeDelta = buttonRect.sizeDelta * scaleMultiplier;
        vector += new Vector2(-buttonSizeDelta.x * (pivot.x - 0.5f), -buttonSizeDelta.y * (pivot.y - 0.5f));
        guideRect.anchoredPosition = vector;
        
        guideButtonRect.sizeDelta = guideRect.sizeDelta;
        guideButton.onClick.RemoveAllListeners();
        if (buttonAction != null)
        {
            Debug.Log("Add listener to guideButton");
            guideButton.onClick.AddListener(buttonAction);
        }
        guideButton.interactable = true;
        
        if (isShowHand)
        {
            hand.gameObject.SetActive(true);
            hand.SetParent(Canvas.transform);
            hand.SetAsLastSibling();
            hand.GetComponent<RectTransform>().localScale = Vector3.zero;
            hand.GetComponent<RectTransform>().DOScale(1, 0.1f);
            hand.DOMove(button.transform.position, 0.1f).SetEase(Ease.InOutQuad);
        }
        else
        {
            hand.gameObject.SetActive(false);
        }
    }
    
    public void SetGuideMask(Image image, UnityAction buttonAction, bool isShowHand = true, bool isMaskShapeSquare = true)
    {
        if (isMaskShapeSquare)
        {
            guideMask.GetComponent<Image>().sprite = sprites[1];
        }
        else
        {
            guideMask.GetComponent<Image>().sprite = sprites[0];
        }
        UnityEngine.Canvas canvas = image.GetComponent<Graphic>().canvas;
        Vector2 vector = canvas.transform.InverseTransformPoint(image.transform.position);
        Debug.Log("Guide mask inverse point: " + vector);
        RectTransform guideRect = guideMask.GetComponent<RectTransform>();
        RectTransform imgRect = image.GetComponent<RectTransform>();
        //var maxSizeDelta = Mathf.Max(buttonRect.sizeDelta.x, buttonRect.sizeDelta.y);
        guideRect.sizeDelta = new Vector2(imgRect.sizeDelta.x + 50f, imgRect.sizeDelta.y + 50f);
        Debug.Log("guide a");
        var pivot = imgRect.pivot;
        var buttonSizeDelta = imgRect.sizeDelta;
        vector += new Vector2(-buttonSizeDelta.x * (pivot.x - 0.5f), -buttonSizeDelta.y * (pivot.y - 0.5f));
        guideRect.anchoredPosition = vector;
        Debug.Log("guide b");
        guideButtonRect.sizeDelta = guideRect.sizeDelta;
        guideButton.onClick.RemoveAllListeners();
        guideButton.onClick.AddListener(buttonAction);
        guideButton.interactable = true;
        Debug.Log("guide c");
        if (isShowHand)
        {
            hand.gameObject.SetActive(true);
            hand.SetParent(Canvas.transform);
            hand.SetAsLastSibling();
            hand.GetComponent<RectTransform>().localScale = Vector3.zero;
            hand.GetComponent<RectTransform>().DOScale(1, 0.1f);
            hand.DOMove(image.transform.position, 0.1f).SetEase(Ease.InOutQuad);
            Debug.Log("guide d");
        }
        else
        {
            hand.gameObject.SetActive(false);
        }
        Debug.Log("guide z");
    }

    public IEnumerator DelaySetGuideMaskImage(float waitDuration, Image image, UnityAction buttonAction, bool isShowHand = true, bool isMaskShapeSquare = true)
    {
        PopupEventFarmBuilding.instance?.preventClickLayer.gameObject.SetActive(true);
        yield return new WaitForSeconds(waitDuration);
        ToggleGuideMask(true);
        PopupEventFarmBuilding.instance?.preventClickLayer.gameObject.SetActive(false);
        SetGuideMask(image, buttonAction, isShowHand, isMaskShapeSquare);
    }

    public void ToggleGuideMask(bool value){
        guideMask.gameObject.SetActive(value);
    }
}
