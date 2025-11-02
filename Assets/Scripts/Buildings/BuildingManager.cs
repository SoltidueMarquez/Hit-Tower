using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.EditorTools;
using UnityEngine;

namespace Buildings
{
    public class BuildingManager : MonoBehaviour
    {
        public List<BuildingMono> activeBuildings = new List<BuildingMono>();
        public BuildingBuilder builder;
        
        public void Init()
        {
            builder.Init(this);
        }
        
        #region Tick
        public event Action onTick;

        public void Update()
        {
            onTick?.Invoke();
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
                building.buildingLogic.SetDie();
            }
        }
        #endregion

        #region 建造封装
        public bool TryBuild(string buildingName, Transform placementCell, out BuildingMono buildingMono)
        {
            buildingMono = null;
            if (builder.GetBuildCost(buildingName, out var cost))
            {
                if (GameManager.Instance.playerManager.playerLogic.ModifyMoney(-cost))
                {
                    buildingMono = builder.CreateBuilding("单体塔", placementCell);
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

        // TODO:思路是用ComputerShader计算，每个塔遍历所有的敌人坐标计算距离，距离小于攻击范围则视为处于范围中，将对应enemy在enemy列表中的Indedx(?这个会不会有问题)
        public void UpdateBuildingsEnemyList()
        {
            List<List<int>> res = new List<List<int>>();
            // res[i]表示第i个塔的范围内敌人列表
            // 获取到res之后再逐个更新每个塔(BuildingMono)的List<EnemyMono> enemiesInRange
            
            // 建筑物的攻击范围 activeBuildings[0].buildingLogic.buildingInfo.attackRange.Value
            // 建筑物的坐标,只取x，z，忽略y坐标： activeBuildings[0].transform.position
            // 敌人列表List<EnemyMono> GameManager.Instance.enemyManager.activeEnemies
            // 敌人坐标，同样只取xz坐标即可 GameManager.Instance.enemyManager.activeEnemies[0].transform.position
        }
        
        #endregion
        
        #region 测试部分
        [StringToEnum("Buildings")] public string buildingName;

        public void TestBuild(Transform placementCell, out BuildingMono buildingMono)
        {
            TryBuild(buildingName, placementCell,out buildingMono);
        }
        #endregion
    }
}