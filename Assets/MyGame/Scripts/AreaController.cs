using BizzyBeeGames.ColorByNumbers;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class AreaController : MonoBehaviour
{
    public TabSelectCateArea tabCategory;
    public Transform contentTab;
    public Transform selectTab;
    [SerializeField] CatagoryAreaItem category;
    [SerializeField] Transform contenCate;
    public static AreaController instance;
    public ScrollRect parent;
    public TabSelectCateArea currentSelect;
    public HorizontalScrollSnap horizontalScroll;
    Vector2 startPosContentTab;
    bool onDrag;
    public List<CatagoryAreaItem> categoryItems;
    float[] posTab;
    public DataArena dataArena;
    
    #region FARM_EVENT_VARIABLE
    public Image treasureFillImg;
    #endregion
    
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        startPosContentTab = contentTab.transform.position;
        UpdateBannerTreasureFillImg();
    }

    public void UpdateBannerTreasureFillImg()
    {
        Debug.Log(GameData.FarmTreasureFillAmount / 4f);
        treasureFillImg.fillAmount = Mathf.Min(GameData.FarmTreasureFillAmount / 4f, 0.25f);
    }
    public void LoadFilePic()
    {
        categoryItems = new List<CatagoryAreaItem>();
        int page = 0;
        posTab = new float[GameController.Instance.dataPic.CategoryInfos.Values.Count];
        foreach (var item in GameController.Instance.dataAreas.cateItems)
        {
            TabSelectCateArea obj = Instantiate(tabCategory, contentTab);
            if (currentSelect == null)
            {
                currentSelect = obj;
            }
            obj.InitPage(page, item.name);
            CatagoryAreaItem cateItem = Instantiate(category, contenCate);
            cateItem.InitArea(item.dataPic, page);
            categoryItems.Add(cateItem);
            page++;
        }
        if (currentSelect.page == 0)
        {
            ActiveCategory(-1, 0, 1);
        }
        for (int i = 0; i < contentTab.childCount; i++)
        {
            if (i == contentTab.childCount - 1)
            {
                posTab[i] = contentTab.transform.localPosition.x - (float)i * 50 - 60;
            }
            else
                posTab[i] = contentTab.transform.localPosition.x - (float)i * 50;
        }
        selectTab.SetParent(currentSelect.transform);
        currentSelect.txtNameCate.color = new Color32(253, 180, 100, 255);
        selectTab.transform.localPosition = new Vector3(currentSelect.transform.position.x, -23, selectTab.transform.localPosition.z);
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
                contentTab.GetChild(currentSelect.page + 1).GetComponent<TabSelectCateArea>().SelectCate();
            }
        }
        else
        {
            if (currentSelect.page > 0)
            {
                contentTab.GetChild(currentSelect.page - 1).GetComponent<TabSelectCateArea>().SelectCate();
            }
        }
        ActiveCategory(currentSelect.page - 1, currentSelect.page, currentSelect.page + 1);
    }
    public void SetPos(int page)
    {
        //contentTab.DOLocalMoveX(posTab[page], 0.2f);
    }
}
