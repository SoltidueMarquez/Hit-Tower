using UnityEngine;
using DG.Tweening;

namespace Enemy
{
    public class EnemyView : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer enemyRenderer;
        
        protected bool initialized = false;
        protected EnemyMono enemyMono;
        
        private Sequence m_HurtSequence; // 保存伤害动画序列
        private Color m_OriginalColor; // 保存原始颜色
        
        public virtual void Init(EnemyMono mono)
        {
            enemyMono = mono;
            enemyMono.enemyLogic.OnBeAttacked += HurtAnim;
            
            // 保存原始颜色
            m_OriginalColor = enemyRenderer.color;
            
            // 创建动画序列（只需创建一次）
            m_HurtSequence = DOTween.Sequence();
            
            // 变红动画（快速变红然后恢复）
            m_HurtSequence.Append(enemyRenderer.DOColor(Color.red, 0.1f)
                .SetEase(Ease.OutQuad));
            
            // 恢复原色
            m_HurtSequence.Append(enemyRenderer.DOColor(m_OriginalColor, 0.1f)
                .SetEase(Ease.InQuad));
            
            // 设置动画为可重用
            m_HurtSequence.SetAutoKill(false);
            m_HurtSequence.Pause(); // 暂停序列，等待播放
            
            initialized = true;
        }

        protected virtual void HurtAnim()
        {
            if (!initialized) return;
            
            m_HurtSequence.Restart();
        }

        /// <summary>
        /// 这是脚本的回收逻辑
        /// </summary>
        public void Recycle()
        {
            initialized = false;
            
            // 杀死动画序列
            m_HurtSequence?.Kill();
            m_HurtSequence = null;
            
            enemyMono.enemyLogic.OnBeAttacked -= HurtAnim;
        }
        
        // 可选：添加OnDestroy方法确保动画被正确清理
        protected virtual void OnDestroy()
        {
            m_HurtSequence?.Kill();
        }
    }
}