using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        public List<EnemyMono> activeEnemies = new List<EnemyMono>();
        public EnemySpawner enemySpawner;
        public event Action OnEnemyClear;
        public void Init()
        {
            enemySpawner.Init(this);
        }

        #region Tick
        public event Action OnTick;

        public void Update()
        {
            OnTick?.Invoke();
        }
        #endregion
        
        #region 敌人列表管理
        // 添加敌人到活跃列表
        public void AddEnemy(EnemyMono enemy)
        {
            if (enemy != null && !activeEnemies.Contains(enemy))
            {
                activeEnemies.Add(enemy);
            }
        }

        // 从活跃列表移除敌人
        public void RemoveEnemy(EnemyMono enemy)
        {
            if (enemy != null && activeEnemies.Contains(enemy))
            {
                activeEnemies.Remove(enemy);
            }

            if (activeEnemies.Count == 0)
            {
                OnEnemyClear?.Invoke();
            }
        }

        // 清空所有敌人（游戏结束或重置时使用）
        public void ClearAllEnemies()
        {
            foreach (var enemy in activeEnemies.Where(enemy => enemy != null))
            {
                enemy.EnemyLogicMono.SetDie();
            }

            // activeEnemies.Clear();
            OnEnemyClear?.Invoke();
        }
        #endregion

        public int GetActiveBuffNum()
        {
            return activeEnemies.Sum(x => x.EnemyLogicMono.BuffHandler.buffList.Count);
        }
    }
}