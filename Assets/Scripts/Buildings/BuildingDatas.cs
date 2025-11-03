using System;
using System.Collections.Generic;
using System.Linq;
using Buff_System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Buildings
{
    [CreateAssetMenu(fileName = "Building", menuName = "Buildings/BuildingData")]
    public class BuildingDatas : ScriptableObject
    {
        public List<BuildingData> buildingDataList = new List<BuildingData>();

        public BuildingData GetData(string buildingName) =>
            buildingDataList.FirstOrDefault(data => data.buildingName == buildingName);
    }

    [Serializable]
    public class BuildingData
    {
        [LabelText("唯一标识符")] public string buildingName;
        [LabelText("等级数值信息")] public List<BuildingLevelData> levelData;
        [LabelText("预制体")] public GameObject prefab;
        public int maxLv => levelData.Count - 1;
        public float buildCost => levelData[0].cost;
    }

    [Serializable]
    public class BuildingLevelData
    {
        [Header("==============================================")]
        [LabelText("建造/升级到该等级花费的金币")] public float cost;
        [LabelText("拆除返还的金币")] public float giveBack;
        [LabelText("攻击力")] public float attack;
        [LabelText("攻击范围")] public float attackRange;
        [LabelText("攻击间隔")] public float attackInterval;
        [LabelText("单体攻击")] public bool ifSingle;
        [LabelText("增加的被动Buff")] public List<BuffData> addBuffs;
        
        [EnableIf("ifSingle"),LabelText("同时攻击的敌人个数")] public int attackNum;
        [HideInInspector] public string attackTargetNum=> ifSingle ? $"{attackNum}" : "All";
        
        [Header("暂时用不上")] public float maxHealth;
    }
}
