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

        #region 时间轴封装

        // 时间轴封装
        [System.Serializable]
        public class WaveTimeline
        {
            public List<WaveEvent> events = new List<WaveEvent>();
            public float totalDuration;

            [System.Serializable]
            public class WaveEvent
            {
                public int waveIndex;
                public int spawnIndex;
                public string enemyName;
                public string spawnerID;
                public float absoluteTime;
                public float relativeTime;
                public string description;
            }
        }

        // 生成时间轴
        public WaveTimeline GenerateTimeline()
        {
            WaveTimeline timeline = new WaveTimeline();
            float currentTime = 0f;

            for (int waveIndex = 0; waveIndex < waveDataList.Count; waveIndex++)
            {
                var waveData = waveDataList[waveIndex];

                // 添加波次开始事件（在waitTime之前）
                // timeline.events.Add(new WaveTimeline.WaveEvent
                // {
                //     waveIndex = waveIndex,
                //     spawnIndex = -1,
                //     enemyName = "WAVE_START",
                //     spawnerID = "ALL",
                //     absoluteTime = currentTime,
                //     relativeTime = 0,
                //     description = $"第{waveIndex + 1}波开始等待"
                // });

                // 波次开始前的等待时间
                currentTime += waveData.waitTime;

                // 添加波次真正开始事件
                timeline.events.Add(new WaveTimeline.WaveEvent
                {
                    waveIndex = waveIndex,
                    spawnIndex = -1,
                    enemyName = "WAVE_START",
                    spawnerID = "ALL",
                    absoluteTime = currentTime,
                    relativeTime = 0,
                    description = $"第{waveIndex + 1}波开始生成"
                });

                float waveStartTime = currentTime;

                for (int spawnIndex = 0; spawnIndex < waveData.singleWaveList.Count; spawnIndex++)
                {
                    var spawnData = waveData.singleWaveList[spawnIndex];
                    float spawnGroupStartTime = currentTime;
                    
                    for (int enemyIndex = 0; enemyIndex < spawnData.num; enemyIndex++)
                    {
                        // 计算当前敌人的生成时间
                        float enemySpawnTime = currentTime + (enemyIndex * spawnData.singleInterval);

                        timeline.events.Add(new WaveTimeline.WaveEvent
                        {
                            waveIndex = waveIndex,
                            spawnIndex = spawnIndex,
                            enemyName = spawnData.enemyName,
                            spawnerID = spawnData.spawnerID,
                            absoluteTime = enemySpawnTime,
                            relativeTime = enemySpawnTime - waveStartTime,
                            description =
                                $"波次{waveIndex + 1} 第{spawnIndex + 1}组 敌人{enemyIndex + 1}: \n{spawnData.enemyName}\n于生成点{spawnData.spawnerID}出现"
                        });
                    }

                    // 更新当前时间到这一组敌人生成完毕
                    currentTime += (spawnData.num - 1) * spawnData.singleInterval;

                    // 添加组间隔（除了最后一组）
                    if (spawnIndex < waveData.singleWaveList.Count - 1)
                    {
                        // 添加组间隔事件
                        timeline.events.Add(new WaveTimeline.WaveEvent
                        {
                            waveIndex = waveIndex,
                            spawnIndex = spawnIndex,
                            enemyName = "GROUP_INTERVAL",
                            spawnerID = "ALL",
                            absoluteTime = currentTime,
                            relativeTime = currentTime - waveStartTime,
                            description = $"波次{waveIndex + 1} 第{spawnIndex + 1}组最后一个敌人: \n{spawnData.enemyName}\n于生成点{spawnData.spawnerID}出现\n波次{waveIndex + 1} 第{spawnIndex + 1}组结束"
                        });

                        currentTime += waveData.interval;
                    }
                    else
                    {
                        // 最后一组结束，添加波次结束事件
                        timeline.events.Add(new WaveTimeline.WaveEvent
                        {
                            waveIndex = waveIndex,
                            spawnIndex = spawnIndex,
                            enemyName = "WAVE_END",
                            spawnerID = "ALL",
                            absoluteTime = currentTime,
                            relativeTime = currentTime - waveStartTime,
                            description = $"波次{waveIndex + 1}最后一个敌人: \n{spawnData.enemyName}\n于生成点{spawnData.spawnerID}出现\n第{waveIndex + 1}波结束"
                        });
                    }
                }

                // 更新总时间
                timeline.totalDuration = Mathf.Max(timeline.totalDuration, currentTime);
            }

            return timeline;
        }
        #endregion
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