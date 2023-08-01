using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEventController : MonoBehaviour
{
    [SerializeField] Image bg;
    private void Start()
    {
        Capture();
    }
    public void Capture()
    {
        ScreenCapture.CaptureScreenshot("screenshot.png", 4);
    }
}
