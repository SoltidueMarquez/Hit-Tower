using UI_Framework.Scripts;
using UnityEngine;

namespace UI_Framework.UI.UIBuildings
{
    public class UIBuildings : UIFormBase
    {
        public GameObject uiBuildingPanelPrefab;
        protected override void OnInit()
        {
            foreach (var placementCell in GameManager.Instance.buildingManager.placementCells)
            {
                var panel = Instantiate(uiBuildingPanelPrefab, transform);
                placementCell.SetBuildPanel(panel.GetComponent<UIBuildingPanel>());
                // 实现位置映射：将世界坐标转换为UI坐标
                MapWorldPositionToUI(placementCell.transform.position, panel.transform as RectTransform);
                panel.GetComponent<UIBuildingPanel>().Init(placementCell);
            }
            
            Open();
        }
        
        /// <summary>
        /// 将世界坐标映射到UI坐标
        /// </summary>
        /// <param name="worldPosition">建筑单元格的世界坐标</param>
        /// <param name="uiElement">UI元素的RectTransform</param>
        private void MapWorldPositionToUI(Vector3 worldPosition, RectTransform uiElement)
        {
            if (uiElement == null) return;
            
            // 获取世界摄像机
            Camera worldCamera = GameManager.Instance.gameCamera;
            // 获取UI摄像机（屏幕空间）
            Camera uiCamera = GameManager.Instance.uiCamera;
            
            // 将世界坐标转换为屏幕坐标
            Vector3 screenPoint = worldCamera.WorldToScreenPoint(worldPosition);
            
            // 将屏幕坐标转换为UI Canvas的局部坐标
            RectTransform parentCanvas = uiElement.parent as RectTransform;
            if (parentCanvas != null)
            {
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentCanvas, 
                    screenPoint, 
                    uiCamera, 
                    out localPoint);
                
                // 设置UI元素的位置
                uiElement.anchoredPosition = localPoint;
                
                // 可选：调整Z轴位置确保正确显示
                uiElement.localPosition = new Vector3(
                    uiElement.localPosition.x, 
                    uiElement.localPosition.y, 
                    0f);
            }
        }
        
        // TODO：每个building也需要创造一个对应的UI
    }
}
