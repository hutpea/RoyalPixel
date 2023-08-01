using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "DataPictureJigsaw")]
public class DataPictureJigsaw : ScriptableObject
{
    public List<PicturePice> picturePices;
    public static int Current
    {
        get
        {
            return PlayerPrefs.GetInt("current", 0);
        }
        set
        {
            PlayerPrefs.SetInt("current", value);
        }
    }
}
[System.Serializable]
public class PicturePice
{
    public List<OncePice> oncePices;
    public int ID;
    public Sprite sprite;
    public string name;
    public GameObject anim;

    public bool Completed
    {
        get
        {
            return PlayerPrefs.GetInt("pice_complete_" + ID) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("pice_complete_" + ID, value ? 1 : 0);
        }
    }
    public bool Unlock
    {
        get
        {
            if (ID == 0)
                return true;
            return PlayerPrefs.GetInt("pice_unlock_" + ID) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("pice_unlock_" + ID, value ? 1 : 0);
        }
    }

}
[System.Serializable]
public class OncePice
{
    public TextAsset textAsset;
    public int ID;
}

