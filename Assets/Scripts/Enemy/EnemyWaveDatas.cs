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
        [LabelText("生成前的等待时间")] public float waitTime;
        public List<EnemyWaveSingle> singleWaveList = new List<EnemyWaveSingle>();
        [LabelText("间隔时间")] public float interval;
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