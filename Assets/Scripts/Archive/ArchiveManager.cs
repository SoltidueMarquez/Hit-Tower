using System;
using UnityEngine;

namespace Archive
{
    public class ArchiveManager : MonoBehaviour
    {
        public static ArchiveManager Instance { get; private set; }
 
        public void Init()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
 
            DontDestroyOnLoad(gameObject);

            if (data.GetCurPlayer() == null) SetCurrentPlayer(data.players[0]);
        }
        
        public ArchiveData data;

        public event Action OnArchiveSelected;

        public void SetCurrentPlayer(SinglePlayerArchiveData player)
        {
            if (player != data.GetCurPlayer())
            {
                data.SetPlayer(player);
                OnArchiveSelected?.Invoke();
            }
        }
        
        public void ResetCurPlayer() => data.ResetCurPlayer();

        public void UpdateCurPlayerMaxWave(int newWave) => data.UpdateCurPlayerMaxWave(newWave);
    }
}