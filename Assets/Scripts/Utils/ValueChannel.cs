using System.Collections.Generic;

namespace Utils
{
    /// <summary>
    /// 表示一个可被多种因子（加法、乘法）修改的数值通道。
    /// 计算顺序为：最终值 = (基础值 + 加数因子之和) * 乘数因子之积
    /// </summary>
    [System.Serializable]
    public class ValueChannel
    {
        // 基础值
        private float _baseValue;
    
        // 加法修饰符列表
        private List<float> _additiveModifiers = new List<float>();
        // 乘法修饰符列表
        private List<float> _multiplicativeModifiers = new List<float>();

        // 用于标记最终值是否需要重新计算（脏位模式优化性能）
        private bool _isDirty = true;
        // 缓存的计算结果
        private float _cachedValue;

        /// <summary>
        /// 最终计算值。如果值被标记为脏（已修改），则会重新计算。
        /// 计算顺序：FinalValue = (BaseValue + AdditiveSum) * MultiplicativeProduct
        /// </summary>
        public float Value
        {
            get
            {
                if (_isDirty)
                {
                    RecalculateValue();
                }
                return _cachedValue;
            }
        }

        /// <summary>
        /// 获取或设置基础值。设置此值会标记最终值需要重新计算。
        /// </summary>
        public float BaseValue
        {
            get => _baseValue;
            set
            {
                if (_baseValue != value)
                {
                    _baseValue = value;
                    _isDirty = true;
                }
            }
        }

        /// <summary>
        /// 获取所有加数因子的总和。
        /// </summary>
        public float AdditiveSum { get; private set; }

        /// <summary>
        /// 获取所有乘数因子的乘积。初始为1（乘法单位元）。
        /// </summary>
        public float MultiplicativeProduct { get; private set; } = 1.0f;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="baseValue">初始的基础值</param>
        public ValueChannel(float baseValue = 0f)
        {
            _baseValue = baseValue;
            RecalculateValue(); // 立即计算初始最终值
        }

        /// <summary>
        /// 添加一个加数因子。
        /// </summary>
        /// <param name="additive">要添加的加数</param>
        public void AddAdditive(float additive)
        {
            _additiveModifiers.Add(additive);
            _isDirty = true;
        }

        /// <summary>
        /// 移除一个加数因子。
        /// </summary>
        /// <param name="additive">要移除的加数</param>
        /// <returns>如果成功找到并移除返回true，否则返回false</returns>
        public bool RemoveAdditive(float additive)
        {
            bool removed = _additiveModifiers.Remove(additive);
            if (removed)
            {
                _isDirty = true;
            }
            return removed;
        }

        /// <summary>
        /// 添加一个乘数因子。
        /// </summary>
        /// <param name="multiplier">要添加的乘数</param>
        public void AddMultiplier(float multiplier)
        {
            _multiplicativeModifiers.Add(multiplier);
            _isDirty = true;
        }

        /// <summary>
        /// 移除一个乘数因子。
        /// </summary>
        /// <param name="multiplier">要移除的乘数</param>
        /// <returns>如果成功找到并移除返回true，否则返回false</returns>
        public bool RemoveMultiplier(float multiplier)
        {
            bool removed = _multiplicativeModifiers.Remove(multiplier);
            if (removed)
            {
                _isDirty = true;
            }
            return removed;
        }

        /// <summary>
        /// 清除所有加数因子和乘数因子，重置为基础值。
        /// </summary>
        public void ClearAllModifiers()
        {
            _additiveModifiers.Clear();
            _multiplicativeModifiers.Clear();
            _isDirty = true;
        }

        /// <summary>
        /// 强制重新计算最终值。
        /// 内部使用，通常在获取Value属性时自动调用。
        /// </summary>
        private void RecalculateValue()
        {
            // 计算加法和
            AdditiveSum = 0f;
            foreach (float modifier in _additiveModifiers)
            {
                AdditiveSum += modifier;
            }

            // 计算乘法积
            MultiplicativeProduct = 1.0f; // 乘法单位元
            foreach (float modifier in _multiplicativeModifiers)
            {
                MultiplicativeProduct *= modifier;
            }

            // 最终计算: (基础值 + 加法总和) * 乘法总积
            _cachedValue = (_baseValue + AdditiveSum) * MultiplicativeProduct;
            _isDirty = false; // 数据已更新，清除脏位
        }

        /// <summary>
        /// 获取当前所有修饰符的详细信息字符串，用于调试。
        /// </summary>
        public string GetDebugInfo()
        {
            return $"Base: {BaseValue}, Additives: [{string.Join(", ", _additiveModifiers)}] (Sum: {AdditiveSum}), " +
                   $"Multipliers: [{string.Join(", ", _multiplicativeModifiers)}] (Product: {MultiplicativeProduct}), " +
                   $"Final Value: {Value}";
        }
    }
}