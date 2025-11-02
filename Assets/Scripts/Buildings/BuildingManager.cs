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



        #region 测试部分

        [StringToEnum("Buildings")] public string buildingName;

        public bool TryBuild(Transform placementCell, out BuildingMono buildingMono)
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
    }
}