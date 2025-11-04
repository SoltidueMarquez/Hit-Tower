using Enemy;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Buff_System.BuffModules
{
    [CreateAssetMenu(fileName = "_Burning",
        menuName = "BuffSystem/BuffModule/Burning", order = 2)]
    public class Burning : BaseBuffModule
    {
        [LabelText("每层每次灼烧的最大生命值百分比")] public float percent;
        public override void Apply(BuffInfo buffInfo)
        {
            var character = buffInfo.target.GetComponent<EnemyMono>(); //找到目标身上的角色脚本
            if (character)
            {
                var finalPercent = buffInfo.curStack * percent;

                if (finalPercent != 0)
                    character.enemyLogic
                        .ModifyCurrentHealth(-finalPercent / 100 * character.enemyLogic.EnemyInfo.maxHealth.Value);
            }
        }
    }
}