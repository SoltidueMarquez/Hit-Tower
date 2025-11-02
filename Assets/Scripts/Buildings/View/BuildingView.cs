using UnityEngine;
using UnityEngine.EventSystems;

namespace Buildings
{
    public class BuildingView : MonoBehaviour, IPointerClickHandler
    {
        private BuildingMono m_BuildingMono;

        private bool m_Initialized = false;
        public void Init(BuildingMono mono)
        {
            m_BuildingMono = mono;
            m_Initialized = true;
        }

        public void Tick()
        {
            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_Initialized)
            {
                m_BuildingMono.buildingLogic.SetDie();
            }
        }

        public void Recycle()
        {
            m_Initialized = false;
        }
    }
}