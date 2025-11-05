using System;
using System.Collections;
using System.Collections.Generic;
using Buff_System;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace Enemy
{
    public class EnemyLogic
    {
        public EnemyInfo EnemyInfo { get; private set; }

        public BuffHandler BuffHandler { get; private set; }
        
        private readonly GameObject m_MonoGameObject;

        #region 回调
        public event Action OnDie;
        public event Action<float> OnBeAttacked;
        public event Action OnFirstDropDownHalf;
        private bool firstDropDownHalfHealth;
        #endregion
        
        public EnemyLogic(EnemyData enemyData, GameObject mono)
        {
            firstDropDownHalfHealth = false;
            EnemyInfo = new EnemyInfo(enemyData);
            BuffHandler = new BuffHandler();
            m_MonoGameObject = mono;

            var currentCoroutineId = CoroutineHelper.StartWithId(LateHandleBuff());

            EnemyInfo.maxHealth.OnValueChanged += ReCalculateHealth;
        }
        
        public IEnumerator LateHandleBuff()
        {
            yield return null;
            foreach (var buffData in EnemyInfo.initBuffs)
            {
                BuffHandler.AddBuff(new BuffInfo(buffData, m_MonoGameObject, m_MonoGameObject));
            }
        }

        /// <summary>
        /// 周期性执行函数
        /// </summary>
        public void Tick()
        {
            BuffHandler.Tick();
        }

        /// <summary>
        /// 修改当前血量的接口
        /// </summary>
        /// <param name="delta">血量变化量，正数为治疗，负数为伤害</param>
        /// <returns>返回实际的血量变化量（新值-旧值）</returns>
        public float ModifyCurrentHealth(float delta)
        {
            var original = EnemyInfo.curHealth;
            var newDelta = delta;
    
            // 伤害抵抗计算（只在受到伤害时生效）
            if (delta < 0)
            {
                newDelta = delta * EnemyInfo.atkAbsorbPercent.Value;
                newDelta = Mathf.Min(0, EnemyInfo.shield.Value + newDelta);// 计算护盾
                // Debug.Log($"伤害{damageAmount}，吸收{EnemyInfo.atkAbsorbPercent}，实际伤害{newDelta}点");
            }
    
            // 如果没有实际的血量变化，直接返回
            if (Mathf.Approximately(newDelta, 0f))
            {
                OnBeAttacked?.Invoke(newDelta);
                return 0f;
            }
    
            // 应用血量变化
            EnemyInfo.curHealth += newDelta;
    
            // 控制血量不越界
            EnemyInfo.curHealth = Mathf.Clamp(EnemyInfo.curHealth, 0, EnemyInfo.maxHealth.Value);

            // 狂暴的回调
            if (EnemyInfo.curHealth < EnemyInfo.maxHealth.Value / 2 && !firstDropDownHalfHealth)
            {
                firstDropDownHalfHealth = true;
                OnFirstDropDownHalf?.Invoke();
            }
            
            // 触发受击事件（只在受到伤害时）
            if (newDelta < 0)
            {
                OnBeAttacked?.Invoke(newDelta);
            }
    
            // 死亡判断
            if (EnemyInfo.curHealth <= 0f)
            {
                Die();
            }
    
            // 返回实际的血量变化量（新值-旧值）
            return EnemyInfo.curHealth - original;
        }

        private void ReCalculateHealth(float maxHealthDelta)
        {
            EnemyInfo.curHealth = Mathf.Clamp(EnemyInfo.curHealth, 0, EnemyInfo.maxHealth.Value);
            
            if (Mathf.Approximately(EnemyInfo.curHealth, 0f))
            {
                Die();
            }
        }

        private void Die(bool isKilled = true)
        {
            // 加钱
            if(isKilled) GameManager.Instance.playerManager.playerLogic.ModifyMoney(EnemyInfo.value.Value);
            OnDie?.Invoke();
        }

        public void SetDie(bool isKilled = true)
        {
            Die(isKilled);
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
        [LabelText("伤害吸收倍率")] public ValueChannel atkAbsorbPercent;
        [LabelText("护甲")] public ValueChannel shield;
        [LabelText("击杀后奖励")] public ValueChannel value;
        [LabelText("初始挂载的buff")] public List<BuffData> initBuffs;

        public EnemyInfo(EnemyData enemyData)
        {
            enemyName = enemyData.enemyName;
            maxHealth = new ValueChannel(enemyData.maxHealth);
            curHealth = enemyData.maxHealth;
            speed = new ValueChannel(enemyData.speed);
            attack = new ValueChannel(enemyData.attack);
            atkAbsorbPercent = new ValueChannel(enemyData.atkAbsorbPercent);
            shield = new ValueChannel(enemyData.shield);
            value = new ValueChannel(enemyData.value);
            initBuffs = enemyData.initBuffs;
        }
    }
}