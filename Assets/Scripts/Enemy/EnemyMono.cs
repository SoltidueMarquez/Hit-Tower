using Sirenix.OdinInspector;
using UnityEngine;

namespace Enemy
{
    public class EnemyMono : MonoBehaviour
    {
        [LabelText("逻辑类")] private EnemyLogic m_EnemyLogic;
        [LabelText("管理器")] private EnemyManager m_EnemyManager;

        public void Init(EnemyData enemyData, EnemyManager manager)
        {
            m_EnemyLogic = new EnemyLogic(enemyData);
            m_EnemyManager = manager;
            
            // 事件订阅
            m_EnemyLogic.OnDie += Recycle;
            m_EnemyManager.OnTick += Tick;
        }

        private void Tick()
        {
            m_EnemyLogic.Tick();
        }
        
        // TODO:对象池
        private void Recycle()
        {
            
        }
    }
}