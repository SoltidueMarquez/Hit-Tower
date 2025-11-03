using System;
using System.Collections.Generic;
using System.Linq;
using Enemy;
using UnityEngine;

namespace Buildings
{
    public class BuildingManager : MonoBehaviour
    {
        public List<BuildingMono> activeBuildings;
        public List<PlacementCell> placementCells;
        public BuildingBuilder builder;
        public BuildingViewModelHelper buildingViewModelHelper;
        
        // 定时更新相关变量
        private float m_UpdateTimer = 0f;
        
        
        public void Init()
        {
            activeBuildings = new List<BuildingMono>();
            
            // 查找场景中所有的PlacementCell组件（包括挂载在失活物体上的）
            placementCells = FindObjectsByType<PlacementCell>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();

            if (builder != null)
            {
                builder.Init(this);
            }
            else
            {
                Debug.LogError("BuildingBuilder 未分配！");
            }

            buildingViewModelHelper = new BuildingViewModelHelper();
        }
        
        #region Tick
        public event Action onTick;

        public void Update()
        {
            onTick?.Invoke();
            
            // 定时更新建筑物敌人列表
            m_UpdateTimer += Time.deltaTime;
            if (m_UpdateTimer >= ConstManager.k_UpdateInterval)
            {
                m_UpdateTimer = 0f;
                UpdateBuildingsEnemyList();
            }
        }
        #endregion
        
        #region 建筑物列表管理
        public void AddBuilding(BuildingMono building)
        {
            if (building != null && !activeBuildings.Contains(building))
            {
                activeBuildings.Add(building);
            }
        }
        
        public void RemoveBuilding(BuildingMono building)
        {
            if (building != null && activeBuildings.Contains(building))
            {
                activeBuildings.Remove(building);
            }
        }
        
        public void ClearAllBuildings()
        {
            foreach (var building in activeBuildings.Where(building => building != null))
            {
                if (building.buildingLogic != null)
                {
                    building.buildingLogic.SetDie();
                }
            }
        }
        #endregion
        
        public int GetActiveBuffNum()
        {
            return activeBuildings.Sum(x => x.buildingLogic.BuffHandler.buffList.Count);
        }

        #region 建造封装
        public bool TryBuild(string buildingName, Transform placementCell, out BuildingMono buildingMono)
        {
            buildingMono = null;
            
            if (builder == null)
            {
                Debug.LogError("BuildingBuilder 未分配！");
                return false;
            }
            
            if (GameManager.Instance == null || GameManager.Instance.playerManager == null || 
                GameManager.Instance.playerManager.playerLogic == null)
            {
                Debug.LogError("GameManager 或 PlayerManager 未初始化！");
                return false;
            }

            if (builder.GetBuildCost(buildingName, out var cost))
            {
                if (GameManager.Instance.playerManager.playerLogic.ModifyMoney(-cost))
                {
                    buildingMono = builder.CreateBuilding(buildingName, placementCell);
                    return true;
                }
                else
                {
                    Debug.LogWarning($"货币不足{buildingName}");
                    return false;
                }
            }
            else
            {
                Debug.LogWarning($"找不到建筑物{buildingName}");
                return false;
            }
        }
        #endregion

        #region 更新建筑物的范围敌人列表
        // 思路是使用computerShader让每个塔遍历敌人列表计算距离，距离小于攻击范围就加进列表中
        [SerializeField] private ComputeShader buildingEnemyRangeShader;
        private const string k_KernelName = "CalculateEnemyInRange";
        private int m_KernelIndex = -1;
        private ComputeBuffer m_BuildingPositionsBuffer;
        private ComputeBuffer m_BuildingRangesBuffer;
        private ComputeBuffer m_EnemyPositionsBuffer;
        private ComputeBuffer m_EnemyInRangeMatrixBuffer;

