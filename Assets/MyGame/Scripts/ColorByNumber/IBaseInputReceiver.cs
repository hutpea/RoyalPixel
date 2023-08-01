using UnityEngine.EventSystems;

internal interface IBaseInputReceiver : IPointerDownHandler, IPointerUpHandler, IDragHandler, IScrollHandler, IEventSystemHandler
{
}