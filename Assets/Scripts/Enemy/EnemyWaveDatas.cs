using System;
using System.Collections.Generic;
using Plugins.EditorTools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Enemy
{
    // TODO:需要实现JSON的导入导出配置
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
    }
    
    [Serializable]
    public class EnemyWaveSingle
    {
        [LabelText("敌人"), StringToEnum("Enemy")] public string enemyName;
        [LabelText("数量")] public int num;
        [LabelText("间隔时间")] public float singleInterval;
    }
}