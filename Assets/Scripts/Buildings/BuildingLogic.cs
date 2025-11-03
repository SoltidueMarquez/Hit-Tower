using System;
using System.Collections.Generic;
using Buff_System;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace Buildings
{
    public class BuildingLogic
    {
        public BuildingInfo buildingInfo { get; private set; }
        
        public BuffHandler BuffHandler { get; private set; }

        private readonly GameObject m_MonoGameObject;
        
        #region 回调
        public event Action OnDie;
        public event Action OnBeAttacked;
        #endregion

        public BuildingLogic(BuildingData data , GameObject mono)
        {
            buildingInfo = new BuildingInfo(data);
            // 挂载配置的buff
            BuffHandler = new BuffHandler();
            foreach (var buffData in data.levelData[0].addBuffs)
            {
                BuffHandler.AddBuff(new BuffInfo(buffData, mono, mono));
            }

            m_MonoGameObject = mono;

            buildingInfo.maxHealth.OnValueChanged += ReCalculateHealth;
        }

        /// <summary>
        /// 升级函数
        /// </summary>
        /// <returns></returns>
        public bool LevelUp()
        {
            if (!buildingInfo.LevelUp()) return false;
            foreach (var buffData in buildingInfo.levelData[buildingInfo.curLv].addBuffs)
            {
                BuffHandler.AddBuff(new BuffInfo(buffData, m_MonoGameObject, m_MonoGameObject));
            }

            return true;
        }
        
        /// <summary>
        /// 周期性执行函数
        /// </summary>
        public void Tick()
        {
            BuffHandler.Tick();
        }

        #region 血量与死亡函数

        /// <summary>
        /// 修改当前血量的接口
        /// </summary>
        /// <param name="delta"></param>
        /// <returns>返回new-old的delta</returns>
        public float ModifyCurrentHealth(float delta)
        {
            var original = buildingInfo.curHealth;
            buildingInfo.curHealth += delta;
            // 控制血量不越界
            buildingInfo.curHealth = Mathf.Clamp(buildingInfo.curHealth, 0, buildingInfo.maxHealth.Value);

            if (delta < 0)
            {
                OnBeAttacked?.Invoke();
            }
            
            if (Mathf.Approximately(buildingInfo.curHealth, 0f))
            {
                Die();
            }
            return original - buildingInfo.curHealth;
        }

        private void ReCalculateHealth(float maxHealthDelta)
        {
            buildingInfo.curHealth = Mathf.Clamp(buildingInfo.curHealth, 0, buildingInfo.maxHealth.Value);
            
            if (Mathf.Approximately(buildingInfo.curHealth, 0f))
            {
                Die();
            }
        }

        private void Die(bool isKilledByPlayer = true)
        {
            // 还钱
            if(isKilledByPlayer) GameManager.Instance.playerManager.playerLogic.ModifyMoney(buildingInfo.giveBack.Value);
            OnDie?.Invoke();
        }

        public void SetDie(bool isKilledByPlayer = true)
        {
            Die(isKilledByPlayer);
        }

        public void Recycle()
        {
            SetDie(true);
        }
        #endregion
    }

    public class BuildingInfo
    {
        public string buildingName;
        public int curLv { get; private set; }
        public int maxLv { get; private set; }

        [LabelText("攻击力")] public ValueChannel attack;
        [LabelText("攻击范围")] public ValueChannel attackRange;
        [LabelText("攻击间隔")] public ValueChannel attackInterval;
        [LabelText("单体攻击")] public bool ifSingle;
        [LabelText("同时攻击的敌人个数")] public ValueChannel attackNum;
        [LabelText("拆除返还的金币")] public ValueChannel giveBack;
        
        [Header("暂时用不上")] 
        public ValueChannel maxHealth;
        public float curHealth;


        public List<BuildingLevelData> levelData { get; private set; }

        public BuildingInfo(BuildingData data)
        {
            levelData = data.levelData;
            buildingName = data.buildingName;
            curLv = 0;
            maxLv = data.maxLv;

            // 数值默认从0级开始
            attack = new ValueChannel(levelData[0].attack);
            attackRange = new ValueChannel(levelData[0].attackRange);
            attackInterval = new ValueChannel(levelData[0].attackInterval);
            ifSingle = levelData[0].ifSingle;
            attackNum = new ValueChannel(levelData[0].attackNum);
            giveBack = new ValueChannel(levelData[0].giveBack);
            maxHealth = new ValueChannel(levelData[0].maxHealth);
            curHealth = maxHealth.Value;
        }

        /// <summary>
        /// 获取升级的花费
        /// </summary>
        /// <returns></returns>
        public float upgradeCost => CheckIfMaxLv() ? -1 : levelData[curLv + 1].cost;

        /// <summary>
        /// 升级
        /// </summary>
        /// <returns></returns>
        public bool LevelUp()
        {
            if (CheckIfMaxLv())
            {
                Debug.LogWarning("超出升级范围");
                return false;
            }

            curLv++;
            attack.SetBaseValue(levelData[curLv].attack);
            attackRange .SetBaseValue(levelData[curLv].attackRange);
            attackInterval.SetBaseValue(levelData[curLv].attackInterval);
            ifSingle = levelData[curLv].ifSingle;
            attackNum.SetBaseValue(levelData[curLv].attackNum);
            maxHealth.SetBaseValue(levelData[curLv].maxHealth);
            giveBack.SetBaseValue(levelData[curLv].giveBack);
            curHealth = maxHealth.Value;

            return true;
        }

        /// <summary>
        /// 是否满级
        /// </summary>
        /// <returns></returns>
        public bool CheckIfMaxLv()
        {
            return curLv == maxLv;
        }
    }
}
