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

        public List<AdditionalBuff> additionalBuffs { get; private set; }

        // public void Update()
        // {
        //     // TODO:这里是additionalBuffs的计时器更新逻辑
        // }

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
        
    }
}