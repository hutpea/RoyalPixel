using BizzyBeeGames.ColorByNumbers;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataPicture.asset", menuName = "PixelArt/DataPicture")]
public class LoadDataPic : ScriptableObject
{
    [SerializeField] private List<CategoryInfo> categoryInfos;
    private Dictionary<string, CategoryInfo> tempCate = new Dictionary<string, CategoryInfo>();
    public Dictionary<string, CategoryInfo> CategoryInfos
    {
        get
        {
            for (int i = 0; i < categoryInfos.Count; i++)
            {
                string key = categoryInfos[i].displayName;
                if (!tempCate.ContainsKey(key))
                {
                    tempCate.Add(key, categoryInfos[i]);
                }
            }
            return tempCate;

        }
    }
}
