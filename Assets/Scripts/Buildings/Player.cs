using UnityEngine;
using Utils;

namespace Buildings
{
    public class Player : Singleton<Player>
    {
        public Vector3 playerPos => transform.position;
    }
}