using Buildings;
using UnityEngine;
using UnityEngine.UI;

namespace UI_Framework.UI.UIBuildings
{
    public class UIBuildingControlPanel : MonoBehaviour
    {
        public Button upgradeBtn;
        public Button recycleBtn;
        
        private BuildingMono m_BuildingMono;
        
        public void Init(BuildingMono buildingMono)
        {
            m_BuildingMono = buildingMono;
            
            recycleBtn.onClick.AddListener(m_BuildingMono.buildingLogic.Recycle);
            upgradeBtn.onClick.AddListener(m_BuildingMono.UpGrade);

            GameManager.Instance.playerManager.playerLogic.OnMoneyChanged += UpdateUpgradeBtn;
            GameManager.Instance.buildingManager.buildingViewModelHelper.OnBuildingViewSelectDataChanged += UpdateSelf;
            
            Close();
        }

        private void UpdateUpgradeBtn(float old, float newMoney)
        {
            if (!gameObject.activeSelf) return;// 页面关闭时就不更新了
            upgradeBtn.interactable = (newMoney >= m_BuildingMono.buildingLogic.buildingInfo.upgradeCost) &&
                                      !m_BuildingMono.buildingLogic.buildingInfo.CheckIfMaxLv();
        }
        
        private void UpdateSelf(BuildingViewModelData data)
        {
            if (data.currentSelectedBuildingView != m_BuildingMono.buildingView && gameObject.activeSelf)
            {
                Close();
            }
            if (data.currentSelectedBuildingView == m_BuildingMono.buildingView && !gameObject.activeSelf)
            {
                Open();
            }
        }

        private void Open()
        {
            gameObject.SetActive(true);
            
            UpdateUpgradeBtn(0, GameManager.Instance.playerManager.playerLogic.money);
        }

        private void Close()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            GameManager.Instance.playerManager.playerLogic.OnMoneyChanged -= UpdateUpgradeBtn;
            GameManager.Instance.buildingManager.buildingViewModelHelper.OnBuildingViewSelectDataChanged -= UpdateSelf;
        }
    }
}