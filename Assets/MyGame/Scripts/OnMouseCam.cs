using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMouseCam : MonoBehaviour
{
    public MainCamera mainCamera;
    private void OnMouseUpAsButton()
    {
        mainCamera.TapPixel(Input.mousePosition);
    }
}
