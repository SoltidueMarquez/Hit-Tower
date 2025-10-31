using System.Collections;
using System.Linq;
using ObjectPool;
using UnityEngine;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        public EnemyDatas enemyDataList;
        public EnemyWaveDatas enemyWaveDataList;

        #region 创建单个敌人
        private void CreateEnemy(string enemyName)
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
        private void CreateEnemy(EnemyData data)
        {
            var enemy = GameObjectPool.Instance.Get(data.prefab, transform);
            // 设置位置
            enemy.transform.position = transform.position;

            // 初始化操作
            enemy.GetComponent<EnemyMono>().Init(data, EnemyManager.Instance);
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
                    CreateEnemy(data);
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

        #region 测试部分

        // private void Start()
        // {
        //     StartSpawn();
        // }

        // [StringToEnum("Enemy")] public string eName;
        // private void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.Space))
        //     {
        //         CreateEnemy(eName);
        //     }
        // }

        #endregion
    }
}