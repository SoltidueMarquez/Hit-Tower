using System;
using UnityEngine;
using Utils;

namespace Player
{
    public class PlayerLogic
    {
        public float money { get; private set; }
        public float curHealth { get; private set; }
        public ValueChannel maxHealth;
        
        public event Action OnBeAttacked;
        public event Action<float, float> OnHealthChanged;
        public event Action<float, float> OnMoneyChanged;
        public event Action OnDie;
        
        public PlayerLogic(PlayerData data)
        {
            money = data.startMoney;
            curHealth = data.startHealth;
            maxHealth = new ValueChannel(data.maxHealth);

            maxHealth.OnValueChanged += ReCalculateHealth;
        }

        #region 生命值修改
        public float ModifyCurrentHealth(float delta)
        {
            var original = curHealth;
            curHealth += delta;
            // 控制血量不越界
            curHealth = Mathf.Clamp(curHealth, 0, maxHealth.Value);
            OnHealthChanged?.Invoke(original, curHealth);
            
            if (delta < 0)
            {
                OnBeAttacked?.Invoke();
                // Debug.Log($"基地受到伤害{delta}");
                // Debug.Log($"基地当前血量{curHealth}");
            }
            
            if (Mathf.Approximately(curHealth, 0f))
            {
                OnDie?.Invoke();
            }
            return original - curHealth;
        }

        private void ReCalculateHealth(float maxHealthDelta)
        {
            curHealth = Mathf.Clamp(curHealth, 0, maxHealth.Value);
            
            if (Mathf.Approximately(curHealth, 0f))
            {
                OnDie?.Invoke();
            }
        }
        #endregion

        #region 货币修改
        public bool ModifyMoney(float delta)
        {
            if (money + delta < 0) return false;
            var original = money;
            
            money += delta;
            OnMoneyChanged?.Invoke(original, money);
            return true;
        }
        #endregion
    }
}