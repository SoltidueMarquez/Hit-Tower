using Buildings;
using UnityEngine;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private PlayerData playerData;
        [SerializeField] private PlayerBuilding playerBuilding;
        public Vector3 playerPos => playerBuilding.transform.position;
        
        public PlayerLogic playerLogic;

        public void Init()
        {
            playerLogic = new PlayerLogic(playerData);
        }

        private void FixedUpdate()
        {
            playerLogic.Tick();
        }
        
        public int GetActiveBuffNum()
        {
            return playerLogic.BuffHandler.buffList.Count;
        }

        public bool CheckCanSpent(float cost)
        {
            return playerLogic.money >= cost;
        }
    }
}