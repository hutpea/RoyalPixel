using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupRename : BaseBox
{
    private static PopupRename instance;
    [SerializeField] InputField inputField;
    public static PopupRename Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<PopupRename>(PathPrefabs.POPUP_RENAME));
        }
        //ChickenDataManager.CountTillShowRate = 0;
        return instance;
    }
    public void SaveName()
    {
        if (inputField.text.Length <= 3)
        {
            GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp(inputField.transform.position, "Enter a name with more than 3 characters", Color.red);
        }
        else
        {
            MyProfileController.instance.txtName.text = inputField.text;
            Close();
            GameData.UserName = inputField.text;
        }
    }

}
