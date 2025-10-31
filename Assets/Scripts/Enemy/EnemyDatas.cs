using System;
using System.Collections.Generic;
using System.Linq;
using Buff_System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "EnemyData",menuName = "Enemy/EnemyData")]
    public class EnemyDatas : ScriptableObject
    {
        public List<EnemyData> enemyDataList;

        public EnemyData GetEnemyData(string enemyName) =>
            enemyDataList.FirstOrDefault(data => data.enemyName == enemyName);
    }

    [Serializable]
    public class EnemyData
    {
        [LabelText("名称")] public string enemyName;
        [LabelText("血量")] public float maxHealth;
        [LabelText("速度")] public float speed;
        [LabelText("攻击力")] public float attack;
        [LabelText("护甲")] public float shield;
        [LabelText("击杀后奖励")] public float value;
        [LabelText("初始挂载的buff")] public List<BuffData> initBuffs;
        [LabelText("预制体")] public GameObject prefab;
    }
}