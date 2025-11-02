using System;
using System.Collections.Generic;
using Buildings.Specific_Building.Default.State;
using Enemy;
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

        public List<EnemyMono> enemiesInRange;
        
        public virtual void Init(BuildingData buildingData,BuildingManager manager)
        {
            buildingLogic = new BuildingLogic(buildingData, gameObject);
            buildingManager = manager;
            buildingView = GetComponent<BuildingView>();
            buildingView.Init(this);
                
            InitStateMachine();
            
            // 添加到塔的列表
            buildingManager.AddBuilding(this);
            enemiesInRange = new List<EnemyMono>();
            
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
            // buildingView.Tick();
        }
        
        protected void Recycle()
        { 
            initialized = false;
            enemiesInRange.Clear();
            
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

        #region 升级封装
        public bool CheckUpgrade()
        {
            if (buildingLogic.buildingInfo.CheckIfMaxLv())
            {
                Debug.LogWarning("已经升满级了");
                return false;
            }
            // 检查钱够不够
            if (!GameManager.Instance.playerManager.CheckCanSpent(buildingLogic.buildingInfo.upgradeCost))
            {
                Debug.LogWarning("钱不够");
                return false;
            }

            return true;
        }
        
        public void UpGrade()
        {
            if (CheckUpgrade())
            {
                GameManager.Instance.playerManager.playerLogic.ModifyMoney(-buildingLogic.buildingInfo.upgradeCost);
                buildingLogic.LevelUp();
            }
        }
        #endregion

        #region Debug

        private void OnDrawGizmos()
        {
            // 如果未初始化，则不绘制
            if (!initialized) return;
    
            // 获取攻击范围
            float attackRange = buildingLogic.buildingInfo.attackRange.Value;
    
            // 设置Gizmos颜色
            Gizmos.color = Color.blue;
    
            // 绘制攻击范围圆环（XZ平面）
            Vector3 center = transform.position;
    
            // 绘制圆环
            int segments = 36; // 分段数，越多越圆滑
            float angle = 0f;
            Vector3 prevPoint = center + new Vector3(Mathf.Cos(angle) * attackRange, 0, Mathf.Sin(angle) * attackRange);
    
            for (int i = 1; i <= segments; i++)
            {
                angle = (i / (float)segments) * 2 * Mathf.PI;
                Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle) * attackRange, 0, Mathf.Sin(angle) * attackRange);
                Gizmos.DrawLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }
        }

        #endregion
    }
}