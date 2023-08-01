using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

public class ScrollArea : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    // Start is called before the first frame update
    public HorizontalScrollSnap horizontalScrollSnap;
    Vector2 startPos;
    void Start()
    {
        horizontalScrollSnap = AreaController.instance.parent.gameObject.GetComponent<HorizontalScrollSnap>();
    }
    public void OnBeginDrag(PointerEventData data)
    {
        startPos = data.position;
    }
    public void OnDrag(PointerEventData data)
    {
        //imgSnap.SetActive(false);
        horizontalScrollSnap.enabled = false;
    }
    public void OnEndDrag(PointerEventData data)
    {
        //Debug.Log("dd1" + Mathf.Abs(data.position.y - startPos.y));
        horizontalScrollSnap.enabled = true;
        if (data.position.x < startPos.x && Mathf.Abs(data.position.y - startPos.y) < 100f)
        {
            horizontalScrollSnap.NextScreen();
            AreaController.instance.EndDragSelectTap(true);
        }
        else if (data.position.x > startPos.x && Mathf.Abs(data.position.y - startPos.y) < 100f)
        {
            horizontalScrollSnap.PreviousScreen();
            AreaController.instance.EndDragSelectTap(false);
        }
    }
}
