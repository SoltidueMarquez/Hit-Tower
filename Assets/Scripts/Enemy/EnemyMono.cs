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

        [LabelText("管理器")] protected EnemyManager m_EnemyManager;
        [LabelText("Nav组件")] protected NavMeshAgent m_Agent;

        [LabelText("状态机")] protected StateMachine m_StateMachine;

        [LabelText("初始化标志")] protected bool m_Initialized = false;

        public virtual void Init(EnemyData enemyData, EnemyManager manager)
        {
            EnemyLogicMono = new EnemyLogic(enemyData);
            m_EnemyManager = manager;
            
            m_Agent = GetComponent<NavMeshAgent>();
            m_Agent.enabled = true;
            SetSpeed(EnemyLogicMono.EnemyInfo.speed.value);

            InitStateMachine();
            
            // 添加到活跃敌人列表
            m_EnemyManager.AddEnemy(this);
            
            // 事件订阅
            EnemyLogicMono.OnDie += Recycle;
            m_EnemyManager.OnTick += Tick;
            EnemyLogicMono.EnemyInfo.speed.OnValueChanged += SetSpeed;
            
            m_Initialized = true;
        }

        protected virtual void InitStateMachine()
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

        protected void SetSpeed(float speed)
        {
            m_Agent.speed = speed;
        }
        
        protected void Tick()
        {
            if (!m_Initialized) return;
            EnemyLogicMono.Tick();
            m_StateMachine.Tick();
        }

        protected void Recycle()
        {
            m_Initialized = false;
            m_Agent.enabled = false;
            
            // 从管理器列表中移除
            if (m_EnemyManager != null)
            {
                m_EnemyManager.RemoveEnemy(this);
            }
            
            // 取消事件订阅避免重复订阅与泄漏
            if (EnemyLogicMono != null)
            {
                EnemyLogicMono.OnDie -= Recycle;
                EnemyLogicMono.EnemyInfo.speed.OnValueChanged -= SetSpeed;
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