using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace Buildings
{
    public class BuildingLogic
    {
        
    }

    public class BuildingInfo
    {
        public string buildingName;
        
        [LabelText("攻击力")] public ValueChannel attack;
        [LabelText("攻击范围")] public ValueChannel attackRange;
        [LabelText("攻击间隔")] public ValueChannel attackInterval;
        [LabelText("单体攻击")] public bool ifSingle;
        [LabelText("同时攻击的敌人个数")] public ValueChannel attackNum;
        
        [Header("暂时用不上")] 
        public ValueChannel maxHealth;
        public float curHealth;
        
        
        public List<BuildingLevelData> levelData;

        public BuildingInfo(BuildingData data)
        {
            levelData = data.levelData;// TODO:这边需要深拷贝
        }
    }
}