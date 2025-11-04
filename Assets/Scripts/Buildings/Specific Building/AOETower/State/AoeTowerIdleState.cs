using UnityEngine;
using Utils.StateMachine;

namespace Buildings.Specific_Building.AOETower.State
{
    public class AoeTowerIdleState : IState
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
                stateMachine.SwitchTo<AoeTowerAttackState>();
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