        private void UpdateBuildingsEnemyList()
        {
            if (activeBuildings.Count == 0 || activeBuildings == null) return;
            if (GameManager.Instance.enemyManager.activeEnemies.Count == 0)// 如果没有敌人
            {
                foreach (var buildingMono in activeBuildings)
                {
                    buildingMono.enemiesInRange.Clear();
                }
                return;
            }
            try
            {
                if (buildingEnemyRangeShader == null)
                {
                    // 回退到CPU计算
                    FallbackToCPUCalculation();
                    return;
                }

                // 安全检查GameManager相关组件
                if (GameManager.Instance == null || GameManager.Instance.enemyManager == null)
                {
                    ClearAllBuildingEnemyLists();
                    return;
                }

                // 使用更安全的空值检查
                var buildings = activeBuildings
                    .Where(b => b != null && b.gameObject != null && b.buildingLogic != null && 
                               b.buildingLogic.buildingInfo != null)
                    .ToList();
                    
                var enemies = (GameManager.Instance.enemyManager.activeEnemies ?? new List<EnemyMono>())
                    .Where(e => e != null && e.gameObject != null)
                    .ToList();

                // 如果建筑物或敌人数量为0，清空所有建筑物的敌人列表
                if (buildings.Count == 0 || enemies.Count == 0)
                {
                    ClearAllBuildingEnemyLists(buildings);
                    return;
                }

                int buildingCount = buildings.Count;
                int enemyCount = enemies.Count;
                int totalElements = buildingCount * enemyCount;

                // 检查数量是否合理
                if (buildingCount <= 0 || enemyCount <= 0 || totalElements <= 0)
                {
                    ClearAllBuildingEnemyLists(buildings);
                    return;
                }

                // 初始化 kernel 索引
                if (m_KernelIndex == -1)
                {
                    try
                    {
                        m_KernelIndex = buildingEnemyRangeShader.FindKernel(k_KernelName);
                        if (m_KernelIndex < 0)
                        {
                            Debug.LogError($"找不到ComputeShader kernel: {k_KernelName}");
                            FallbackToCPUCalculation(buildings, enemies);
                            return;
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"初始化ComputeShader失败: {e.Message}");
                        FallbackToCPUCalculation(buildings, enemies);
                        return;
                    }
                }

                // 准备输入数据
                Vector2[] buildingPositions = new Vector2[buildingCount];
                float[] buildingRanges = new float[buildingCount];
                Vector2[] enemyPositions = new Vector2[enemyCount];

                // 填充建筑物数据（带安全检查）
                for (int i = 0; i < buildingCount; i++)
                {
                    if (buildings[i] == null || buildings[i].transform == null) continue;
                    
                    Vector3 pos = buildings[i].transform.position;
                    buildingPositions[i] = new Vector2(pos.x, pos.z);
                    
                    if (buildings[i].buildingLogic != null && buildings[i].buildingLogic.buildingInfo != null)
                    {
                        buildingRanges[i] = buildings[i].buildingLogic.buildingInfo.attackRange.Value;
                    }
                    else
                    {
                        buildingRanges[i] = 0f; // 默认值
                    }
                }

                // 填充敌人数据（带安全检查）
                for (int i = 0; i < enemyCount; i++)
                {
                    if (enemies[i] == null || enemies[i].transform == null) continue;
                    
                    Vector3 pos = enemies[i].transform.position;
                    enemyPositions[i] = new Vector2(pos.x, pos.z);
                }

                // 创建或更新 ComputeBuffer
                const int buildingPositionsSize = 8; // Vector2 = 2 * float = 8 bytes
                const int floatSize = 4; // float = 4 bytes
                const int intSize = 4; // int = 4 bytes

                // 建筑物位置Buffer
                if (m_BuildingPositionsBuffer == null || m_BuildingPositionsBuffer.count != buildingCount)
                {
                    m_BuildingPositionsBuffer?.Release();
                    m_BuildingPositionsBuffer = new ComputeBuffer(buildingCount, buildingPositionsSize);
                }

                // 建筑物范围Buffer
                if (m_BuildingRangesBuffer == null || m_BuildingRangesBuffer.count != buildingCount)
                {
                    m_BuildingRangesBuffer?.Release();
                    m_BuildingRangesBuffer = new ComputeBuffer(buildingCount, floatSize);
                }

                // 敌人位置Buffer
                if (m_EnemyPositionsBuffer == null || m_EnemyPositionsBuffer.count != enemyCount)
                {
                    m_EnemyPositionsBuffer?.Release();
                    m_EnemyPositionsBuffer = new ComputeBuffer(enemyCount, buildingPositionsSize);
                }

                // 结果矩阵Buffer
                if (m_EnemyInRangeMatrixBuffer == null || m_EnemyInRangeMatrixBuffer.count != totalElements)
                {
                    m_EnemyInRangeMatrixBuffer?.Release();
                    m_EnemyInRangeMatrixBuffer = new ComputeBuffer(totalElements, intSize);
                }

                // 上传数据到 GPU
                m_BuildingPositionsBuffer.SetData(buildingPositions);
                m_BuildingRangesBuffer.SetData(buildingRanges);
                m_EnemyPositionsBuffer.SetData(enemyPositions);

                // 设置 Compute Shader 参数（先设置标量参数）
                buildingEnemyRangeShader.SetInt("BuildingCount", buildingCount);
                buildingEnemyRangeShader.SetInt("EnemyCount", enemyCount);
                buildingEnemyRangeShader.SetBuffer(m_KernelIndex, "BuildingPositions", m_BuildingPositionsBuffer);
                buildingEnemyRangeShader.SetBuffer(m_KernelIndex, "BuildingRanges", m_BuildingRangesBuffer);
                buildingEnemyRangeShader.SetBuffer(m_KernelIndex, "EnemyPositions", m_EnemyPositionsBuffer);
                buildingEnemyRangeShader.SetBuffer(m_KernelIndex, "EnemyInRangeMatrix", m_EnemyInRangeMatrixBuffer);

                // 计算线程组数量（每个线程组64个线程）
                int threadGroups = Mathf.CeilToInt(totalElements / 64.0f);
                threadGroups = Mathf.Max(1, threadGroups);

                // 执行 Compute Shader
                buildingEnemyRangeShader.Dispatch(m_KernelIndex, threadGroups, 1, 1);

                // 读取结果
                int[] results = new int[totalElements];
                m_EnemyInRangeMatrixBuffer.GetData(results);

                // 将结果转换为每个塔的敌人列表
                for (int buildingIdx = 0; buildingIdx < buildingCount; buildingIdx++)
                {
                    if (buildings[buildingIdx] == null) continue;
                    
                    List<EnemyMono> enemiesInRange = new List<EnemyMono>();

                    for (int enemyIdx = 0; enemyIdx < enemyCount; enemyIdx++)
                    {
                        int matrixIndex = buildingIdx * enemyCount + enemyIdx;
                        if (results[matrixIndex] == 1 && enemies[enemyIdx] != null)
                        {
                            enemiesInRange.Add(enemies[enemyIdx]);
                        }
                    }

                    // 更新建筑物的敌人列表
                    buildings[buildingIdx].enemiesInRange = enemiesInRange;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"UpdateBuildingsEnemyList执行错误: {e.Message}");
                // 回退到CPU计算
                FallbackToCPUCalculation();
            }
        }

