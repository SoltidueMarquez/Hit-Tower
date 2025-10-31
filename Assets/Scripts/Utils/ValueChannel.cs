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

        public float value => CalculateValue();
        
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="baseValue">初始的基础值</param>
        public ValueChannel(float baseValue = 0f)
        {
            m_BaseValue = baseValue;
            m_AdditiveModifier = 0f;
            m_MultiplicativeModifier = 0f;
        }
        
        public void ModifyAdditive(float delta)
        {
            m_AdditiveModifier += delta;
        }
        
        public void ModifyMultiplier(float delta)
        {
            m_MultiplicativeModifier += delta;
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
                   $"Final Value: {value}";
        }
    }
}