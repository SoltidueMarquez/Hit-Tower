using System;
using Enemy;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Buff_System.BuffModules
{
    [CreateAssetMenu(fileName = "_ModifyEnemyInfoBuffModule",
        menuName = "BuffSystem/BuffModule/ModifyEnemyInfoBuffModule", order = 1)]
    public class ModifyEnemyInfoBuffModule : BaseBuffModule
    { 
        public EnemyProperty enemyProperty;

        public override void Apply(BuffInfo buffInfo)
        {
            var character = buffInfo.target.GetComponent<EnemyMono>(); //找到目标身上的角色脚本
            if (character)
            {
                if (enemyProperty.curHealth != 0) character.enemyLogic.ModifyCurrentHealth(enemyProperty.curHealth);
                
                var info = character.enemyLogic.EnemyInfo;
                
                info.maxHealth.ModifyAdditive(enemyProperty.maxHealth.x);
                info.maxHealth.ModifyMultiplier(enemyProperty.maxHealth.y);
                
                info.speed.ModifyAdditive(enemyProperty.speed.x);
                info.speed.ModifyMultiplier(enemyProperty.speed.y);

                info.attack.ModifyAdditive(enemyProperty.attack.x);
                info.attack.ModifyMultiplier(enemyProperty.attack.y);
                
                info.shield.ModifyAdditive(enemyProperty.shield.x);
                info.shield.ModifyMultiplier(enemyProperty.shield.y);
                
                info.value.ModifyAdditive(enemyProperty.value.x);
                info.value.ModifyMultiplier(enemyProperty.value.y);
            }
        }
    }
    
    [Serializable]
    public class EnemyProperty
    {
        [Header("(加算，乘算)")]
        [LabelText("血量上限")] public Vector2 maxHealth;
        [LabelText("当前血量，只能加减")] public float curHealth;
        [LabelText("速度")] public Vector2 speed;
        [LabelText("攻击力")] public Vector2 attack;
        [LabelText("护甲")] public Vector2 shield;
        [LabelText("击杀后奖励")] public Vector2 value;
    }
}