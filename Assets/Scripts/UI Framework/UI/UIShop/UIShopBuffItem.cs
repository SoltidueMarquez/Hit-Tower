using System;
using Buff_System;
using Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI_Framework.UI.UIShop
{
    public class UIShopBuffItem : MonoBehaviour
    {
        public Button btn;
        public TextMeshProUGUI text;

        private ShopBuff m_Buff;
        public void Init(ShopBuff buff)
        {
            m_Buff = buff;
            text.text = $"{buff.buffData.buffName}-{buff.buffData.duration}s: {buff.cost}";
            btn.onClick.AddListener(TryBuy);
            
            GameManager.Instance.playerManager.playerLogic.OnMoneyChanged += UpdateInteractable;
        }

        public void TryBuy()
        {
            var playerLogic = GameManager.Instance.playerManager.playerLogic;
            if (playerLogic.money >= m_Buff.cost)
            {
                playerLogic.ModifyMoney(-m_Buff.cost);
                Apply();
            }
        }

        private void Apply()
        {
            switch (m_Buff.type)
            {
                case ShopBuffType.Building:
                    foreach (var buildingMono in GameManager.Instance.buildingManager.activeBuildings)
                    {
                        buildingMono.buildingLogic.BuffHandler.AddBuff(new BuffInfo(m_Buff.buffData,gameObject,buildingMono.gameObject));
                    }
                    break;
                case ShopBuffType.Enemy:
                    foreach (var enemyMono in GameManager.Instance.enemyManager.activeEnemies)
                    {
                        enemyMono.enemyLogic.BuffHandler.AddBuff(new BuffInfo(m_Buff.buffData,gameObject,enemyMono.gameObject));
                    }
                    break;
                default:
                    break;
            }
        }

        private void UpdateInteractable(float old, float newMoney)
        {
            btn.interactable = newMoney >= m_Buff.cost;
        }

        private void OnDestroy()
        {
            GameManager.Instance.playerManager.playerLogic.OnMoneyChanged -= UpdateInteractable;
        }
    }
}