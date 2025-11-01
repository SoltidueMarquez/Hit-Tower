using System;
using System.Collections.Generic;
using Plugins.EditorTools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "EnemyWaveData",menuName = "Enemy/EnemyWaveData")]
    public class EnemyWaveDatas : ScriptableObject
    {
        public List<EnemyWaveData> waveDataList = new List<EnemyWaveData>();
    }

    [Serializable]
    public class EnemyWaveData
    {
        public List<EnemyWaveSingle> singleWaveList = new List<EnemyWaveSingle>();
        [LabelText("间隔时间")] public float interval;
        [LabelText("结束的等待时间")] public float waitTime;
    }
    
    [Serializable]
    public class EnemyWaveSingle
    {
        [LabelText("敌人"), StringToEnum("Enemy")] public string enemyName;
        [LabelText("数量")] public int num;
        [LabelText("间隔时间")] public float singleInterval;
        [LabelText("出怪口"), StringToEnum("Enemy Spawner")] public string spawnerID;
    }
}