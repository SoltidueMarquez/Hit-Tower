using Enemy.State;
using ObjectPool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Utils.StateMachine;

namespace Enemy
{
    [RequireComponent(typeof(StateMachine), typeof(NavMeshAgent),typeof(EnemyView))]
    public class EnemyMono : MonoBehaviour
    {
        [LabelText("逻辑类")] public EnemyLogic enemyLogic { get; private set; }
        
        [LabelText("表现层")] public EnemyView enemyView { get; private set; }
        
        [LabelText("管理器")] protected EnemyManager enemyManager;
        [LabelText("Nav组件")] protected NavMeshAgent agent;

        [LabelText("状态机")] protected StateMachine stateMachine;

        [LabelText("初始化标志")] protected bool initialized = false;

        public virtual void Init(EnemyData enemyData, EnemyManager manager)
        {
            enemyLogic = new EnemyLogic(enemyData, gameObject);
            enemyManager = manager;
            
            enemyView = GetComponent<EnemyView>();
            enemyView.Init(this);
            
            agent = GetComponent<NavMeshAgent>();
            agent.enabled = true;
            SetSpeed(enemyLogic.EnemyInfo.speed.Value);

            InitStateMachine();
            
            // 添加到活跃敌人列表
            enemyManager.AddEnemy(this);
            
            // 事件订阅
            enemyLogic.OnDie += Recycle;
            enemyManager.OnTick += Tick;
            enemyLogic.EnemyInfo.speed.OnValueChanged += SetSpeed;
            
            initialized = true;
        }

        protected virtual void InitStateMachine()
        {
            // 添加状态机组
            stateMachine = GetComponent<StateMachine>();
            stateMachine.ClearStates();
            // 注册状态，如果已经注册过了就什么都不会做
            stateMachine.RegisterState(new IdleState());
            stateMachine.RegisterState(new MoveState());
            stateMachine.RegisterState(new AttackState());
        
            // 设置初始状态
            stateMachine.SwitchTo<IdleState>();
        }

        protected void SetSpeed(float delta)
        {
            agent.speed = enemyLogic.EnemyInfo.speed.Value;
        }
        
        protected void Tick()
        {
            if (!initialized) return;
            enemyLogic.Tick();
            stateMachine.Tick();
        }

        protected void Recycle()
        {
            initialized = false;
            agent.enabled = false;
            
            // 从管理器列表中移除
            if (enemyManager != null)
            {
                enemyManager.RemoveEnemy(this);
            }
            
            // 取消事件订阅避免重复订阅与泄漏
            if (enemyLogic != null)
            {
                enemyLogic.OnDie -= Recycle;
                enemyLogic.EnemyInfo.speed.OnValueChanged -= SetSpeed;
            }
            if (enemyManager != null)
            {
                enemyManager.OnTick -= Tick;
            }
            
            if (enemyView != null)
            {
                enemyView.Recycle();
            }
            
            // 回收到对象池
            GameObjectPool.Instance.Release(gameObject);
        }
    }
}