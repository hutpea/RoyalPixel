using BizzyBeeGames.ColorByNumbers;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataCate", menuName = "PixelArt/DataArea")]
public class LoadDataCate : ScriptableObject
{
    public List<CateItem> cateItems = new List<CateItem>();
}
[System.Serializable]
public class CateItem
{
    public string name;
    public LoadDataPic dataPic;
}
