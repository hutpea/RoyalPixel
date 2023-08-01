using BizzyBeeGames.ColorByNumbers;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

public class GridSimulateArena : EnhancedScrollerCellView
{
    public AreasElement[] rowCellViews;
    public AreasElement element;
    public int NumberCellOfRow;
    bool init;
    /// <summary>
    /// This function just takes the Demo data and displays it
    /// </summary>
    /// <param name="data"></param>
    public void SetData(ref SmallList<CategoryInfo> data, int startingIndex, int ID)
    {
        if (!init)
        {
            float aspect = GameData.aspect;
            if (aspect >= 0.68f)
            {
                NumberCellOfRow = 3;
            }
            else
            {
                NumberCellOfRow = 2;
            }
            rowCellViews = new AreasElement[NumberCellOfRow];
            for (int i = 0; i < NumberCellOfRow; i++)
            {
                rowCellViews[i] = Instantiate(element, transform);
            }
            init = true;
        }
        foreach (var rTmp in rowCellViews)
        {
            rTmp.gameObject.SetActive(false);
        }

        // loop through the sub cells to display their data (or disable them if they are outside the bounds of the data)
        for (var i = 0; i < rowCellViews.Length; i++)
        {
            // if the sub cell is outside the bounds of the data, we pass null to the sub cell
            rowCellViews[i].gameObject.SetActive(true);
            rowCellViews[i].InitAreas(startingIndex + i < data.Count ? data[startingIndex + i] : null, ID);
        }
    }
}