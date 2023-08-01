using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupPermission : BaseBox {
    private static PopupPermission instance;
    [SerializeField] GameObject txtCam;
    [SerializeField] GameObject txtPhoto;
    public static PopupPermission Setup(int typePermission) {
        if (instance == null) {
            instance = Instantiate(Resources.Load<PopupPermission>(PathPrefabs.POPUP_PERMISSION));
        }
        instance.Init(typePermission);
        return instance;
    }
    private void Init(int typePermission) {
        if (typePermission == 0) {
            txtPhoto.SetActive(false);
            txtCam.SetActive(true);
        } else {
            txtPhoto.SetActive(true);
            txtCam.SetActive(false);
        }
    }
    public void OpenSetting()
    {
        NativeGallery.OpenSettings();
    }
}
