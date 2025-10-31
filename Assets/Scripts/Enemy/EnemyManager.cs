using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Enemy
{
    public class EnemyManager : Singleton<EnemyManager>
    {
        public List<EnemyMono> activeEnemies;

        public event Action OnTick;

        public void Update()
        {
            OnTick?.Invoke();
        }
    }
}