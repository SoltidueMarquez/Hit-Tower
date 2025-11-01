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
        public EnemyWaveDatas EnemyWaveDataList => enemyWaveDataList;
        
        private EnemyManager m_EnemyManager;
        private int m_CurrentWave;
        
        public event Action OnSpawnStart;
        public event Action OnSpawnEnd;
        
        public void Init(EnemyManager manager)
        {
            m_EnemyManager = manager;

            // 为了确保进度条能订阅到开始事件，这个方法必须放到后一帧执行
            StartCoroutine(StartSpawnCoroutine());
        }

        private IEnumerator StartSpawnCoroutine()
        {
            yield return null;
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
            yield return new WaitForSeconds(waveData.waitTime);
            for (int i = 0; i < waveData.singleWaveList.Count; i++)
            {
                var singleWave = waveData.singleWaveList[i];
                yield return CreateSingleWaveCoroutine(singleWave);
                if (i != waveData.singleWaveList.Count - 1) yield return new WaitForSeconds(waveData.interval);
            }
        }
        #endregion

        #region 按照波次配置生成敌人

        private IEnumerator CreateWavesCoroutine(EnemyWaveDatas waveDataList, int startIndex = 0)
        {
            // 从指定的波次索引开始遍历
            for (int waveIndex = startIndex; waveIndex < waveDataList.waveDataList.Count; waveIndex++)
            {
                // 更新当前波次索引
                m_CurrentWave = waveIndex;

                // 执行当前波次的生成逻辑
                yield return CreateWaveCoroutine(waveDataList.waveDataList[waveIndex]);

                // 可选：在这里可以添加波次结束的回调或事件
                Debug.Log($"第{waveIndex + 1}波敌人生成完成");
            }

            // 所有波次完成后的处理
            OnSpawnEnd?.Invoke();
            Debug.Log("所有波次敌人生成完成");
        }

        public void StartSpawn(int startIndex = -1)
        {
            m_CurrentWave = (startIndex == -1)
                ? 0
                : Mathf.Clamp(startIndex, 0, enemyWaveDataList.waveDataList.Count - 1);
            
            OnSpawnStart?.Invoke();
            StartCoroutine(CreateWavesCoroutine(enemyWaveDataList, m_CurrentWave));
        }

        // 获取当前波次索引
        public int GetCurrentWaveIndex()
        {
            return m_CurrentWave;
        }

        // 跳转到指定波次（重新开始生成）
        public void JumpToWave(int waveIndex)
        {
            // 先停止当前的生成协程
            StopAllCoroutines();

            // 重新开始从指定波次生成
            StartSpawn(waveIndex);
        }

        // 暂停生成
        public void PauseSpawn()
        {
            StopAllCoroutines();
        }

        // 继续生成（从当前波次继续）
        public void ResumeSpawn()
        {
            StartSpawn(m_CurrentWave);
        }

        // 判断是否生成完毕
        public bool IsSpawnOver()
        {
            return m_CurrentWave >= enemyWaveDataList.waveDataList.Count - 1;
        }
        #endregion

        private EnemySpawnerParent GetSpawner(string id)
            => spawners.FirstOrDefault(spawner => spawner.id == id);

        private EnemySpawnerParent GetRandomSpawner() => spawners[Random.Range(0, spawners.Count)];

    }

    [Serializable]
    public class EnemySpawnerParent
    {
        [LabelText("标识符")] public string id;
        [LabelText("Transform")] public Transform spawnTransform;
    }
}