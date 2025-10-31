using Enemy.State;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Utils.StateMachine;

namespace Enemy
{
    [RequireComponent(typeof(StateMachine), typeof(NavMeshAgent))]
    public class EnemyMono : MonoBehaviour
    {
        [LabelText("逻辑类")] private EnemyLogic m_EnemyLogic;
        [LabelText("管理器")] private EnemyManager m_EnemyManager;
        [LabelText("Nav组件")] private NavMeshAgent m_Agent;

        [LabelText("状态机")] private StateMachine m_StateMachine;

        public void Init(EnemyData enemyData, EnemyManager manager)
        {
            m_EnemyLogic = new EnemyLogic(enemyData);
            m_EnemyManager = manager;
            m_Agent = GetComponent<NavMeshAgent>();
            SetSpeed(m_EnemyLogic.enemyInfo.speed.value);

            InitStateMachine();

            // 事件订阅
            m_EnemyLogic.OnDie += Recycle;
            m_EnemyManager.OnTick += Tick;
            m_EnemyLogic.enemyInfo.speed.OnValueChanged += SetSpeed;
        }

        private void InitStateMachine()
        {
            // 添加状态机组
            m_StateMachine = GetComponent<StateMachine>();
        
            // 注册状态
            m_StateMachine.RegisterState(new IdleState());
            m_StateMachine.RegisterState(new MoveState());
            m_StateMachine.RegisterState(new AttackState());
        
            // 设置初始状态
            m_StateMachine.SwitchTo<IdleState>();
        }

        private void SetSpeed(float speed)
        {
            m_Agent.speed = speed;
        }
        
        private void Tick()
        {
            m_EnemyLogic.Tick();
            m_StateMachine.Tick();
        }

        // TODO:对象池
        private void Recycle()
        {
        }
    }
}