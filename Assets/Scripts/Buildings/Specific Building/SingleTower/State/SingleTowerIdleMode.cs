using Utils.StateMachine;

namespace Buildings.Specific_Building.SingleTower.State
{
    public class SingleTowerIdleMode : IState
    {
        private StateMachine stateMachine { get; set; }
        private SingleTower m_SingleTower;
        public void SetStateMachine(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void OnEnter()
        {
            // 隐藏攻击范围
            m_SingleTower.buildingView.SetRangeIndicatorVisible(false);
        }

        public void OnTick()
        {
            if (m_SingleTower.enemiesInRange.Count > 0)
            {
                stateMachine.SwitchTo<SingleTowerAttackMode>();
            }
        }

        public void OnExit()
        {
            
        }

        public void Init()
        {
            m_SingleTower = stateMachine.GetComponent<SingleTower>();
        }
    }
}