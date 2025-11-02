using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Buildings
{
    public class BuildingView : MonoBehaviour, IPointerClickHandler
    {
        [LabelText("攻击范围指示器"), SerializeField] protected SpriteRenderer rangeIndicator;
        protected BuildingMono m_BuildingMono;

        private bool m_Initialized = false;
        public void Init(BuildingMono mono)
        {
            m_BuildingMono = mono;
            m_Initialized = true;

            UpdateRangeIndicator();
            // 订阅范围更新事件
            m_BuildingMono.buildingLogic.buildingInfo.attackRange.OnValueChanged += UpdateRangeIndicator;
        }

        // public void Tick()
        // {
        //     
        // }

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
        
        public void UpdateRangeIndicator(float newValue = 0f)
        {
            if (rangeIndicator == null || m_BuildingMono == null) return;
        
            // 获取攻击范围
            float attackRange = m_BuildingMono.buildingLogic.buildingInfo.attackRange.Value;
        
            // 获取精灵的原始大小（世界单位）
            float spriteSize = rangeIndicator.sprite.bounds.size.x;
        
            // 计算缩放比例：攻击范围直径 / 精灵原始大小
            float scale = (attackRange * 2) / spriteSize;
        
            // 应用缩放
            rangeIndicator.transform.localScale = new Vector3(scale, scale, 1f);
        }

        /// <summary>
        /// 这是脚本的回收逻辑
        /// </summary>
        public void Recycle()
        {
            m_Initialized = false;
            
            m_BuildingMono.buildingLogic.buildingInfo.attackRange.OnValueChanged -= UpdateRangeIndicator;
        }
        
    }
}