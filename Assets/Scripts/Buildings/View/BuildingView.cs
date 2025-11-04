using Sirenix.OdinInspector;
using UI_Framework.Scripts;
using UI_Framework.UI.UIBuildings;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Buildings
{
    public class BuildingView : MonoBehaviour, IPointerClickHandler
    {
        [LabelText("攻击范围指示器"), SerializeField] protected SpriteRenderer rangeIndicator;
        protected Color rangeIndicatorColor;
        protected BuildingMono m_BuildingMono;
        protected UIBuildingControlPanel m_ControlPanel;

        private bool m_Initialized = false;
        public virtual void Init(BuildingMono mono)
        {
            m_BuildingMono = mono;

            rangeIndicatorColor = rangeIndicator.color;
            UpdateRangeIndicator();
            
            // 订阅范围更新事件
            m_BuildingMono.buildingLogic.buildingInfo.attackRange.OnValueChanged += UpdateRangeIndicator;
            
            // 创建对应的m_ControlPanel
            m_ControlPanel = UIMgr.Instance.GetFirstUI<UIBuildings>().CreateBuildingControlPanel(m_BuildingMono);

            m_Initialized = true;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_Initialized)
            {
                GameManager.Instance.buildingManager.buildingViewModelHelper.OnBuildingClicked(this);
            }
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

        public void SetRangeIndicatorColor(Color color,bool needReset = false)
        {
            rangeIndicator.color =
                needReset ? rangeIndicatorColor : new Color(color.r, color.g, color.b, rangeIndicatorColor.a);
        }
        #endregion

        public virtual void AtkAnim() { }

        /// <summary>
        /// 这是脚本的回收逻辑
        /// </summary>
        public void Recycle()
        {
            m_Initialized = false;
            
            m_BuildingMono.buildingLogic.buildingInfo.attackRange.OnValueChanged -= UpdateRangeIndicator;

            // 直接销毁UI吧，对象池要考虑得太多了，不会消耗多少性能的
            if (m_ControlPanel != null) Destroy(m_ControlPanel.gameObject);
        }
        
    }
}