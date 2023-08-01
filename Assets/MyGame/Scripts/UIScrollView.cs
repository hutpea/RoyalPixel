using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScrollView : MonoBehaviour
{
    // Start is called before the first frame update
    public GridLayoutGroup gridLayout;
    public Vector2 cellSize;
    public int NumberCellOfRow;
    void Awake()
    {
        float aspect = GameData.aspect;
        if (aspect >= 0.68f)
        {
            if (gridLayout != null)
            {
                float widthContent = gridLayout.GetComponent<RectTransform>().rect.width;
                gridLayout.cellSize = new Vector2((widthContent - 10 * 4f) / 3f, (widthContent - 10 * 4f) / 3f);
                cellSize = gridLayout.cellSize;
                NumberCellOfRow = 3;
            }
        }
        else
        {
            if (gridLayout != null)
            {
                float widthContent = gridLayout.GetComponent<RectTransform>().rect.width;
                gridLayout.cellSize = new Vector2((widthContent - 10 * 3f) / 2f, (widthContent - 10 * 3f) / 2f);
                cellSize = gridLayout.cellSize;
                NumberCellOfRow = 2;
            }
        }
    }

}
