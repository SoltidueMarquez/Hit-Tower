using Player;
using TMPro;
using UI_Framework.Scripts;

namespace UI_Framework.UI
{
    public class PlayerInfoUI : UIFormBase
    {
        public TextMeshProUGUI healthText;
        public TextMeshProUGUI moneyText;

        private PlayerManager m_PlayerManager;

        protected override void OnInit()
        {
            m_PlayerManager = GameManager.Instance.playerManager;
            m_PlayerManager.playerLogic.OnHealthChanged += SetHealthText;
            m_PlayerManager.playerLogic.OnMoneyChanged += SetMoneyText;

            SetHealthText(0, m_PlayerManager.playerLogic.curHealth);
            SetMoneyText(0, m_PlayerManager.playerLogic.money);

            Open();
        }

        protected override void BeforeDestroy()
        {
            m_PlayerManager.playerLogic.OnHealthChanged -= SetHealthText;
            m_PlayerManager.playerLogic.OnMoneyChanged -= SetMoneyText;
        }

        private void SetHealthText(float old, float currentHealth)
        {
            healthText.text = $"Health: {currentHealth} / {m_PlayerManager.playerLogic.maxHealth.Value}";
        }

        private void SetMoneyText(float old, float currentMoney)
        {
            moneyText.text = $"Money: {currentMoney}";
        }
    }
}