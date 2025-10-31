using System;
using Buff_System;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace Enemy
{
    public class EnemyLogic
    {
        private EnemyInfo m_EnemyInfo;
        
        public BuffHandler buffHandler { get; private set; }

        #region 回调
        public event Action OnDie;
        public event Action OnBorn;
        public event Action OnBeAttacked;
        #endregion
        
        public EnemyLogic(EnemyData enemyData)
        {
            m_EnemyInfo = new EnemyInfo(enemyData);
            buffHandler = new BuffHandler();
        }

        /// <summary>
        /// 周期性执行函数
        /// </summary>
        public void Tick()
        {
            buffHandler.Tick();
        }

        /// <summary>
        /// 修改当前血量的接口
        /// </summary>
        /// <param name="delta"></param>
        /// <returns>返回new-old的delta</returns>
        public float ModifyCurrentHealth(float delta)
        {
            var original = m_EnemyInfo.curHealth;
            m_EnemyInfo.curHealth += delta;
            // 控制血量不越界
            m_EnemyInfo.curHealth = Mathf.Clamp(m_EnemyInfo.curHealth, 0, m_EnemyInfo.maxHealth.value);
            if (Mathf.Approximately(m_EnemyInfo.curHealth, 0f))
            {
                Die();
            }
            return original - m_EnemyInfo.curHealth;
        }

        private void Die()
        {
            OnDie?.Invoke();
        }
    }

    /// <summary>
    /// 敌人的运行时信息类
    /// </summary>
    public class EnemyInfo
    {
        [LabelText("名称")] public string enemyName;
        [LabelText("血量")] public ValueChannel maxHealth;
        [LabelText("当前血量")] public float curHealth;
        [LabelText("速度")] public ValueChannel speed;
        [LabelText("攻击力")] public ValueChannel attack;
        [LabelText("护甲")] public ValueChannel shield;
        [LabelText("击杀后奖励")] public ValueChannel value;

        public EnemyInfo(EnemyData enemyData)
        {
            enemyName = enemyData.enemyName;
            maxHealth = new ValueChannel(enemyData.maxHealth);
            curHealth = enemyData.maxHealth;
            speed = new ValueChannel(enemyData.speed);
            attack = new ValueChannel(enemyData.attack);
            shield = new ValueChannel(enemyData.shield);
            value = new ValueChannel(enemyData.value);
        }
    }
}