// using UnityEngine;
//
// namespace Buff_System.Example
// {
//     /// <summary>
//     /// 修改角色属性的实现类
//     /// </summary>
//     [CreateAssetMenu(fileName = "_ChangePropertyBuffModule",menuName = "BuffSystem/BuffModule/ChangePropertyBuffModule",order = 1)]
//     public class ChangePropertyBuffModule : BaseBuffModule
//     {
//         public Property property;
//         public override void Apply(BuffInfo buffInfo, DamageInfo damageInfo = null)
//         {
//             var character = buffInfo.target.GetComponent<Character>();//找到目标身上的角色脚本
//             if (character)
//             {
//                 character.property.hp += property.hp;
//                 character.property.speed += property.speed;
//                 character.property.atk += property.atk;
//             } 
//         }
//     }
// }