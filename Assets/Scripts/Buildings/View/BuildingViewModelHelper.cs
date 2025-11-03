using System;

namespace Buildings
{
    public class BuildingViewModelHelper
    {
        //- 引入MVVC与函数式编程思想优化UI与表现部分（借鉴MVVM的ModelView对View的单向绑定与抽象的函数值编程），增强UI的扩展性，部分UI更新采用数据向下流通消息向上流通的机制。例如：
        // - 设计 NodeSelectHelper（ViewModel层）管理节点选择状态，通过数据绑定自动触发UI刷新。
        // - 新增功能（如发送按钮验证）仅需扩展与切片订阅数据字段（如validityData），无需修改底层逻辑，显著降低耦合度。

        private readonly BuildingViewModelData m_BuildingViewData;
        private readonly PlacementCellModelData m_PlacementCellData;
        public event Action<BuildingViewModelData> OnBuildingViewSelectDataChanged;
        public event Action<PlacementCellModelData> OnPlacementCellSelectDataChanged;

        public BuildingViewModelHelper()
        {
            m_BuildingViewData = new BuildingViewModelData();
            m_PlacementCellData = new PlacementCellModelData();
        }

        #region BuildingView部分
        public void OnBuildingClicked(BuildingView buildingView)
        {
            if (m_BuildingViewData.currentSelectedBuildingView == buildingView)
            {
                DeselectBuilding();
            }
            else
            {
                SelectBuilding(buildingView);
            }
        }

        private void SelectBuilding(BuildingView buildingView)
        {
            m_BuildingViewData.currentSelectedBuildingView = buildingView;
            OnBuildingViewSelectDataChanged?.Invoke(m_BuildingViewData);

            // 弹出升级面板的时候就关闭建造面板
            DeselectCell();
        }

        private void DeselectBuilding()
        {
            if (m_BuildingViewData.currentSelectedBuildingView == null) return;
            m_BuildingViewData.currentSelectedBuildingView = null;
            OnBuildingViewSelectDataChanged?.Invoke(m_BuildingViewData);
        }
        #endregion

        #region PlacementCell部分
        public void OnCellClicked(PlacementCell cell)
        {
            if (m_PlacementCellData.currentSelectedPlacementCell == cell)
            {
                DeselectCell();
            }
            else
            {
                SelectCell(cell);
            }
        }
        
        private void SelectCell(PlacementCell cell)
        {
            m_PlacementCellData.currentSelectedPlacementCell = cell;
            OnPlacementCellSelectDataChanged?.Invoke(m_PlacementCellData);
            
            // 弹出建造面板的时候就关闭升级面板
            DeselectBuilding();
        }

        private void DeselectCell()
        {
            if (m_PlacementCellData.currentSelectedPlacementCell == null) return;
            m_PlacementCellData.currentSelectedPlacementCell = null;
            OnPlacementCellSelectDataChanged?.Invoke(m_PlacementCellData);
        }
        #endregion
    }

    public class BuildingViewModelData
    {
        public BuildingView currentSelectedBuildingView;
    }
    
    public class PlacementCellModelData
    {
        public PlacementCell currentSelectedPlacementCell;
    }
}