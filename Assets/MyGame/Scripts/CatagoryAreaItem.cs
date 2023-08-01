using BizzyBeeGames.ColorByNumbers;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatagoryAreaItem : MonoBehaviour, IEnhancedScrollerDelegate
{
    public GridSimulateArena areasElement;
    public Transform contentAreas;
    public int ID;
    private SmallList<CategoryInfo> _data = new SmallList<CategoryInfo>();
    public EnhancedScroller scroller;
    [SerializeField] UIScrollView scrollView;
    float widthContent;
    [SerializeField] GameObject empty;
    void Start()
    {
        scroller.Delegate = this;
    }
    public void InitArea(LoadDataPic loadData, int id)
    {
        this.ID = id;
        foreach (CategoryInfo item in loadData.CategoryInfos.Values)
        {
            //AreasElement areasElement = Instantiate(Resources.Load("Home/AreasElement") as GameObject, contentAreas).GetComponent<AreasElement>();
            item.PictureInfos = new List<PictureInformation>();
            for (int j = 0; j < item.pictureFiles.Count; j++)
            {
                PictureInformation pictureInfo = new PictureInformation(item.pictureFiles[j].text);
                pictureInfo.IdCateAre = id;
                item.PictureInfos.Add(pictureInfo);
            }
            PictureInformation[] pictureInformation = item.PictureInfos.ToArray();
            int idAreas = int.Parse(item.displayName);
            _data.Add(item);
            //areasElement.InitAreas(item, id);
            bool check = false;
            if (!PlayerPrefs.HasKey(StringConstants.KEY.SAVE_INPROGRESS))
            {
                for (int i = 0; i < pictureInformation.Length; i++)
                {
                    if (pictureInformation[i].TotalPixelPainted != 0)
                    {
                        check = true;
                    }
                }
                if (check)
                {
                    HomeController.instance.progress += idAreas + ",";
                }
            }
            if (GameData.CompleteArea)
            {
                if (!HomeController.instance.startView)
                {
                    HomeController.instance.startView = true;
                    HomeController.instance.idView = GameData.PaintingAreas;
                }
                if (idAreas == HomeController.instance.idView)
                {
                    //areasElement.ViewPic();
                    //HomeController.instance.SetActiveTab(1);
                    GameData.CompleteArea = false;

                }
                //GameData.PaintingAreas = GetKey();
            }
            else
            {
                //HomeController.instance.SetActiveTab(2);
            }
        }
    }
    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return Mathf.CeilToInt((float)_data.Count / (float)scrollView.NumberCellOfRow);
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        widthContent = transform.GetComponent<RectTransform>().rect.width;
        return (widthContent - 40f) / (scrollView.NumberCellOfRow);
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        GridSimulateArena cellView = scroller.GetCellView(areasElement) as GridSimulateArena;
        cellView.name = "Cell " + dataIndex;
        cellView.SetData(ref _data, dataIndex * scrollView.NumberCellOfRow, this.ID);
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
}
