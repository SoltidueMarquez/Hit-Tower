using System.Linq;
using Buildings.Specific_Building.SingleTower.Bullet;
using Enemy;
using ObjectPool;
using UnityEngine;
using Utils.StateMachine;

namespace Buildings.Specific_Building.SingleTower.State
{
    public class SingleTowerAttackMode : IState
    {
        private StateMachine stateMachine { get; set; }
        private SingleTower m_SingleTower;
        
        private float m_AttackTimer = 0f; // 计时器字段
        
        public void SetStateMachine(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void OnEnter()
        {
            m_SingleTower.buildingView.SetRangeIndicatorVisible(true);
            m_AttackTimer = 0f;
            DoAttack();// 先进行一次攻击
        }

        public void OnTick()
        {
            if (m_SingleTower.enemiesInRange.Count == 0)
            {
                stateMachine.SwitchTo<SingleTowerIdleMode>();
            }
            
            // 一个计时器，每过攻击间隔的时间就执行一次DoAttack()
            m_AttackTimer += Time.deltaTime;
            float attackInterval = m_SingleTower.buildingLogic.buildingInfo.attackInterval.Value;
            
            if (m_AttackTimer >= attackInterval)
            {
                DoAttack();
                m_AttackTimer = 0f; // 重置计时器
            }
        }
        
        private void DoAttack()
        {
            m_SingleTower.buildingView.AtkAnim();
            var targetList = m_SingleTower.enemiesInRange.Where(target => target.isActiveAndEnabled);
            if (!m_SingleTower.buildingLogic.buildingInfo.ifSingle)
            {
                foreach (var target in targetList)
                {
                    DoSingleAtk(target);
                }
            }
            else
            {
                var enemyMonos = targetList as EnemyMono[] ?? targetList.ToArray();
                for (int i = 0; i < enemyMonos.Count(); i++)
                {
                    if (i == (int)m_SingleTower.buildingLogic.buildingInfo.attackNum.Value) break;
                    DoSingleAtk(enemyMonos[i]);
                }
            }
        }
        
        private void DoSingleAtk(EnemyMono mono)
        {
            // 攻击是创造一个子弹，并且设置目标
            var bullet =
                GameObjectPool.Instance.Get(
                        GameManager.Instance.buildingManager.builder.GetBuildingPrefabs(
                            m_SingleTower.buildingLogic.buildingInfo.buildingName)[0], m_SingleTower.transform)
                    .GetComponent<SingleBullet>();
            bullet.Init(mono, m_SingleTower);
        }

        public void OnExit()
        {
            
        }

        public void Init()
        {
            m_SingleTower = stateMachine.GetComponent<SingleTower>();
        }
    }
}