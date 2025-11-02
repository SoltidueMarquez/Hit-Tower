using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Buildings
{
    [CreateAssetMenu(fileName = "Building", menuName = "Buildings/BuildingData")]
    public class BulidingDatas : ScriptableObject
    {
        public List<BuildingData> buildingDataList = new List<BuildingData>();
    }

    [Serializable]
    public class BuildingData
    {
        [LabelText("唯一标识符")] public string buildingName;
        [LabelText("等级数值信息")] public List<BuildingLevelData> levelData;
        public int MaxLv => levelData.Count;
    }

    [Serializable]
    public class BuildingLevelData
    {
        [LabelText("攻击力")] public float attack;
        [LabelText("攻击范围")] public float attackRange;
        [LabelText("攻击间隔")] public float attackInterval;
        [LabelText("单体攻击")] public bool ifSingle;
        [EnableIf("ifSingle"),LabelText("同时攻击的敌人个数")] public int attackNum;
        [Header("暂时用不上")] public float maxHealth;
    }
}
