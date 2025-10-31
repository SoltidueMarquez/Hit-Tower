using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Enemy
{
    public class EnemyDatas : ScriptableObject
    {
        public List<EnemyData> enemyDataList;
    }

    [Serializable]
    public class EnemyData
    {
        [LabelText("名称")] public string enemyName;
        [LabelText("血量")] public float maxHealth;
        [LabelText("速度")] public float speed;
        [LabelText("击杀后奖励")] public float value;
    }
}