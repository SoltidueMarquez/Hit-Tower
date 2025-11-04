using System;
using System.Collections.Generic;
using System.Linq;
using Buff_System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy
{
    [CreateAssetMenu(fileName = "EnemyData",menuName = "Enemy/EnemyData")]
    public class EnemyDatas : ScriptableObject
    {
        public List<EnemyData> enemyDataList = new List<EnemyData>();

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
        [LabelText("伤害吸收倍率"), Range(0.1f, 2f)] public float atkAbsorbPercent = 1;
        [LabelText("击杀后奖励")] public float value;
        [LabelText("初始挂载的buff")] public List<BuffData> initBuffs;
        [LabelText("预制体")] public GameObject prefab;
    }
}