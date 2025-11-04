using Utils.StateMachine;

namespace Buildings.Specific_Building.DOTTower.State
{
    public class DotTowerIdleState : IState
    {
        private StateMachine stateMachine { get; set; }
        private BuildingMono m_Building;
        public void SetStateMachine(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void OnEnter()
        {
            m_Building.buildingView.SetRangeIndicatorVisible(false);
        }

        public void OnTick()
        {
            if (m_Building.enemiesInRange.Count > 0)
            {
                stateMachine.SwitchTo<DotTowerAttackState>();
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