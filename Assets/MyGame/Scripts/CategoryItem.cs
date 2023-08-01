using BizzyBeeGames.ColorByNumbers;
using EnhancedScrollerDemos.CellEvents;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CategoryItem : MonoBehaviour, IEnhancedScrollerDelegate
{
    public int id;
    public int totalPic;
    public List<PictureInformation> pictures;
    public string nameCate = "";
    public GridSimulate element;
    [SerializeField] Transform contentPic;
    [SerializeField] GameObject empty;
    //[SerializeField] AdmobNativeUI elementNativeAds;
    //AdmobNativeUI native;
    bool loadScucces;
    float yStartContent;
    bool loadDone;
    float aspect;
    private SmallList<PictureInformation> _data;
    public EnhancedScroller scroller;
    [SerializeField] UIScrollView scrollView;
    float widthContent;

    private void Start()
    {
        scroller.Delegate = this;
    }
    public void InitCategory(CategoryInfo category)
    {
        _data = new SmallList<PictureInformation>();
        _data.Clear();
        this.id = category.Id;
        this.totalPic = category.PictureInfos.Count;
        this.pictures = category.PictureInfos;
        this.nameCate = category.displayName.ToUpper();
        LoadListPic(category.PictureInfos.ToArray());
        aspect = GameData.aspect;
    }
    public void LoadListPic(PictureInformation[] pictures)
    {
        for (int i = 0; i < pictures.Length; i++)
        {
            //ElementPic elementPic = Instantiate(element, contentPic);
            //elementPic.InitPic(pictures[i]);
            if (!pictures[i].Completed)
                _data.Add(pictures[i]);
            int number = RemoteConfigController.GetIntConfig(FirebaseConfig.NUMBER_PIC_SHOW_NATIVE_ADS, 12);

        }
        //if (elementNativeAds != null && !UseProfile.IsRemoveAds && !UseProfile.IsVip && native == null)
        //{
        //    native = Instantiate(elementNativeAds, contentPic);
        //    native.noNative.GetComponent<ElementPic>().InitPic(null);
        //}
        //GameObject obj = Instantiate(empty, contentPic);
        //yStartContent = contentPic.transform.localPosition.y;
        loadScucces = true;
    }
    float timer;
    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return Mathf.CeilToInt((float)_data.Count / (float)scrollView.NumberCellOfRow);
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        widthContent = transform.GetComponent<RectTransform>().rect.width;
        return (widthContent - 40) / (scrollView.NumberCellOfRow);
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        GridSimulate cellView = scroller.GetCellView(element) as GridSimulate;
        cellView.name = "Cell " + dataIndex;
        cellView.SetData(ref _data, dataIndex * scrollView.NumberCellOfRow);
        return cellView;
    }


    /// <summary>
    /// Handler for when the cell view fires a fixed text button click event
    /// </summary>
    /// <param name="value">value of the text</param>
    private void CellButtonTextClicked(string value)
    {
        Debug.Log("Cell Text Button Clicked! Value = " + value);
    }

    /// <summary>
    /// Handler for when the cell view fires a fixed integer button click event
    /// </summary>
    /// <param name="value">value of the integer</param>
    private void CellButtonFixedIntegerClicked(int value)
    {
        Debug.Log("Cell Fixed Integer Button Clicked! Value = " + value);
    }

    /// <summary>
    /// Handler for when the cell view fires a data integer button click event
    /// </summary>
    /// <param name="value">value of the integer</param>
    private void CellButtonDataIntegerClicked(int value)
    {
        Debug.Log("Cell Data Integer Button Clicked! Value = " + value);
    }
    //private void FixedUpdate()
    //{
    //    if (loadScucces && !loadDone)
    //    {
    //        timer += Time.deltaTime;
    //        if (timer > 1)
    //        {
    //            native.gameObject.SetActive(false);
    //            loadDone = true;
    //        }
    //    }
    //}
    //GameObject preObj;
    //int tempIndex;
    //float showY;

    //public void SwipeScrollChangeNative()
    //{
    //    if (loadDone && NativeAdsManager.Instance.nativeAdLoaded)
    //    {

    //        contentPic.GetComponent<GridLayoutGroup>().enabled = false;
    //        contentPic.GetComponent<ContentSizeFitter>().enabled = false;
    //        //Debug.Log(contentPic.transform.localPosition.y - yStartContent);
    //        int index = 0;
    //        float height = 0;
    //        if (aspect >= 0.68f)
    //        {
    //            height = contentPic.transform.parent.GetComponent<RectTransform>().rect.height * 2f;
    //            index = 15 * (int)((contentPic.transform.localPosition.y) / height);
    //        }
    //        else
    //        {
    //            height = contentPic.transform.parent.GetComponent<RectTransform>().rect.height * 2.5f;
    //            index = 14 * (int)((contentPic.transform.localPosition.y) / height);
    //        }
    //        native.gameObject.SetActive(true);
    //        if (index != 0 && index < contentPic.childCount && index != tempIndex && Mathf.Abs(contentPic.transform.localPosition.y - showY) > height / 2)
    //        {
    //            if (preObj != null)
    //                preObj.SetActive(true);
    //            preObj = contentPic.GetChild(index).gameObject;
    //            preObj.SetActive(false);
    //            native.transform.position = preObj.transform.position;
    //            native.transform.SetSiblingIndex(index);
    //            tempIndex = index;
    //            showY = contentPic.transform.localPosition.y;
    //        }
    //    }
    //}

}
