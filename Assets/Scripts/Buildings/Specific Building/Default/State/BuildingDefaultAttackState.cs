using Utils.StateMachine;

namespace Buildings.Specific_Building.Default.State
{
    public class BuildingDefaultAttackState : IState
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