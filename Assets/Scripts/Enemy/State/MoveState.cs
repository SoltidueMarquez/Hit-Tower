using Buildings;
using UnityEngine;
using UnityEngine.AI;
using Utils.StateMachine;

namespace Enemy.State
{
    public class MoveState : IState
    {
        private StateMachine stateMachine { get; set; }
        private NavMeshAgent m_Agent;
        
        public void SetStateMachine(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void OnEnter()
        {
            m_Agent = stateMachine.gameObject.GetComponent<NavMeshAgent>();
            MoveTo(Player.Instance.playerPos);
        }

        public void OnTick()
        {
            // TODO:实现Attack状态的切换
        }

        private void MoveTo(Vector3 target)
        {
            m_Agent.isStopped = false;
            m_Agent.SetDestination(target);
        }

        public void OnExit()
        {
            m_Agent.isStopped = true;
        }
    }
}