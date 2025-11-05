using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Archive
{
    [CreateAssetMenu(fileName = "ArchiveData", menuName = "ArchiveData/ArchiveData")]
    public class ArchiveData : ScriptableObject
    {
        public List<SinglePlayerArchiveData> players;

        public int curPlayerDataIndex;

        public SinglePlayerArchiveData GetCurPlayer()
        {
            if (curPlayerDataIndex >= players.Count)
            {
                return null;
            }

            return players[curPlayerDataIndex];
        }
        
        public void SetPlayer(SinglePlayerArchiveData player)
        {
            curPlayerDataIndex = players.IndexOf(player);
        }

        public void ResetCurPlayer()
        {
            if (curPlayerDataIndex >= players.Count) return;
            players[curPlayerDataIndex].Reset();
        }

        public void UpdateCurPlayerMaxWave(int newWave)
        {
            if (curPlayerDataIndex >= players.Count) return;
            players[curPlayerDataIndex].maxWave = Mathf.Max(newWave, players[curPlayerDataIndex].maxWave);
        }
    }
    
    [Serializable]
    public class SinglePlayerArchiveData
    {
        [LabelText("最高的波次")] public int maxWave = 0;

        public void Reset()
        {
            maxWave = 0;
        }
    }
}