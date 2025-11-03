using TMPro;
using UnityEngine;

namespace UI_Framework.UI.UIDebug
{
    public class DebugPanel : MonoBehaviour
    {
        public TextMeshProUGUI buffText;
        public TextMeshProUGUI enemyText;
        public TextMeshProUGUI fpsText;
        
        private int m_FrameCount = 0;
        private float m_AccumulatedTime = 0f;
        
        private void UpdateInfo()
        {
            if (!gameObject.activeSelf) return;
            
            // 累积帧时间和帧数
            m_AccumulatedTime += Time.unscaledDeltaTime;
            m_FrameCount++;
            
            // 达到更新时间间隔时计算FPS
            if (!(m_AccumulatedTime >= ConstManager.k_FPSUpdateInterval)) return;
            var fps = m_FrameCount / m_AccumulatedTime;
                
            buffText.text = $"Active Buff: {GameManager.Instance.GetActiveBuffNum()}";
            enemyText.text = $"Active Enemies: {GameManager.Instance.enemyManager.activeEnemies.Count}";
            fpsText.text = $"FPS: {fps:F0}";
                
            // 重置计数器
            m_FrameCount = 0;
            m_AccumulatedTime = 0f;
        }

        private void Update()
        {
            UpdateInfo();
        }
    }
}