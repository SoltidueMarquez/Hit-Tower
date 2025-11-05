using System;
using Buildings;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Buff_System.BuffModules
{
    [CreateAssetMenu(fileName = "_ModifyBuildingInfo",
        menuName = "BuffSystem/BuffModule/ModifyBuildingInfo", order = 1)]
    public class ModifyBuildingInfoBuffModule : BaseBuffModule
    {
        public BuildingProperty buildingProperty;
        public override void Apply(BuffInfo buffInfo)
        {
            var character = buffInfo.target.GetComponent<BuildingMono>(); //找到目标身上的角色脚本
            if (character)
            {
                var info = character.buildingLogic.buildingInfo;

                if(buildingProperty.attackInterval!=Vector2.zero)
                {
                    info.attackInterval.ModifyAdditive(buildingProperty.attackInterval.x);
                    info.attackInterval.ModifyMultiplier(buildingProperty.attackInterval.y);
                }
                
                if(buildingProperty.attack!=Vector2.zero)
                {
                    info.attack.ModifyAdditive(buildingProperty.attack.x);
                    info.attack.ModifyMultiplier(buildingProperty.attack.y);
                }
                
                if(buildingProperty.attackRange!=Vector2.zero)
                {
                    info.attackRange.ModifyAdditive(buildingProperty.attackRange.x);
                    info.attackRange.ModifyMultiplier(buildingProperty.attackRange.y);
                }
                
                if(buildingProperty.giveBack!=Vector2.zero)
                {
                    info.giveBack.ModifyAdditive(buildingProperty.giveBack.x);
                    info.giveBack.ModifyMultiplier(buildingProperty.giveBack.y);
                }
                
                if(buildingProperty.attackNum!=Vector2.zero)
                {
                    info.attackNum.ModifyAdditive(buildingProperty.attackNum.x);
                    info.attackNum.ModifyMultiplier(buildingProperty.attackNum.y);
                }
            }
        }
    }
    [Serializable]
    public class BuildingProperty
    {
        [Header("(加算，乘算)")]
        [LabelText("攻击力")] public Vector2 attack;
        [LabelText("攻击范围")] public Vector2 attackRange;
        [LabelText("攻击间隔")] public Vector2 attackInterval;
        [LabelText("同时攻击的敌人个数")] public Vector2 attackNum;
        [LabelText("拆除返还的金币")] public Vector2 giveBack;
    }
    
}