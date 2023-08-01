using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragPic : MonoBehaviour, IDragHandler
{
    public PanelCreateManager createManager;
    float deltaMagnitudeDiff;
    float scale = 1;
    public ScrollRect scrollRect;
    void OnEnable()
    {
   
    }
    // Start is called before the first frame update
    public void OnDrag(PointerEventData eventData)
    {
        if (Input.touchCount >= 2)
            return;
        //Debug.LogError("update");
        createManager.UpdateTexture();
    }
    void Update()
    {
        if (createManager.imageTexture == null)
            return;

        if (Input.touchCount == 2)
        {
            // Store both touches.
            scrollRect.horizontal = false;
            scrollRect.vertical = false;
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
            deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            //Debug.Log("deltaMagnitudeDiff " + deltaMagnitudeDiff);
            if (Mathf.Abs(deltaMagnitudeDiff) < 1f)
                return;
            Vector2 centerBoard = new Vector2(-createManager.image.transform.localPosition.x, -createManager.image.transform.localPosition.y) + Vector2.one * 150;
            Vector2 pivot = new Vector2(centerBoard.x / (createManager.imageTexture.width * createManager.scale), centerBoard.y / (createManager.imageTexture.height * createManager.scale));
            SetPivot(createManager.image.GetComponent<RectTransform>(), pivot);
            scale += deltaMagnitudeDiff < 0 ? 0.1f : -0.1f;
            scale = Mathf.Clamp(scale, 1, 3);
            createManager.scale = scale;
            createManager.image.transform.localScale = new Vector3(createManager.scale, createManager.scale, 0);
            SetPivot(createManager.image.GetComponent<RectTransform>(), Vector2.zero);
            Timer.Schedule(this, 0f, createManager.UpdateTexture);
        }
        if (Input.touchCount < 2)
        {
            scrollRect.horizontal = true;
            scrollRect.vertical = true;
        }
    }
    private void SetPivot(RectTransform rectTransform, Vector2 pivot)
    {
        if (rectTransform == null) return;
        Vector2 size = rectTransform.rect.size * createManager.scale;
        Vector2 deltaPivot = rectTransform.pivot - pivot;
        Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
        rectTransform.pivot = pivot;
        rectTransform.localPosition -= deltaPosition;
    }

}
