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

            UpdateRangeIndicator();
            
            // 订阅范围更新事件
            m_BuildingMono.buildingLogic.buildingInfo.attackRange.OnValueChanged += UpdateRangeIndicator;
            
            // 订阅viewmodel数据更新
            GameManager.Instance.buildingManager.buildingViewModelHelper.OnBuildingViewSelectDataChanged += OnBuildingViewSelectDataChanged;
            
            m_Initialized = true;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_Initialized)
            {
                GameManager.Instance.buildingManager.buildingViewModelHelper.OnBuildingClicked(this);
            }
        }
        
        protected void OnBuildingViewSelectDataChanged(BuildingViewModelData selectData)
        {
            
            // if (selectData.currentSelectedBuildingView == this)
            // {
            //     
            // }
            // else
            // {
            //     
            // }

            // // TODO:测试部分，默认升级，升到满就拆除建筑
            // if (m_BuildingMono.buildingLogic.buildingInfo.CheckIfMaxLv())
            // {
            //     RecycleBuilding();
            // }
            // else
            // {
            //     UpgradeBuilding();
            // }
        }

        /// <summary>
        /// 升级
        /// </summary>
        protected void UpgradeBuilding()
        {
            m_BuildingMono.UpGrade();
        }
        
        /// <summary>
        /// 拆除建筑物
        /// </summary>
        protected void RecycleBuilding()
        {
            m_BuildingMono.buildingLogic.SetDie();
        }

        #region 范围指示相关
        protected void UpdateRangeIndicator(float newValue = 0f)
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

        public void SetRangeIndicatorVisible(bool visible)
        {
            rangeIndicator.gameObject.SetActive(visible);
        }
        #endregion

        /// <summary>
        /// 这是脚本的回收逻辑
        /// </summary>
        public void Recycle()
        {
            m_Initialized = false;
            
            m_BuildingMono.buildingLogic.buildingInfo.attackRange.OnValueChanged -= UpdateRangeIndicator;
            GameManager.Instance.buildingManager.buildingViewModelHelper.OnBuildingViewSelectDataChanged -= OnBuildingViewSelectDataChanged;
        }
        
    }
}