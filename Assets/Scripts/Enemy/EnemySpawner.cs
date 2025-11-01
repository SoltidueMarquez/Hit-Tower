using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ObjectPool;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField, LabelText("敌人信息")] private EnemyDatas enemyDataList;
        [SerializeField, LabelText("波次信息")] private  EnemyWaveDatas enemyWaveDataList;
        [SerializeField, LabelText("出怪口")] private List<EnemySpawnerParent> spawners;
        private EnemyManager m_EnemyManager;
        public void Init(EnemyManager manager)
        {
            m_EnemyManager = manager;
            StartSpawn();
        }
        
        #region 创建单个敌人
        private void CreateEnemy(string enemyName, string spawnerID = default)
        {
            var data = enemyDataList.GetEnemyData(enemyName);
            if (data != default)
            {
                CreateEnemy(data);
            }
            else
            {
                Debug.LogWarning($"{enemyName}不存在");
            }
        }

        private void CreateEnemy(EnemyData data, string spawnerID = default)
        {
            var spawner = spawnerID != default ? GetSpawner(spawnerID) : GetRandomSpawner();
            if (spawner == null)
            {
                Debug.LogWarning("找不到出怪口"); 
                return;
            }
            var enemy = GameObjectPool.Instance.Get(data.prefab, spawner.spawnTransform);
            // 设置位置
            enemy.transform.position = spawner.spawnTransform.position;

            // 初始化操作
            enemy.GetComponent<EnemyMono>().Init(data, m_EnemyManager);
        }
        #endregion

        #region 创建单个小波次敌人
        private IEnumerator CreateSingleWaveCoroutine(EnemyWaveSingle singleWave)
        {
            var data = enemyDataList.GetEnemyData(singleWave.enemyName);
            if (data != default)
            {
                for (int i = 0; i < singleWave.num; i++)
                {
                    CreateEnemy(data, singleWave.spawnerID);
                    if (i == singleWave.num - 1) yield break;
                    yield return new WaitForSeconds(singleWave.singleInterval);
                }
            }
            else
            {
                yield break;
            }
        }
        #endregion

        #region 生成一波敌人
        private IEnumerator CreateWaveCoroutine(EnemyWaveData waveData)
        {
            foreach (var singleWave in waveData.singleWaveList)
            {
                yield return CreateSingleWaveCoroutine(singleWave);
                yield return new WaitForSeconds(waveData.interval);
            }
            yield return new WaitForSeconds(waveData.waitTime);
        }
        #endregion

        #region 按照波次配置生成敌人
        private IEnumerator CreateWavesCoroutine(EnemyWaveDatas waveDataList)
        {
            return waveDataList.waveDataList.Select(CreateWaveCoroutine).GetEnumerator();
        }

        public void StartSpawn()
        {
            StartCoroutine(CreateWavesCoroutine(enemyWaveDataList));
        }
        #endregion

        private EnemySpawnerParent GetSpawner(string id)
            => spawners.FirstOrDefault(spawner => spawner.id == id);

        private EnemySpawnerParent GetRandomSpawner()
        {
            return spawners[Random.Range(0, spawners.Count)];
        }
    }

    [Serializable]
    public class EnemySpawnerParent
    {
        [LabelText("标识符")] public string id;
        [LabelText("Transform")] public Transform spawnTransform;
    }
}