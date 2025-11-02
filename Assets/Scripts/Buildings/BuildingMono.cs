using Buildings.Specific_Building.Default.State;
using ObjectPool;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils.StateMachine;

namespace Buildings
{
    [RequireComponent(typeof(StateMachine),typeof(BuildingView))]
    public class BuildingMono : MonoBehaviour
    {
        [LabelText("逻辑类"), HideInInspector] public BuildingLogic buildingLogic;
        
        [LabelText("管理器")] protected BuildingManager buildingManager;
        [LabelText("表现层")] protected BuildingView buildingView;
        [LabelText("状态机")] protected StateMachine stateMachine;
        [LabelText("初始化标志")] protected bool initialized = false;

        public virtual void Init(BuildingData buildingData,BuildingManager manager)
        {
            buildingLogic = new BuildingLogic(buildingData, gameObject);
            buildingManager = manager;
            buildingView = GetComponent<BuildingView>();
            buildingView.Init(this);
                
            InitStateMachine();
            
            // 添加到活跃敌人列表
            buildingManager.AddBuilding(this);
            
            // 事件订阅
            buildingLogic.OnDie += Recycle;
            buildingManager.onTick += Tick;

            initialized = true;
        }
        
        protected virtual void InitStateMachine()
        {
            // 添加状态机组
            stateMachine = GetComponent<StateMachine>();

            stateMachine.ClearStates();
            // 注册状态，如果已经注册过了就什么都不会做
            stateMachine.RegisterState(new BuildingDefaultIdleState());
            stateMachine.RegisterState(new BuildingDefaultAttackState());

            // 设置初始状态
            stateMachine.SwitchTo<BuildingDefaultIdleState>();
        }
        
        protected void Tick()
        {
            if (!initialized) return;
            buildingLogic.Tick();
            stateMachine.Tick();
            buildingView.Tick();
        }
        
        protected void Recycle()
        { 
            initialized = false;
            
            // 从管理器列表中移除
            if (buildingManager != null)
            {
                buildingManager.RemoveBuilding(this);
            }
            
            // 取消事件订阅避免重复订阅与泄漏
            if (buildingLogic != null)
            {
                buildingLogic.OnDie -= Recycle;
            }
            if (buildingManager != null)
            {
                buildingManager.onTick -= Tick;
            }

            if (buildingView != null)
            {
                buildingView.Recycle();
            }
            
            // 回收到对象池
            GameObjectPool.Instance.Release(gameObject);
        }
    }
}