using Enemy.State;
using ObjectPool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Utils.StateMachine;

namespace Enemy
{
    [RequireComponent(typeof(StateMachine), typeof(NavMeshAgent))]
    public class EnemyMono : MonoBehaviour
    {
        [field: LabelText("逻辑类")] public EnemyLogic EnemyLogicMono { get; private set; }

        [LabelText("管理器")] private EnemyManager m_EnemyManager;
        [LabelText("Nav组件")] private NavMeshAgent m_Agent;

        [LabelText("状态机")] private StateMachine m_StateMachine;

        [LabelText("初始化标志")] private bool m_Initialized = false;

        public void Init(EnemyData enemyData, EnemyManager manager)
        {
            EnemyLogicMono = new EnemyLogic(enemyData);
            m_EnemyManager = manager;
            
            m_Agent = GetComponent<NavMeshAgent>();
            m_Agent.enabled = true;
            SetSpeed(EnemyLogicMono.enemyInfo.speed.value);

            InitStateMachine();

            // 事件订阅
            EnemyLogicMono.OnDie += Recycle;
            m_EnemyManager.OnTick += Tick;
            EnemyLogicMono.enemyInfo.speed.OnValueChanged += SetSpeed;
            
            m_Initialized = true;
        }

        private void InitStateMachine()
        {
            // 添加状态机组
            m_StateMachine = GetComponent<StateMachine>();
        
            // 注册状态，如果已经注册过了就什么都不会做
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
            if (!m_Initialized) return;
            EnemyLogicMono.Tick();
            m_StateMachine.Tick();
        }

        private void Recycle()
        {
            m_Initialized = false;
            m_Agent.enabled = false;
            // 取消事件订阅避免重复订阅与泄漏
            if (EnemyLogicMono != null)
            {
                EnemyLogicMono.OnDie -= Recycle;
                EnemyLogicMono.enemyInfo.speed.OnValueChanged -= SetSpeed;
            }
            if (m_EnemyManager != null)
            {
                m_EnemyManager.OnTick -= Tick;
            }
            
            // 回收到对象池
            GameObjectPool.Instance.Release(gameObject);
        }
    }
}