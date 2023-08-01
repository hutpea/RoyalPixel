using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpin : MonoBehaviour
{
    [SerializeField] private Item itemName;
    [SerializeField] private Text txtAmount;
    [SerializeField] private int _id;
    public int amount;

    public void SetView(int amount)
    {
        txtAmount.text = amount.ToString();
    }

    public Item ItemName
    {
        get
        {
            return itemName;
        }
    }

    public int Id
    {
        get
        {
            return _id;
        }
    }
}
