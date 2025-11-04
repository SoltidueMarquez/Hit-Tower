using System.Linq;
using Buff_System;
using Enemy;
using UnityEngine;
using IState = Utils.StateMachine.IState;
using StateMachine = Utils.StateMachine.StateMachine;

namespace Buildings.Specific_Building.AOETower.State
{
    public class AoeTowerAttackState : IState
    {
        private StateMachine stateMachine { get; set; }
        private BuildingMono m_Building;

        private float m_AttackTimer = 0f; // 计时器字段
        
        public void SetStateMachine(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void OnEnter()
        {
            m_Building.buildingView.SetRangeIndicatorVisible(true);
            m_AttackTimer = 0f;
            DoAttack();// 先进行一次攻击
        }

        public void OnTick()
        {
            if (m_Building.enemiesInRange.Count == 0)
            {
                stateMachine.SwitchTo<AoeTowerIdleState>();
            }
            
            // 一个计时器，每过攻击间隔的时间就执行一次DoAttack()
            m_AttackTimer += Time.deltaTime;
            float attackInterval = m_Building.buildingLogic.buildingInfo.attackInterval.Value;
            
            if (m_AttackTimer >= attackInterval)
            {
                DoAttack();
                m_AttackTimer = 0f; // 重置计时器
            }
        }

        private void DoAttack()
        {
            m_Building.buildingView.AtkAnim();
            var targetList = m_Building.enemiesInRange.Where(target => target.isActiveAndEnabled);
            if (!m_Building.buildingLogic.buildingInfo.ifSingle)
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
                    if (i == (int)m_Building.buildingLogic.buildingInfo.attackNum.Value) break;
                    DoSingleAtk(enemyMonos[i]);
                }
            }
        }

        private void DoSingleAtk(EnemyMono mono)
        {
            // 攻击
            mono.EnemyLogicMono.ModifyCurrentHealth(-m_Building.buildingLogic.buildingInfo.attack.Value);
            // 加减速buff
            mono.EnemyLogicMono.BuffHandler.AddBuff(new BuffInfo(GameManager.Instance.buffManager.GetBuffData(1),m_Building.gameObject,mono.gameObject));
        }

        public void OnExit()
        {
            
        }

        public void Init()
        {
            m_Building = stateMachine.GetComponent<BuildingMono>();
        }
    }
}