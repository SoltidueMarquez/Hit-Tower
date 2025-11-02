using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Buildings
{
    public class BuildingView : MonoBehaviour, IPointerClickHandler
    {
        [LabelText("攻击范围指示器"), SerializeField] protected Transform rangeIndicator;
        protected BuildingMono m_BuildingMono;

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
                OnPointerClick();
            }
        }
        
        protected virtual void OnPointerClick()
        {
            // TODO:测试部分，默认升级，升到满就拆除建筑
            if (m_BuildingMono.buildingLogic.buildingInfo.CheckIfMaxLv())
            {
                RecycleBuilding();
            }
            else
            {
                UpgradeBuilding();
            }
        }

        protected void UpgradeBuilding()
        {
            m_BuildingMono.UpGrade();
        }
        
        protected void RecycleBuilding()
        {
            m_BuildingMono.buildingLogic.SetDie();
        }

        /// <summary>
        /// 这是脚本的回收逻辑
        /// </summary>
        public void Recycle()
        {
            m_Initialized = false;
        }
        
    }
}