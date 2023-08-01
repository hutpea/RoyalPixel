using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PictureEventJigsaw : MonoBehaviour
{
    public Image imgBg;
    [SerializeField] Transform content;
    [SerializeField] ElementPicEventJigsaw[] eventJigsaws;
    [SerializeField] GameObject objLock;
    public Text txtProgress;
    int ID;
    public Text txtName;
    PicturePice pice;
    void OnEnable()
    {
        if (pice != null)
        {
            InitJigsaw(pice);
        }
    }
    public void InitJigsaw(PicturePice picturePice)
    {
        int index = 0;
        this.ID = picturePice.ID;
        imgBg.sprite = picturePice.sprite;
        bool completed = true;
        pice = picturePice;
        //foreach (Transform child in content)
        //    Destroy(child.gameObject);
        foreach (OncePice oncePice in picturePice.oncePices)
        {
            if (DataPictureJigsaw.Current <= oncePice.ID)
            {
                completed = false;

            }
            if (DataPictureJigsaw.Current >= oncePice.ID)
            {
                picturePice.Unlock = true;
            }
        }
        picturePice.Completed = completed;
        int i = 0;
        if (picturePice.Unlock)
        {
            foreach (OncePice oncePice in picturePice.oncePices)
            {
                if (oncePice.ID < DataPictureJigsaw.Current)
                    index++;
                eventJigsaws[i].InitPicture(oncePice);
                i++;
            }
            objLock.SetActive(false);
        }
        else
        {
            objLock.SetActive(true);
        }
        txtProgress.text = index + "/" + picturePice.oncePices.Count;
        txtName.text = picturePice.name;
        if (completed)
            if (picturePice.anim != null)
            {
                Instantiate(picturePice.anim, imgBg.transform);
            }
    }
}
