using System;
using System.Collections.Generic;
using Utils;

namespace Enemy
{
    public class EnemyManager : Singleton<EnemyManager>
    {
        public List<EnemyMono> activeEnemies = new List<EnemyMono>();

        public void Init()
        {
            
        }

        public event Action OnTick;

        public void Update()
        {
            OnTick?.Invoke();
        }
    }
}