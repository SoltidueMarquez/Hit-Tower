using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.StateMachine
{
    public class StateMachine : MonoBehaviour
    {
        // 存储所有注册的状态
        private Dictionary<Type, IState> m_States = new Dictionary<Type, IState>();
    
        // 当前状态和上一个状态
        private IState m_CurrentState;
        private IState m_LastState;
        
        public Type CurrentStateType { get; private set; }
        
        // 注册状态到状态机中（修改）
        public void RegisterState<T>(T state) where T : IState
        {
            var stateType = typeof(T);
            if (!m_States.ContainsKey(stateType))
            {
                // 设置状态机的引用
                state.SetStateMachine(this);
                m_States[stateType] = state;
            }
        }
    
        // 切换到指定状态类型
        public void SwitchTo<T>() where T : IState
        {
            var targetType = typeof(T);
        
            if (m_States.TryGetValue(targetType, out var newState))
            {
                // 退出当前状态
                m_CurrentState?.OnExit();
            
                // 记录上一个状态
                if (m_CurrentState != null)
                    m_LastState = m_CurrentState;
            
                // 切换状态
                m_CurrentState = newState;
                CurrentStateType = targetType;
            
                // 进入新状态
                m_CurrentState.OnEnter();
            }
        }
    
        // 返回到上一个状态
        public void ReturnToLastState()
        {
            if (m_LastState != null)
            {
                Type lastStateType = m_LastState.GetType();
                var method = typeof(StateMachine).GetMethod("SwitchTo")?.MakeGenericMethod(lastStateType);
                if (method != null) method.Invoke(this, null);
            }
        }
    
        // Unity生命周期方法
        public void Tick()
        {
            m_CurrentState?.OnTick();
        }

        // 获取特定状态实例
        public T GetState<T>() where T : class, IState
        {
            Type stateType = typeof(T);
            if (m_States.TryGetValue(stateType, out IState state))
            {
                return state as T;
            }
            return null;
        }
    
        // 检查是否处于特定状态
        public bool IsInState<T>() where T : IState
        {
            return CurrentStateType == typeof(T);
        }
    }
}