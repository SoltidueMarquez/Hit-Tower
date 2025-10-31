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
        }

        public void OnTick()
        {
        }

        public void OnExit()
        {
        }
    }
}