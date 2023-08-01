using BizzyBeeGames.ColorByNumbers;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ScrollController : MonoBehaviour, IEnhancedScrollerDelegate {
    private SmallList<ElementPic> _data = new SmallList<ElementPic>();
    public EnhancedScroller scroller;
    public ElementPic elementPrefab;
    private int _totalStartPosition;
    // Start is called before the first frame update
    void Awake() {
        scroller.Delegate = this;
    }

    public void InitState(ElementPic[] dataPictures) {
        _totalStartPosition = 0;
        _data.Clear();
        for (int i = 0; i < dataPictures.Length; i++) {
            int index = i;
            _data.Add(dataPictures[index]);
        }
        GetDataScroll();
    }
    public void GetDataScroll(int startPosition = 0) {
        var startIndex = scroller.NumberOfCells > 0
           ? scroller.StartDataIndex
           : 0;

        scroller.ReloadData();
        if (_data.Count >= 4) {
            scroller.JumpToDataIndex(startIndex, 0f);
        } else {
            scroller.cellViewVisibilityChanged = null;
        }
    }


    public int GetNumberOfCells(EnhancedScroller rankingScroller) {
        return (int)Math.Ceiling((double)_data.Count / 2);
    }

    public virtual float GetCellViewSize(EnhancedScroller rankingScroller, int dataIndex) {
        float aspectCam = GameData.aspect;
        //Debug.Log("aspectCam" + aspectCam);
        if (aspectCam < 0.68f)
            return 260/* * (aspectCam / (750f / 1334f))*/;
        else
            return 210;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
        var cellView = scroller.GetCellView(elementPrefab);
        //cellView.gameObject.GetComponent<ElementDouble>().id = 
        //Assert.IsTrue(cellView != null, "rankingCell is null");
        //cellView.transform.GetChild(0).GetComponent<ElementLibrary>().InitElement(_data[dataIndex * 2]);
        //if (dataIndex * 2 + 1 < _data.Count) {
        //    cellView.transform.GetChild(1).GetComponent<Button>().interactable = true;
        //    cellView.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        //    cellView.transform.GetChild(1).GetComponent<ElementLibrary>().InitElement(_data[dataIndex * 2 + 1]);
        //}
        //if (_data.Count / 2 != 0 && dataIndex * 2 + 1 == _data.Count) {
        //    cellView.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
        //    cellView.transform.GetChild(1).GetComponent<Button>().interactable = false;
        //}
        return cellView;
    }
}
