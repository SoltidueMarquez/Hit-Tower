using System.Collections.Generic;
using System.Linq;
using Enemy;
using ObjectPool;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils.StateMachine;

namespace Buildings.Specific_Building.SingleTower.Bullet
{
    [RequireComponent(typeof(StateMachine), typeof(Collider), typeof(Rigidbody))]
    public class SingleBullet : MonoBehaviour
    {
        [SerializeField, LabelText("最大存活时间")] public float maxAliveTime;
        [SerializeField, LabelText("速度")] public float speed;
        [SerializeField] private StateMachine stateMachine;

        [HideInInspector] public bool hasHit;
        
        [LabelText("最大碰撞次数")] private int m_MaxHitNum;
        [LabelText("当前碰撞次数")] private int m_CurHitNum;

        private SingleTower m_FromTower;

        private readonly HashSet<EnemyMono> m_Targets = new HashSet<EnemyMono>();

        private bool m_Initialized = false;

        private EnemyMono m_FirstTarget;
        private Vector2 m_Dir;
        

        #region 初始化

        public void Init(EnemyMono target, SingleTower from)
        {
            SetYPos(); // 约束y坐标
            
            m_FromTower = from;
            m_Targets.Clear();

            m_FirstTarget = target;
            m_Dir = new Vector2(target.transform.position.x - transform.position.x,
                target.transform.position.y - transform.position.y);

            // 穿透次数，可穿透就>0
            m_MaxHitNum = (int)from.penetrateNum.Value + 1;
            m_CurHitNum = 0;
            
            hasHit = false;

            InitStateMachine();

            m_Initialized = true;
        }

        private void InitStateMachine()
        {
            stateMachine.ClearStates();
            // 注册状态，如果已经注册过了就什么都不会做
            stateMachine.RegisterState(new BulletFlyState());
            stateMachine.RegisterState(new BulletAttackState());
            // stateMachine.RegisterState(new BulletTrackState());

            // 设置初始状态
            stateMachine.SwitchTo<BulletFlyState>();
            // stateMachine.SwitchTo<BulletTrackState>();
        }

        #endregion

        private void FixedUpdate()
        {
            if (!m_Initialized) return;

            stateMachine.Tick();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                var target = other.GetComponentInParent<EnemyMono>();
                if(target!=null)
                {
                    m_Targets.Add(target);
                    hasHit = true;
                }
            }

            Debug.Log(hasHit);
        }

        public void MoveToward()
        {
            if (m_FirstTarget.isActiveAndEnabled && m_CurHitNum == 0)
            {
                m_Dir = new Vector2(m_FirstTarget.transform.position.x - transform.position.x,
                    m_FirstTarget.transform.position.z - transform.position.z);
            }
            // 朝向目标在xz平面上移动
            Vector3 direction = new Vector3(m_Dir.x, 0, m_Dir.y).normalized;
            Vector3 movement = direction * (speed * Time.fixedDeltaTime);
    
            // 直接修改位置
            transform.position += movement;
        }

        public void DoSputterCheckAndUpdateTargets()
        {
            // TODO：溅射的处理
            // 溅射范围，可溅射就>0
            // var sputterRadius = m_FromTower.sputterRadius.Value;
        }

        private void SetYPos()
        {
            // 约束y坐标
            transform.position = new Vector3(transform.position.x, ConstManager.k_PreviewYOffset, transform.position.z);
        }

        public void UpdateHitAndCheckRecycle()
        {
            m_CurHitNum++;
            if (m_CurHitNum >= m_MaxHitNum)
            {
                Recycle();
            }
        }

        public void HurtEnemy()
        {
            foreach (var mono in m_Targets.Where(mono => mono.isActiveAndEnabled))
            {
                mono.enemyLogic.ModifyCurrentHealth(-m_FromTower.buildingLogic.buildingInfo.attack.Value);
            }

            m_Targets.Clear();
        }

        public void Recycle()
        {
            m_Initialized = false;
            GameObjectPool.Instance.Release(gameObject);
        }
    }
}