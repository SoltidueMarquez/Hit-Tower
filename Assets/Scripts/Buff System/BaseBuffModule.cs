using UnityEngine;

namespace Buff_System
{
    public abstract class BaseBuffModule : ScriptableObject
    {
        public abstract void Apply(BuffInfo buffInfo);
    }
}