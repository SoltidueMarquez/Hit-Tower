using System;
using System.Collections.Generic;
using Buff_System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Shop
{
    [CreateAssetMenu(fileName = "ShopData",menuName = "Shop/ShopData")]
    public class ShopData : ScriptableObject
    {
        [LabelText("商店售卖的buff")] public List<ShopBuff> shopBuffs;
    }
    
    [Serializable]
    public class ShopBuff
    {
        public ShopBuffType type;
        public BuffData buffData;
        public float cost;
    }

    public enum ShopBuffType
    {
        Building,
        Enemy
    }
}