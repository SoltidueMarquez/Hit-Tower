using Buildings;
using UI_Framework.Scripts;
using UnityEngine;
using Utils;

namespace UI_Framework.UI.UIBuildings
{
    public class UIBuildings : UIFormBase
    {
        [Tooltip("建造面板")] public GameObject uiBuildingPanelPrefab;
        [Tooltip("升级/拆除/查看面板")] public GameObject uiBuildingControlPanelPrefab;

        protected override void OnInit()
        {
            foreach (var placementCell in GameManager.Instance.buildingManager.placementCells)
            {
                var panel = Instantiate(uiBuildingPanelPrefab, transform);
                // 实现位置映射：将世界坐标转换为UI坐标
                UIUtils.MapWorldPositionToUI(placementCell.transform.position, panel.transform as RectTransform);
                panel.GetComponent<UIBuildingPanel>().Init(placementCell);
            }
            
            Open();
        }

        /// <summary>
        /// 每个building也需要创造一个对应的UI
        /// </summary>
        /// <param name="mono"></param>
        /// <returns></returns>
        public UIBuildingControlPanel CreateBuildingControlPanel(BuildingMono mono)
        {
            var panel = Instantiate(uiBuildingControlPanelPrefab, transform);
            // 实现位置映射：将世界坐标转换为UI坐标
            UIUtils.MapWorldPositionToUI(mono.transform.position, panel.transform as RectTransform);
            var buildingPanel = panel.GetComponent<UIBuildingControlPanel>();
            buildingPanel.Init(mono);
            return buildingPanel;
        }
    }
}
