using System;
using UnityEngine;

namespace Utils
{
    [System.Serializable]
    public class ValueChannel
    {
        // 基础值
        private float m_BaseValue;
        // 加法修饰符
        private float m_AdditiveModifier;
        // 乘法修饰符
        private float m_MultiplicativeModifier;

        public float Value => CalculateValue();
        public event Action<float> OnValueChanged;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="baseValue">初始的基础值</param>
        public ValueChannel(float baseValue = 0f)
        {
            m_BaseValue = baseValue;
            m_AdditiveModifier = 0f;
            m_MultiplicativeModifier = 1f;
        }
        
        public void ModifyAdditive(float delta)
        {
            var original = m_AdditiveModifier;
            m_AdditiveModifier += delta;
            if (Mathf.Approximately(original, m_AdditiveModifier)) OnValueChanged?.Invoke(Value);
        }
        
        public void ModifyMultiplier(float delta)
        {
            var original = m_MultiplicativeModifier;
            m_MultiplicativeModifier += delta;
            if (Mathf.Approximately(original, m_MultiplicativeModifier)) OnValueChanged?.Invoke(Value);
        }

        public void SetBaseValue(float newValue)
        {
            m_BaseValue = newValue;
            OnValueChanged?.Invoke(Value);
        }
        
        /// <summary>
        /// 最终计算: (基础值 + 加法总和) * 乘法总积
        /// </summary>
        /// <returns></returns>
        private float CalculateValue()
        {
            return (m_BaseValue + m_AdditiveModifier) * m_MultiplicativeModifier;
        }

        /// <summary>
        /// 获取当前所有修饰符的详细信息字符串，用于调试。
        /// </summary>
        public string GetDebugInfo()
        {
            return $"Base: {m_BaseValue}, Additives: {m_AdditiveModifier} " +
                   $"Multipliers: {m_MultiplicativeModifier}, " +
                   $"Final Value: {Value}";
        }
    }
}