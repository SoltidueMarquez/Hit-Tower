using Enemy;
using UnityEngine;

namespace Buff_System.BuffModules
{
    [CreateAssetMenu(fileName = "FrenzyOn", menuName = "BuffSystem/BuffModule/Frenzy/On")]
    public class FrenzyOn : BaseBuffModule
    {
        public Vector2 deltaSpeed;
        public override void Apply(BuffInfo buffInfo)
        {
            var character = buffInfo.target.GetComponent<EnemyMono>(); //找到目标身上的角色脚本
            if (character)
            {
                var enemy = character.enemyLogic;
                enemy.OnFirstDropDownHalf += (() =>
                {
                    Debug.Log("111");
                    SetFrenzy(enemy.EnemyInfo);
                });
            }
        }

        private void SetFrenzy(EnemyInfo info)
        {
            if(deltaSpeed!=Vector2.zero)
            {
                info.speed.ModifyAdditive(deltaSpeed.x);
                info.speed.ModifyMultiplier(deltaSpeed.y);
            }
        }
    }
}