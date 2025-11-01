using Sirenix.OdinInspector;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerData",menuName = "Player/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        [LabelText("最大生命值")] public float maxHealth;
        [LabelText("初始生命值")] public float startHealth;
        [LabelText("初始货币")] public float startMoney;
    }
}