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
            // 如果路径无效（如目标点无法到达），则处理失败情况
            if (m_Agent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                // 处理移动失败，例如寻找备用目标或切换到闲置状态
                Debug.LogWarning("无法到达目标位置！");
                stateMachine.SwitchTo<AttackState>();
                return;
            }
            
            // 如果路径有效且已抵达，则切换状态
            if (m_Agent.remainingDistance <= m_Agent.stoppingDistance)
            {
                // 成功抵达，切换到攻击状态
                stateMachine.SwitchTo<AttackState>();
            }
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