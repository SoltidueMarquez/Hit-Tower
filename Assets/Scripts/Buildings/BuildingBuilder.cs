using ObjectPool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Buildings
{
    public class BuildingBuilder : MonoBehaviour
    {
        [SerializeField, LabelText("建筑物信息")] private BuildingDatas buildingDataList;
        private BuildingManager m_BuildingManager;

        public void Init(BuildingManager manager)
        {
            m_BuildingManager = manager;
        }

        
        #region 建造建筑物

        public BuildingMono CreateBuilding(string buildingName, Transform placementCell)
        {
            var data = buildingDataList.GetData(buildingName);
            if (data != default)
            {
                return CreateBuilding(data, placementCell);
            }
            else
            {
                Debug.LogWarning($"{buildingName}不存在");
                return null;
            }
        }

        private BuildingMono CreateBuilding(BuildingData data, Transform placementCell)
        {
            var building = GameObjectPool.Instance.Get(data.prefab, transform);
            // 设置位置
            building.transform.position = placementCell.position;

            // 初始化操作
            var mono = building.GetComponent<BuildingMono>();
            mono.Init(data, m_BuildingManager);
            return mono;
        }

        public bool GetBuildCost(string buildingName,out float cost)
        {
            cost = 0;
            var data = buildingDataList.GetData(buildingName);
            if (data == default) return false;
            cost = data.levelData[0].cost;
            return true;
        }
        #endregion
    }
}