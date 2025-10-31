using Plugins.EditorTools;
using UnityEngine;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        public EnemyDatas enemyDataList;

        private void CreateEnemy(string enemyName)
        {
            var data = enemyDataList.GetEnemyData(enemyName);
            if (data != default)
            {
                var enemy = Instantiate(data.prefab, transform);
                // 初始化操作
                enemy.GetComponent<EnemyMono>().Init(data, EnemyManager.Instance);
            }
            else
            {
                Debug.LogWarning($"{enemyName}不存在");
            }
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