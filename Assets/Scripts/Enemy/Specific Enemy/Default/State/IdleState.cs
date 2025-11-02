// IdleState.cs

using Utils.StateMachine;

namespace Enemy.State
{
    public class IdleState : IState
    {
        private StateMachine stateMachine { get; set; }
        
        public void SetStateMachine(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void OnEnter()
        {
            // TODO：等待一段时间再执行
            if(stateMachine!=null) stateMachine.SwitchTo<MoveState>();
        }

        public void OnTick()
        {
        }

        public void OnExit()
        {
        }

        public void Init()
        {
            
        }
    }
}