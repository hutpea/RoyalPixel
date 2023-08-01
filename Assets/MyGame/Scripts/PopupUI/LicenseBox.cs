using UnityEngine;

public class LicenseBox : BaseBox
{
    private static LicenseBox instance;

    public static LicenseBox Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<LicenseBox>(PathPrefabs.POPUP_LICENSE_BOX));
        }
        instance.Show();
        return instance;
    }
}