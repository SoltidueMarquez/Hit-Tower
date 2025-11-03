using Buildings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI_Framework.UI.UIBuildings
{
    public class UIBuildingControlPanel : MonoBehaviour
    {
        public Button upgradeBtn;
        public Button recycleBtn;

        public TextMeshProUGUI upgradeBtnText;
        public TextMeshProUGUI recycleBtnText;
        
        private BuildingMono m_BuildingMono;
        
        public void Init(BuildingMono buildingMono)
        {
            m_BuildingMono = buildingMono;
            
            recycleBtn.onClick.AddListener(m_BuildingMono.buildingLogic.Recycle);
            upgradeBtn.onClick.AddListener(m_BuildingMono.UpGrade);

            BindInfoUpdate();
            m_BuildingMono.buildingLogic.buildingInfo.OnLevelUp += UpdateCostText;
            m_BuildingMono.buildingLogic.buildingInfo.giveBack.OnValueChanged += UpdateUpgradeText;
            GameManager.Instance.playerManager.playerLogic.OnMoneyChanged += UpdateUpgradeBtn;
            GameManager.Instance.buildingManager.buildingViewModelHelper.OnBuildingViewSelectDataChanged += UpdateSelf;
            
            Close();
        }

        #region 信息查看面板封装
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI lvText;
        public TextMeshProUGUI attackText;
        public TextMeshProUGUI attackRangeText;
        public TextMeshProUGUI attackIntervalText;
        public TextMeshProUGUI attackNumText;
        public TextMeshProUGUI dpsText;
        public TextMeshProUGUI newBuffText;

        private void UpdateInfoUIs()
        {
            if (!gameObject.activeSelf) return;
            
            var buildingInfo = m_BuildingMono.buildingLogic.buildingInfo;
            nameText.text = buildingInfo.buildingName;
            newBuffText.text = $"New Buff If Upgrade:{buildingInfo.NewBuffString()}";

            var lvString = $"Level:{buildingInfo.curLv}";
            var attackString = $"Attack:{buildingInfo.attack.Value}";
            var attackRangeString = $"Attack Range:{buildingInfo.attackRange.Value}";
            var attackIntervalString = $"Attack Interval:{buildingInfo.attackInterval.Value}";
            var attackNumString = $"Attack Num:{buildingInfo.attackTargetNum}";
            
            // TODO:DPS该怎么算？
            var dpsString = "DPS:???";

            if (!buildingInfo.CheckIfMaxLv())
            {
                var newLv = buildingInfo.curLv + 1;
                lvString += $"—>{newLv}";
                attackString += $"—>{buildingInfo.levelData[newLv].attack}";
                attackRangeString += $"—>{buildingInfo.levelData[newLv].attackRange}";
                attackIntervalString += $"—>{buildingInfo.levelData[newLv].attackInterval}";
                attackNumString += $"—>{buildingInfo.levelData[newLv].attackTargetNum}";
                dpsString += "—>???";
            }
            
            lvText.text = lvString;
            attackText.text = attackString;
            attackRangeText.text = attackRangeString;
            attackIntervalText.text = attackIntervalString;
            attackNumText.text = attackNumString;
            dpsText.text = dpsString;
        }

        private void UpdateInfoUIs(float delta)
        {
            UpdateInfoUIs();
        }

        private void BindInfoUpdate()
        {
            var buildingInfo = m_BuildingMono.buildingLogic.buildingInfo;
            buildingInfo.attack.OnValueChanged += UpdateInfoUIs;
            buildingInfo.attackRange.OnValueChanged += UpdateInfoUIs;
            buildingInfo.attackInterval.OnValueChanged += UpdateInfoUIs;
            buildingInfo.attackNum.OnValueChanged += UpdateInfoUIs;
            buildingInfo.OnLevelUp += UpdateInfoUIs;
            buildingInfo.OnifSingleChanged += UpdateInfoUIs;
        }
        private void UnBindInfoUpdate()
        {
            var buildingInfo = m_BuildingMono.buildingLogic.buildingInfo;
            buildingInfo.attack.OnValueChanged -= UpdateInfoUIs;
            buildingInfo.attackRange.OnValueChanged -= UpdateInfoUIs;
            buildingInfo.attackInterval.OnValueChanged -= UpdateInfoUIs;
            buildingInfo.attackNum.OnValueChanged -= UpdateInfoUIs;
            buildingInfo.OnLevelUp -= UpdateInfoUIs;
            buildingInfo.OnifSingleChanged -= UpdateInfoUIs;
        }
        
        #endregion

        private void UpdateCostText()
        {
            upgradeBtnText.text = (m_BuildingMono.buildingLogic.buildingInfo.CheckIfMaxLv())
                ? "Level Max"
                : $"Upgrade:-{m_BuildingMono.buildingLogic.buildingInfo.upgradeCost}";
        }

        private void UpdateUpgradeText(float delta)
        {
            recycleBtnText.text = $"Recycle:+{m_BuildingMono.buildingLogic.buildingInfo.giveBack.Value}";
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

            UpdateInfoUIs();
            UpdateCostText();
            UpdateUpgradeText(0f);
            UpdateUpgradeBtn(0, GameManager.Instance.playerManager.playerLogic.money);
        }

        private void Close()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            UnBindInfoUpdate();
            m_BuildingMono.buildingLogic.buildingInfo.OnLevelUp -= UpdateCostText;
            m_BuildingMono.buildingLogic.buildingInfo.giveBack.OnValueChanged -= UpdateUpgradeText;
            GameManager.Instance.playerManager.playerLogic.OnMoneyChanged -= UpdateUpgradeBtn;
            GameManager.Instance.buildingManager.buildingViewModelHelper.OnBuildingViewSelectDataChanged -= UpdateSelf;
        }
    }
}