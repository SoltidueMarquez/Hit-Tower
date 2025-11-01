using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Utils
{
    [CreateAssetMenu(fileName = "ArchiveData", menuName = "ArchiveData/ArchiveData")]
    public class ArchiveData : ScriptableObject
    {
        public List<SinglePlayerArchiveData> players;
    }
    
    [Serializable]
    public class SinglePlayerArchiveData
    {
        [LabelText("最高的波次")] public int maxWave = 0;
    }
}