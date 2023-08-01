using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabSelectCateArea : MonoBehaviour
{
    public Text txtNameCate;
    Color32 color;
    public int page;
    [SerializeField] GameObject iconNew;
    public void InitPage(int page, string name)
    {
        this.page = page;
        txtNameCate.text = name.ToUpper();
        if ((name == "JIGSAW" || name == "SPECIAL") && GameData.NewUpdate)
        {
            iconNew.SetActive(true);
        }
    }
    private void Start()
    {
        color = new Color32(123, 122, 121, 255);
    }
    public void SelectCate()
    {
        if (AreaController.instance.currentSelect != null)
            AreaController.instance.currentSelect.UnSelect();
        txtNameCate.color = new Color32(253, 180, 100, 255);
        AreaController.instance.selectTab.DOMoveX(txtNameCate.transform.position.x, 0.2f).OnComplete(() => { AreaController.instance.SetPos(page); AreaController.instance.selectTab.SetParent(transform); });
        AreaController.instance.currentSelect = this;
        AreaController.instance.horizontalScroll.ChangePage(page);
        AreaController.instance.ActiveCategory(page - 1, page, page + 1);
    }
    public void UnSelect()
    {
        txtNameCate.color = color;
    }
}
