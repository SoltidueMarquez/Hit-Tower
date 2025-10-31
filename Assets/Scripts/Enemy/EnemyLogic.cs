using System;
using Buff_System;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace Enemy
{
    public class EnemyLogic
    {
        public EnemyInfo enemyInfo { get; private set; }

        public BuffHandler buffHandler { get; private set; }

        #region 回调
        public event Action OnDie;
        public event Action OnBeAttacked;
        #endregion
        
        public EnemyLogic(EnemyData enemyData)
        {
            enemyInfo = new EnemyInfo(enemyData);
            buffHandler = new BuffHandler();

            foreach (var buffData in enemyData.initBuffs)
            {
                buffHandler.AddBuff(new BuffInfo(buffData, null, null));
            }
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
            var original = enemyInfo.curHealth;
            enemyInfo.curHealth += delta;
            // 控制血量不越界
            enemyInfo.curHealth = Mathf.Clamp(enemyInfo.curHealth, 0, enemyInfo.maxHealth.value);

            if (delta < 0)
            {
                OnBeAttacked?.Invoke();
            }
            
            if (Mathf.Approximately(enemyInfo.curHealth, 0f))
            {
                Die();
            }
            return original - enemyInfo.curHealth;
        }

        private void Die()
        {
            OnDie?.Invoke();
        }

        public void SetDie()
        {
            Die();
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