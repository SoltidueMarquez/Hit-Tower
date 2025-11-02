using UnityEngine;
using UnityEngine.EventSystems;

namespace Buildings
{
    public class PlacementCell : MonoBehaviour, IPointerClickHandler
    {
        private BuildingMono m_Building;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_Building == null)
            {
                GameManager.Instance.buildingManager.TryBuild(transform, out var mono);
                m_Building = mono;
                if(mono!=null) mono.buildingLogic.OnDie += ClearBuilding;
                Debug.Log($"ClickPos:{transform.position}");
            }
            else
            {
                Debug.LogWarning("已有建筑物");
            }
        }

        public void ClearBuilding()
        {
            m_Building = null;
        }
    }
}