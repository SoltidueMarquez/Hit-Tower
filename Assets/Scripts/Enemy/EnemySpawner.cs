using Plugins.EditorTools;
using ObjectPool;
using UnityEngine;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        public EnemyDatas enemyDataList;
        public EnemyWaveDatas enemyWaveDataList;

        private void CreateEnemy(string enemyName)
        {
            var data = enemyDataList.GetEnemyData(enemyName);
            if (data != default)
            {
                var enemy = GameObjectPool.Instance.Get(data.prefab, transform);
                // 设置位置
                enemy.transform.position = transform.position;
                
                // 初始化操作
                enemy.GetComponent<EnemyMono>().Init(data, EnemyManager.Instance);
            }
            else
            {
                Debug.LogWarning($"{enemyName}不存在");
            }
        }

        private void CreateWave()
        {
            
        }

        #region 测试部分

        [StringToEnum("Enemy")] public string eName;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CreateEnemy(eName);
            }
        }

        #endregion
    }
}