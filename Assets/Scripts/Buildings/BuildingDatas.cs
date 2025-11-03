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
        
        public string GetDPSAnalysis()
        {
            var analysis = new System.Text.StringBuilder();
            analysis.AppendLine($"DPS分析 - {buildingName} (Lv{0})");
            analysis.AppendLine($"攻击力: {levelData[0].attack}");
            analysis.AppendLine($"攻击间隔: {levelData[0].attackInterval}s");
            analysis.AppendLine($"攻击类型: {(levelData[0].ifSingle ? "单体" : "范围")}");
    
            if (levelData[0].ifSingle)
            {
                analysis.AppendLine($"同时攻击目标: {levelData[0].attackNum}");
            }
    
            analysis.AppendLine($"基础DPS: {levelData[0].EstimateDPS(1):F2}");
            analysis.AppendLine($"对3目标DPS: {levelData[0].EstimateDPS(3):F2}");
            analysis.AppendLine($"对5目标DPS: {levelData[0].EstimateDPS(5):F2}");
    
            return analysis.ToString();
        }
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
        public float baseSingleDps => (attackInterval <= 0) ? 0 : attack / attackInterval;
        
        [Header("暂时用不上")] public float maxHealth;
        
        public float EstimateDPS(int targetCount = 1)
        {
            // 根据攻击类型调整DPS
            if (ifSingle)
            {
                // 单体攻击：考虑同时攻击的目标数量
                return baseSingleDps * Mathf.Min(attackNum, targetCount);
            }
            else
            {
                // 范围攻击：攻击所有目标
                return baseSingleDps * targetCount;
            }
        }
    }
}
