using System;

namespace Plugins.EditorTools.OdinTools.SOCreator
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SoCreateLimitAttribute : Attribute
    {
        public int soCreateCount;
        
        public SoCreateLimitAttribute(int amount)
        {
            soCreateCount = amount;
        }
    }
}