using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataGift", order = 1)]
public class DataGift : ScriptableObject
{
    public List<GiftItem> gifts = new List<GiftItem>();
}
[System.Serializable]
public class GiftItem
{
    public int itemBomb;
    public int itemStar;
    public int itemFind;
    public int itemPen;
}
