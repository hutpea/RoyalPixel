using System.Collections;
using System.Collections.Generic;
using BizzyBeeGames.ColorByNumbers;
using UnityEngine;

public class EventFarmBuildingController : MonoBehaviour
{
    public void LoadFilePic()
    {
        foreach (var eleCate in GameController.Instance.dataEventFarm.CategoryInfos.Values)
        {
            eleCate.PictureInfos = new List<PictureInformation>();
            for (int j = 0; j < eleCate.pictureFiles.Count; j++)
            {
                Debug.Log("Load a picture info");
                PictureInformation pictureInfo = new PictureInformation(eleCate.pictureFiles[j].text);
                eleCate.PictureInfos.Add(pictureInfo);
            }
        }
    }
}
