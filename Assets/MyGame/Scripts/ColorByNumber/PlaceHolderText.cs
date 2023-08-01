using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceHolderText : MonoBehaviour {
    public string perTime;
    public void SetValue(string value) {
        GetComponent<Text>().text = string.Format("{0} " + perTime, value);
    }
}
