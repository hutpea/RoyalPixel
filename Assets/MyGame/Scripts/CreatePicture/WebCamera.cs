using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebCamera : MonoBehaviour
{
    public int currentCamindex = 0;
    public WebCamTexture tex;
    public RawImage display;
    PanelCreateManager createManager;
    public Texture2D t, texStop;
    private Queue<Texture2D> textureQueue = new Queue<Texture2D>();
    public bool stop;
    int orient;
    public Quaternion baseRotation;
    private void OnEnable()
    {
        while (textureQueue.Count > 0)
        {
            Destroy(textureQueue.Dequeue());
        }
        currentCamindex = 1;
        createManager = transform.parent.GetComponent<PanelCreateManager>();
        StartStopCam();
        stop = false;
        baseRotation = display.transform.rotation;
        SetRotaCam();
        createManager.slider.value = (createManager.slider.minValue + createManager.slider.maxValue) / 2f;
    }
    public void SwapCam_click()
    {
        if (WebCamTexture.devices.Length > 0)
        {
            if (currentCamindex == 0)
            {
                currentCamindex = 1;
            }
            else
            {
                currentCamindex = 0;
            }
            SetRotaCam();
            if (tex != null)
            {
                StopCam();
                StartStopCam();
            }
        }
    }
    void SetRotaCam()
    {
#if UNITY_ANDROID
        if (currentCamindex > 0)
        {
            display.transform.parent.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            display.uvRect = new Rect(0, 0, 1, -1);
        }
        else
        {
            display.uvRect = new Rect(0, 0, 1, 1);
            display.transform.parent.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0, 0, -90));
        }

#else
         display.transform.parent.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0, 0, -90));
         if (currentCamindex > 0)
        {
            display.uvRect = new Rect(0, 0, 1, -1);
        }
        else
        {
            display.uvRect = new Rect(0, 0, 1, 1);
        }
#endif
    }
    public void StopCam()
    {
        display.texture = null;
        if (tex != null)
        {
            tex.Stop();
            Destroy(tex);
            tex = null;
        }
        stop = false;
        texStop = null;
        if (currentCamindex == 0)
            GameData.typeCreate = "back_camera";
        else
            GameData.typeCreate = "front_camera";
    }
    public void RetakePic()
    {
        createManager.slider.value = (createManager.slider.minValue + createManager.slider.maxValue) / 2f;
        if (createManager.selectPicGallery)
        {
            //StartStopCam();
            createManager.selectPicGallery = false;
        }
        else
        {
            StopCam();
            StartStopCam();
        }
    }
    public void StartStopCam()
    {
        createManager.webCam = true;
        if (tex != null)
        {
            StopCam();
        }
        else
        {
            WebCamDevice device = WebCamTexture.devices[currentCamindex];
            tex = new WebCamTexture(device.name);
            display.texture = tex;
            tex.Play();
        }
        createManager.apply.SetActive(false);
        createManager.selfie.SetActive(true);
    }
    private void Update()
    {
        if (stop || tex == null)
            return;
        int num = Mathf.Min(tex.width, tex.height);
        t = new Texture2D(num, num);
        if (textureQueue.Count > 10)
        {
            Destroy(textureQueue.Dequeue());
        }
        textureQueue.Enqueue(t);
        t.SetPixels(tex.GetPixels((tex.width - num) / 2, (tex.height - num) / 2, num, num));
        t.Apply();
        texStop = t;
        createManager.UpdateColorCameraVirtual(t);
    }

    public void Stop()
    {
        if (tex != null)
        {
            display.texture = t;
            tex.Stop();
            stop = true;
            createManager.selfie.SetActive(false);
            createManager.apply.SetActive(true);
        }
        while (textureQueue.Count > 0)
        {
            Texture2D texture = textureQueue.Dequeue();
            if (texture != t)
            {
                Destroy(texture);
            }
        }
    }
    private void OnDisable()
    {
        StopCam();
    }
    public Texture2D Rotate(Texture2D originalTexture, bool clockwise)
    {
        Color32[] pixels = originalTexture.GetPixels32();
        Color32[] array = new Color32[pixels.Length];
        Color32[] arrayFlip = new Color32[pixels.Length];
        int width = originalTexture.width;
        int height = originalTexture.height;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int num = /*(clockwise) ? (*/(j + 1) * height - i - 1/*) : (i * height + j)*/;
                int num2 = (!clockwise) ? (i * width + j) : (pixels.Length - 1 - (i * width + j));
                array[num] = pixels[num2];
            }
        }
        Texture2D texture2D = new Texture2D(height, width);
        texture2D.SetPixels32(array);

#if UNITY_ANDROID
        if (!clockwise)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int num = i * height + j;
                    int num2 = (i + 1) * height - j - 1;
                    arrayFlip[num] = array[num2];
                    texture2D.SetPixels32(arrayFlip);
                }
            }
        }

#else
        if(currentCamindex>0)
        {
        for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int num = i * height + j;
                    int num2 = (i + 1) * height - j - 1;
                    arrayFlip[num] = array[num2];
                    texture2D.SetPixels32(arrayFlip);
                }
            }
        }
#endif
        texture2D.Apply();
        return texture2D;
    }
}
