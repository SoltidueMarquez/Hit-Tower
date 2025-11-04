using Utils.StateMachine;

namespace Buildings.Specific_Building.SingleTower.Bullet
{
    public class BulletAttackState : IState
    {
        private StateMachine stateMachine { get; set; }
        private SingleBullet m_Bullet;
        public void SetStateMachine(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void OnEnter()
        {
            // 对列表里的所有敌人造成伤害
            m_Bullet.HurtEnemy();
            // 超过碰撞次数就回收
            m_Bullet.UpdateHitAndCheckRecycle();
            // 切换回飞行状态
            stateMachine.SwitchTo<BulletFlyState>();
        }

        public void OnTick()
        {
            
        }

        public void OnExit()
        {
            m_Bullet.hasHit = false;
        }

        public void Init()
        {
            m_Bullet = stateMachine.GetComponent<SingleBullet>();
        }
    }
}