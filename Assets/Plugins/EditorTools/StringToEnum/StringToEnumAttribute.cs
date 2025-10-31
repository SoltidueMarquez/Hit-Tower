using System;
using UnityEngine;

namespace Plugins.EditorTools
{
    /// <summary>
    /// 自定义属性特性用于基础类型重绘
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class StringToEnumAttribute  : PropertyAttribute
    {
        public readonly string Category;
        
        public StringToEnumAttribute(string category)
        {
            Category = category;
        }
    }
}