        /// <summary>
        /// 回退到CPU计算（当ComputeShader失败时使用）
        /// </summary>
        private void FallbackToCPUCalculation(List<BuildingMono> buildings = null, List<EnemyMono> enemies = null)
        {
            Debug.LogWarning("回退到CPU计算（当ComputeShader失败时使用）");
            try
            {
                // 如果参数为空，重新获取数据
                if (buildings == null)
                {
                    buildings = activeBuildings
                        .Where(b => b != null && b.gameObject != null && b.buildingLogic != null && 
                                   b.buildingLogic.buildingInfo != null)
                        .ToList();
                }
                
                if (enemies == null && GameManager.Instance != null && GameManager.Instance.enemyManager != null)
                {
                    enemies = (GameManager.Instance.enemyManager.activeEnemies ?? new List<EnemyMono>())
                        .Where(e => e != null && e.gameObject != null)
                        .ToList();
                }
                else if (enemies == null)
                {
                    enemies = new List<EnemyMono>();
                }

                if (buildings.Count == 0 || enemies.Count == 0)
                {
                    ClearAllBuildingEnemyLists(buildings);
                    return;
                }

                // CPU计算每个建筑物的敌人列表
                foreach (var building in buildings)
                {
                    if (building == null) continue;
                    
                    List<EnemyMono> enemiesInRange = new List<EnemyMono>();
                    Vector3 buildingPos = building.transform.position;
                    float attackRange = building.buildingLogic.buildingInfo.attackRange.Value;
                    float attackRangeSq = attackRange * attackRange;

                    foreach (var enemy in enemies)
                    {
                        if (enemy == null) continue;
                        
                        Vector3 enemyPos = enemy.transform.position;
                        float distanceSq = (enemyPos - buildingPos).sqrMagnitude;
                        
                        if (distanceSq <= attackRangeSq)
                        {
                            enemiesInRange.Add(enemy);
                        }
                    }

                    building.enemiesInRange = enemiesInRange;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"FallbackToCPUCalculation执行错误: {e.Message}");
                ClearAllBuildingEnemyLists();
            }
        }

        /// <summary>
        /// 清空所有建筑物的敌人列表
        /// </summary>
        private void ClearAllBuildingEnemyLists(List<BuildingMono> specificBuildings = null)
        {
            var buildingsToClear = specificBuildings ?? activeBuildings;
            
            foreach (var building in buildingsToClear)
            {
                if (building != null)
                {
                    building.enemiesInRange?.Clear();
                }
            }
        }

        private void OnDestroy()
        {
            // 释放 ComputeBuffer
            m_BuildingPositionsBuffer?.Release();
            m_BuildingRangesBuffer?.Release();
            m_EnemyPositionsBuffer?.Release();
            m_EnemyInRangeMatrixBuffer?.Release();
            
            m_BuildingPositionsBuffer = null;
            m_BuildingRangesBuffer = null;
            m_EnemyPositionsBuffer = null;
            m_EnemyInRangeMatrixBuffer = null;
        }
        
        #endregion
    }
}