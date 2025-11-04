using ObjectPool;
using TMPro;
using UnityEngine;
using DG.Tweening;

namespace UI_Framework.UI.UIDamageText
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class DamageText : MonoBehaviour
    {
        public TextMeshProUGUI damageText;
        private Sequence m_AnimationSequence;
        
        private void Start()
        {
            
        }

        public void Init(float damage)
        {
            damageText.text = $"{damage:F1}";
            
            // 重置状态
            damageText.alpha = 1f;
            transform.localScale = Vector3.one;
            
            // 播放动画序列
            // 创建可复用的动画序列，AutoKill=false确保序列不会自动销毁
            m_AnimationSequence = DOTween.Sequence()
                .SetAutoKill(false)
                .SetRecyclable(true)
                .OnComplete(Recycle)
                .Pause();
            
            // 动画序列：缩放弹跳效果 + 上浮移动 + 淡出
            m_AnimationSequence
                .Append(transform.DOLocalMoveY(transform.localPosition.y + 10f, 0.2f).SetEase(Ease.OutCubic))
                .Join(damageText.DOFade(0f, 0.2f).SetDelay(0.1f));
            m_AnimationSequence.Restart();
        }
        
        private void Recycle()
        {
            // 回收到对象池
            GameObjectPool.Instance.Release(gameObject);
        }

        private void OnDestroy()
        {
            // 清理DOTween序列
            if (m_AnimationSequence != null)
            {
                m_AnimationSequence.Kill();
                m_AnimationSequence = null;
            }
        }
    }
}