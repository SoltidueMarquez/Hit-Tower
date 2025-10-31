using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Buff_System
{
    /// <summary>
    /// Buff更新方式枚举
    /// </summary>
    public enum BuffUpdateTimeEnum
    {
        Add,
        Replace,
        Keep
    }
    
    /// <summary>
    /// Buff移除方式枚举
    /// </summary>
    public enum BuffRemoveStackUpdateEnum
    {
        Clear,
        Reduce
    }

    public class BuffInfo
    {
        [LabelText("Buff数据")] public BuffData buffData;
        [LabelText("创建者")] public GameObject creator;
        [LabelText("目标")] public GameObject target;
        [LabelText("持续时间计时器")] public float durationTimer;
        [LabelText("间隔计时器")] public float tickTimer;
        [LabelText("当前层数")] public int curStack;

        public BuffInfo( BuffData buffData,GameObject creator,GameObject target)
        {
            this.buffData = buffData;
            this.creator = creator;
            this.target = target;
            curStack = 1;
        }
    }

    [Serializable]
    public class Property
    {
        public float hp;
        public float speed;
        public float atk;
    }
}