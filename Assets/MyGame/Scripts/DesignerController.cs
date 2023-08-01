using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesignerController : MonoBehaviour
{
    [SerializeField] GameObject panelDraw;
    [SerializeField] GameObject panelImport;
    public static DesignerController instance;
    private void OnEnable()
    {
        instance = this;
    }
    public void OpenDraw()
    {
        GameController.Instance.admobAds.ShowInterstitial(false, "open_draw", () =>
        {
            GameData.isDrawPixel = true;
            panelDraw.SetActive(true);
            panelImport.SetActive(false);
        });
    }
    public void OpenImport()
    {
        GameController.Instance.admobAds.ShowInterstitial(false, "open_import", () =>
        {
            GameData.isDrawPixel = false;
            panelImport.SetActive(true);
            panelDraw.SetActive(false);
        });
    }
    public void Back()
    {
        panelDraw.SetActive(false);
        panelImport.SetActive(false);
        GameData.isDrawPixel = false;
    }
}
