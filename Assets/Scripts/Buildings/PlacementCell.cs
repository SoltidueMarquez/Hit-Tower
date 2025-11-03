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
                GameManager.Instance.buildingManager.buildingViewModelHelper.OnCellClicked(this);
            }
            else
            {
                Debug.LogWarning("已有建筑物");
            }
        }

        public void Build(string buildingName)
        {
            GameManager.Instance.buildingManager.TryBuild(buildingName, transform, out var mono);
            m_Building = mono;
            if (mono != null) mono.buildingLogic.OnDie += ClearBuilding;
        }

        public void ClearBuilding()
        {
            m_Building = null;
        }
    }
}