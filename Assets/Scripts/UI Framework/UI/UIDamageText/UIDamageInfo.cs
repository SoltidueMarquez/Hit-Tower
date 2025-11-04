using Enemy;
using ObjectPool;
using UI_Framework.Scripts;
using UnityEngine;
using Utils;

namespace UI_Framework.UI.UIDamageText
{
    public class UIDamageInfo : UIFormBase
    {
        public GameObject damageTextPrefab;
        protected override void OnInit()
        {
            Open();
        }

        public void CreateDamageText(EnemyMono mono,float damage)
        {
            var text = GameObjectPool.Instance.Get(damageTextPrefab, transform);
            // 设置位置
            UIUtils.MapWorldPositionToUI(mono.transform.position, text.transform as RectTransform);
            // 初始化操作
            text.GetComponent<DamageText>().Init(damage);
        }
    }
}