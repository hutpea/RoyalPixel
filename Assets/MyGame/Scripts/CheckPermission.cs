using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using static CompletePicController;

public class CheckPermission : MonoBehaviour
{
    public UnityAction finishPermission;

    private IEnumerator CheckCameraPermissionThenStart()
    {
#if UNITY_IOS
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam)) {
            Debug.Log("Request webcam permission");
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

        }
        if (Application.HasUserAuthorization(UserAuthorization.WebCam)) {
            Debug.Log("Show camera panel");
            PanelCreateManager.Setup().Show();
        } else {
            GameController.Instance.ShowPanelPermissionSetting(0);
            Debug.Log("don't have camera permission");
        }
#else
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Debug.Log("Permission 1");
            yield return Permission.HasUserAuthorizedPermission(Permission.Camera);
        }

        if (Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Debug.Log("Permission 2");
            if (finishPermission != null)
                finishPermission.Invoke();
        }
        else
        {
            Debug.Log("Permission 3");
            // We do not have permission to use the microphone.
            // Ask for permission or proceed without the functionality enabled.
            Permission.RequestUserPermission(Permission.Camera);
        }
        Debug.Log("Permission 4");
        yield return null;
#endif
    }
    public void RequestPermission()
    {
        StartCoroutine(CheckCameraPermissionThenStart());
    }
}
