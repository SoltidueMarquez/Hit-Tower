using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Buildings.AoeTower
{
    public class AoeTowerView : BuildingView
    {
        [LabelText("攻击范围指示器"), SerializeField] private SpriteRenderer atkIndicator;
        
        private Sequence m_AttackSequence; // 保存动画序列，用于控制播放和打断

        public override void AtkAnim()
        {
            // 如果已有动画在播放，先杀死它（实现可打断）
            m_AttackSequence?.Kill();
            
            // 重置冲击波状态
            atkIndicator.transform.localScale = Vector3.zero;
            atkIndicator.gameObject.SetActive(true);
            
            // 计算目标缩放大小
            float targetScale = CalculateScale();
            
            // 创建新的动画序列（实现可重复播放）
            m_AttackSequence = DOTween.Sequence();
            
            // 冲击波放大动画
            m_AttackSequence.Append(atkIndicator.transform.DOScale(new Vector3(targetScale, targetScale, 1f), 0.2f)
                .SetEase(Ease.OutQuad));
            
            // 可选：添加淡出效果
            m_AttackSequence.Join(atkIndicator.DOFade(0f, 0.2f)
                .From(1f)
                .SetEase(Ease.OutQuad));
            
            // 动画完成后隐藏冲击波
            m_AttackSequence.OnComplete(() =>
            {
                atkIndicator.gameObject.SetActive(false);
                // 重置状态以便下次播放
                atkIndicator.transform.localScale = Vector3.zero;
                atkIndicator.color = new Color(atkIndicator.color.r, atkIndicator.color.g, atkIndicator.color.b, 1f);
            });
            
            // 开始播放动画
            m_AttackSequence.Play();
        }

        private float CalculateScale()
        {
            if (atkIndicator == null || atkIndicator.sprite == null) return 0f;
        
            // 获取攻击范围
            float attackRange = buildingMono.buildingLogic.buildingInfo.attackRange.Value;
        
            // 获取精灵的原始大小（世界单位）
            float spriteSize = atkIndicator.sprite.bounds.size.x;
        
            // 计算缩放比例：攻击范围直径 / 精灵原始大小
            return (attackRange * 2) / spriteSize;
        }
        
        // 可选：添加清理方法，在对象销毁时杀死动画
        protected virtual void OnDestroy()
        {
            m_AttackSequence?.Kill();
        }
    }
}