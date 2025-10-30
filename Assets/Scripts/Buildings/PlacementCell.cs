using UnityEngine;
using UnityEngine.EventSystems;

namespace Buildings
{
    public class PlacementCell : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"ClickPos:{transform.position}");
        }
    }
}