using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Buff_System
{
    [CreateAssetMenu(fileName = "_BuffData",menuName = "BuffSystem/BuffData",order = 1)]
    public class BuffData : ScriptableObject
    {
        [Header("基本信息")] 
        [LabelText("buff标识")] public int id;
        [LabelText("buff名称")] public string buffName;
        [LabelText("buff描述"), TextArea(5, 3)] public string description;
        [LabelText("buff图标")] public Sprite icon;
        [LabelText("buff优先级")] public int priority;
        [LabelText("buff最大层数")] public int maxStack;

        [Header("时间信息")]
        [LabelText("是否永久")] public bool isForever;
        [LabelText("持续时间")] public float duration;
        [LabelText("触发间隔")] public float tickTime;
        
        [Header("更新方式")]
        [LabelText("叠加方式")] public BuffUpdateTimeEnum buffUpdateTime;
        [LabelText("移除方式")] public BuffRemoveStackUpdateEnum buffRemoveStackUpdate;

        [Header("基础回调点")] 
        [LabelText("创建Buff时")] public List<BaseBuffModule> OnCreate;
        [LabelText("移除Buff时")] public List<BaseBuffModule> OnRemove;
        [LabelText("触发Buff时")] public List<BaseBuffModule> OnTick;
    }
}
