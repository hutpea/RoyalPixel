using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Handler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerUpHandler, IPointerEnterHandler
     , IPointerExitHandler
{
    public MainCamera mainCam;

    public void OnBeginDrag(PointerEventData eventData)
    {
        mainCam.OnBeginDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        mainCam.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        mainCam.OnEndDrag();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mainCam.OnPointerUp(eventData);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
    }
}
