using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabSelectCate : MonoBehaviour
{
    public Text txtNameCate;
    Color32 color;
    public int page;
    [SerializeField] GameObject iconNew;
    public void InitPage(int page, string name)
    {
        this.page = page;
        txtNameCate.text = name.ToUpper();
        //if (txtNameCate.text =="CUTE" && GameData.NewUpdate)
        //{
        //    iconNew.SetActive(true);
        //}
    }
    private void Start()
    {
        color = new Color32(123, 122, 121, 255);
    }
    public void SelectCate()
    {
        if (DiscoveryController.instance.currentSelect != null)
            DiscoveryController.instance.currentSelect.UnSelect();
        txtNameCate.color = new Color32(253, 180, 100, 255);
        DiscoveryController.instance.selectTab.DOMoveX(txtNameCate.transform.position.x, 0.2f).OnComplete(() => { DiscoveryController.instance.SetPos(page); DiscoveryController.instance.selectTab.SetParent(transform); });
        DiscoveryController.instance.currentSelect = this;
        DiscoveryController.instance.horizontalScroll.ChangePage(page);
        DiscoveryController.instance.ActiveCategory(page - 1, page, page + 1);
    }
    public void UnSelect()
    {
        txtNameCate.color = color;
    }
}
