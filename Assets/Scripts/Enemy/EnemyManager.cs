using System;
using System.Collections.Generic;
using UnityEngine.Serialization;
using Utils;

namespace Enemy
{
    public class EnemyManager : Singleton<EnemyManager>
    {
        public List<EnemyMono> activeEnemies = new List<EnemyMono>();
        public EnemySpawner enemySpawner;
        
        public void Init()
        {
            enemySpawner.StartSpawn();
        }

        public event Action OnTick;

        public void Update()
        {
            OnTick?.Invoke();
        }
    }
}