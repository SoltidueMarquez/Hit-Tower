using Buildings.Specific_Building.SingleTower;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Buff_System.BuffModules
{
    [CreateAssetMenu(fileName = "_ModifyPenetrateNum", menuName = "BuffSystem/BuffModule/ModifyPenetrateNum", order = 3)]
    public class ModifyPenetrateNumBuffModule : BaseBuffModule
    {
        [LabelText("改变的值(加成、乘数)")] public Vector2 num;
        public override void Apply(BuffInfo buffInfo)
        {
            var tower = buffInfo.target.GetComponent<SingleTower>(); //找到目标身上的角色脚本
            if (tower)
            {
                if (num.x != 0)
                {
                    tower.penetrateNum.ModifyAdditive(num.x);
                }

                if (num.y != 0)
                {
                    tower.penetrateNum.ModifyMultiplier(num.y);
                }
            }
        }
    }
}