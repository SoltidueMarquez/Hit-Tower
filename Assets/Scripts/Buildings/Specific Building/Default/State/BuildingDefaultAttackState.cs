using Utils.StateMachine;

namespace Buildings.Specific_Building.Default.State
{
    public class BuildingDefaultAttackState : IState
    {
        private StateMachine stateMachine { get; set; }
        private BuildingMono m_Building;
        
        public void SetStateMachine(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void OnEnter()
        {
            // 显示攻击范围
            m_Building.buildingView.SetRangeIndicatorVisible(true);
        }

        public void OnTick()
        {
            if (m_Building.enemiesInRange.Count == 0)
            {
                stateMachine.SwitchTo<BuildingDefaultIdleState>();
            }
        }

        public void OnExit()
        {
            
        }

        public void Init()
        {
            m_Building = stateMachine.GetComponent<BuildingMono>();
        }
    }
}