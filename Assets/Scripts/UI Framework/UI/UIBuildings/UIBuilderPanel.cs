using System;
using Buildings;
using UI_Framework.Scripts.Tools;
using UnityEngine;

namespace UI_Framework.UI.UIBuildings
{
    public class UIBuildingPanel : MonoBehaviour
    {
        public UIList buildBtnList;
        private PlacementCell m_Cell;

        public void Init(PlacementCell cell)
        {
            m_Cell = cell;
            
            buildBtnList.ClearItems();
            foreach (var buildingData in GameManager.Instance.buildingManager.builder.buildingDatas.buildingDataList)
            {
                buildBtnList.CloneItem<UIBuildingBtn>().Init(buildingData,
                    () =>
                    {
                        m_Cell.Build(buildingData.buildingName);// 建造建筑物
                        Close();// 关闭本界面
                    });
            }

            GameManager.Instance.playerManager.playerLogic.OnMoneyChanged += UpdateBuildBtnsInteractable;
            GameManager.Instance.buildingManager.buildingViewModelHelper.OnPlacementCellSelectDataChanged += UpdateSelf;
            
            Close();
        }

        private void UpdateSelf(PlacementCellModelData data)
        {
            if (data.currentSelectedPlacementCell != m_Cell && gameObject.activeSelf)
            {
                Close();
            }
            if (data.currentSelectedPlacementCell == m_Cell && !gameObject.activeSelf)
            {
                Open();
            }
        }

        public void Open()
        {
            gameObject.SetActive(true);
            UpdateBuildBtnsInteractable(0, GameManager.Instance.playerManager.playerLogic.money);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        private void UpdateBuildBtnsInteractable(float old, float newMoney)
        {
            if (!gameObject.activeSelf) return;// 如果界面关闭就不更新
            foreach (var item in buildBtnList.items)
            {
                item.GetComponent<UIBuildingBtn>().UpdateInteractable(newMoney);
            }
        }

        private void OnDestroy()
        {
            GameManager.Instance.playerManager.playerLogic.OnMoneyChanged -= UpdateBuildBtnsInteractable;
            GameManager.Instance.buildingManager.buildingViewModelHelper.OnPlacementCellSelectDataChanged -= UpdateSelf;
        }
    }
}