using UnityEngine;
using Utils.StateMachine;

namespace Buildings.Specific_Building.SingleTower.Bullet
{
    public class BulletFlyState : IState
    {
        private StateMachine stateMachine { get; set; }
        private SingleBullet m_Bullet;
        
        private float m_AliveTimer = 0f; // 计时器字段
        
        public void SetStateMachine(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void OnEnter()
        {
            m_AliveTimer = 0f;
        }

        public void OnTick()
        {
            // 如果超过maxAliveTime就需要回收自己
            m_AliveTimer += Time.deltaTime;
            if (m_AliveTimer >= m_Bullet.maxAliveTime) 
            {
                m_AliveTimer = 0f; // 重置计时器
                m_Bullet.Recycle();
            }
            
            // 移动
            m_Bullet.MoveToward();

            // 达到某个目标了
            if (m_Bullet.hasHit)
            {
                stateMachine.SwitchTo<BulletAttackState>();
            }
        }

        public void OnExit()
        {
            // 如果有溅射的话就更新目标列表
            m_Bullet.DoSputterCheckAndUpdateTargets();
        }

        public void Init()
        {
            m_Bullet = stateMachine.GetComponent<SingleBullet>();
        }
    }
}