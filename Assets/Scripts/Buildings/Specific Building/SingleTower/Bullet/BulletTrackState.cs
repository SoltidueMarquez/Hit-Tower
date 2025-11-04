// using Enemy;
// using UnityEngine;
// using Utils.StateMachine;
//
// namespace Buildings.Specific_Building.SingleTower.Bullet
// {
//     public class BulletTrackState : IState
//     {
//         private StateMachine stateMachine { get; set; }
//         private SingleBullet m_Bullet;
//
//         private EnemyMono m_Target;
//         public void SetStateMachine(StateMachine stateMachine)
//         {
//             this.stateMachine = stateMachine;
//         }
//
//         public void OnEnter()
//         {
//             // 只会进入一次
//         }
//
//         public void OnTick()
//         {
//             if (!m_Bullet.isActiveAndEnabled)// 如果敌人已经死亡了,就直接进入飞行模式
//             {
//                 stateMachine.SwitchTo<BulletFlyState>();
//             }
//
//             // 如果到达了就进行攻击
//             if (Mathf.Abs(m_Bullet.transform.position.x-m_Target.transform.position.x)<=10&&
//                 Mathf.Approximately(m_Bullet.transform.position.z , m_Target.transform.position.z))
//             {
//                 stateMachine.SwitchTo<BulletAttackState>();
//             }
//             
//             // 移动
//             // m_Bullet.MoveTo(m_Target.transform);
//         }
//
//         public void OnExit()
//         {
//             // 如果有溅射的话就更新目标列表
//             m_Bullet.DoSputterCheckAndUpdateTargets();
//         }
//
//         public void Init()
//         {
//             m_Bullet = stateMachine.GetComponent<SingleBullet>();
//             m_Target = m_Bullet.targets[0];
//         }
//     }
// }