using BizzyBeeGames.ColorByNumbers;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class DiscoveryController : MonoBehaviour
{
    public TabSelectCate tabCategory;
    public Transform contentTab;
    public Transform selectTab;
    [SerializeField] CategoryItem category;
    [SerializeField] Transform contenCate;
    public static DiscoveryController instance;
    public ScrollRect parent;
    public TabSelectCate currentSelect;
    public List<PictureInformation> newPics;
    public HorizontalScrollSnap horizontalScroll;
    Vector2 startPosContentTab;
    bool onDrag;
    public List<CategoryItem> categoryItems;
    float[] posTab;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
       
        startPosContentTab = contentTab.transform.position;
    }
    public void LoadFilePic()
    {
        newPics = new List<PictureInformation>();
        categoryItems = new List<CategoryItem>();
        int page = 0;
        posTab = new float[GameController.Instance.dataPic.CategoryInfos.Values.Count];
        foreach (var eleCate in GameController.Instance.dataPic.CategoryInfos.Values)
        {
            CategoryInfo categoryInfo = eleCate;
            //Debug.Log(eleCate.displayName + "|" + eleCate.Id);
            categoryInfo.PictureInfos = new List<PictureInformation>();
            for (int j = 0; j < categoryInfo.pictureFiles.Count; j++)
            {
                PictureInformation pictureInfo = new PictureInformation(categoryInfo.pictureFiles[j].text);
                if (categoryInfo.displayName == "NEW")
                {
                    pictureInfo.IsNew = true;
                }
                else
                {
                    categoryInfo.Id = pictureInfo.IdArea;
                    pictureInfo.IsNew = false;
                }
                categoryInfo.PictureInfos.Add(pictureInfo);
                if (!PlayerPrefs.HasKey(StringConstants.KEY.SAVE_INPROGRESS))
                    if (pictureInfo.TotalPixelPainted != 0)
                    {
                        HomeController.instance.progress += pictureInfo.Id + ",";
                    }
            }
            TabSelectCate obj = Instantiate(tabCategory, contentTab);
            if (currentSelect == null)
            {
                currentSelect = obj;
            }
            obj.InitPage(page, categoryInfo.displayName);
            page++;
            CategoryItem cateItem = Instantiate(category, contenCate);
            cateItem.InitCategory(categoryInfo);
            categoryItems.Add(cateItem);
            //int idAreas = int.Parse(categoryInfo.displayName);
        }
        if (currentSelect.page == 0)
        {
            ActiveCategory(-1, 0, 1);
        }
        for (int i = 0; i < contentTab.childCount; i++)
        {
            if (i == contentTab.childCount - 1)
            {
                if (i < GameController.Instance.dataPic.CategoryInfos.Values.Count)
                {
                    posTab[i] = contentTab.transform.localPosition.x - (float)i * 50 - 60;
                }
            }
            else
            {
                if (i < GameController.Instance.dataPic.CategoryInfos.Values.Count)
                {
                    posTab[i] = contentTab.transform.localPosition.x - (float)i * 50;
                }
            }
        }
        selectTab.SetParent(currentSelect.transform);
        currentSelect.txtNameCate.color = new Color32(253, 180, 100, 255);
        selectTab.transform.localPosition = new Vector3(currentSelect.transform.position.x, -23, selectTab.transform.localPosition.z);
        //CategoryInfo newCate = new CategoryInfo();
        //CategoryItem newItem = Instantiate(category, contenCate);
        //newItem.InitCategory(newCate);
        //newItem.transform.SetAsFirstSibling();
    }
    public void ActiveCategory(int pre, int current, int next)
    {
        for (int i = 0; i < categoryItems.Count; i++)
        {
            if (i == pre || i == next || i == current)
            {
                categoryItems[i].gameObject.SetActive(true);
            }
            else
                categoryItems[i].gameObject.SetActive(false);
        }
    }
    public void EndDragSelectTap(bool next)
    {
        if (next)
        {
            if (currentSelect.page < contentTab.childCount - 1)
            {
                contentTab.GetChild(currentSelect.page + 1).GetComponent<TabSelectCate>().SelectCate();
            }
        }
        else
        {
            if (currentSelect.page > 0)
            {
                contentTab.GetChild(currentSelect.page - 1).GetComponent<TabSelectCate>().SelectCate();
            }
        }
        ActiveCategory(currentSelect.page - 1, currentSelect.page, currentSelect.page + 1);
    }
    public void SetPos(int page)
    {
        contentTab.DOLocalMoveX(posTab[page], 0.2f);
    }
}
