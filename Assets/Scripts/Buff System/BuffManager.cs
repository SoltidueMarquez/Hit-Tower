using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Buff_System
{
    public class BuffManager : MonoBehaviour
    {
        [LabelText("游戏中会出现的,需要外部添加的buff")] public List<BuffData> buffDatas;

        // 为了实现在商店buff购买后再出生的敌人同样会获得效果
        public List<AdditionalBuff> enemyAdditionalBuffs { get; private set; }

        private bool m_Initialized = false;
        public void Init()
        {
            enemyAdditionalBuffs = new List<AdditionalBuff>();
            m_Initialized = true;
        }

        public void Update()
        {
            if (!m_Initialized || enemyAdditionalBuffs.Count == 0) return;
    
            // 倒序遍历，避免删除元素时索引错乱
            for (int i = enemyAdditionalBuffs.Count - 1; i >= 0; i--)
            {
                enemyAdditionalBuffs[i].Update();
            }
        }

        public void AddAdditionalEnemyBuff(BuffData buffData)
        {
            var add = new AdditionalBuff(buffData);
            enemyAdditionalBuffs.Add(add);
            add.OnTimeUp += RemoveAdditionalEnemyBuff;
        }

        private void RemoveAdditionalEnemyBuff(AdditionalBuff buff)
        {
            enemyAdditionalBuffs.Remove(buff);
        }

        public BuffData GetBuffData(int id)
        {
            return buffDatas.FirstOrDefault(data => data.id == id);
        }
    }

    /// <summary>
    /// 通过商店给敌人加临时的buff在这里，看来需要维护一个计时器,
    /// 干脆强制改buffInfo的信息吧，那就涉及到BuffAdd的时候BuffInfo计时器的设置了
    /// </summary>
    [Serializable]
    public class AdditionalBuff
    {
        private float m_Timer;
        private float m_MaxTime;
        public BuffData buffData;

        public event Action<AdditionalBuff> OnTimeUp;

        public AdditionalBuff(BuffData buffData)
        {
            this.buffData = buffData;
            m_Timer = 0;
            m_MaxTime = buffData.duration;
        }

        public BuffInfo GetBuffInfo(GameObject target)
        {
            var info = new BuffInfo(buffData, GameManager.Instance.buffManager.gameObject, target);
            info.durationTimer = m_Timer;
            return info;
        }

        public void Update()
        {
            m_Timer += Time.deltaTime;
            if (m_Timer >= m_MaxTime)
            {
                OnTimeUp?.Invoke(this);
            }
        }
    }
}