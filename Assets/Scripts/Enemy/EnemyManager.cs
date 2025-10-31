using System;
using UnityEngine;

namespace Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        public event Action OnTick;

        public void Update()
        {
            OnTick?.Invoke();
        }
    }
}