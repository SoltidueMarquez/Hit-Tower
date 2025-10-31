using Utils.StateMachine;

namespace Enemy.State
{
    public class AttackState : IState
    {
        private StateMachine stateMachine { get; set; }
        
        public void SetStateMachine(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void OnEnter()
        {
            // TODO:测试用的，直接扣满血
            stateMachine.gameObject.GetComponent<EnemyMono>().EnemyLogicMono.ModifyCurrentHealth(-10000);
        }

        public void OnTick()
        {
        }

        public void OnExit()
        {
        }
    }
}