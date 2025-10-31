using System.Collections.Generic;
using System.Linq;
using Plugins.EditorTools.OdinTools.SOCreator;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Plugins.EditorTools.StringToEnum
{
    /// <summary>
    /// 枚举数据单例，用HashSet重新进行了封装，保证enumData不会有重复
    /// </summary>
    [SoCreateLimit(1)]
    public class EnumDataBase : SerializedScriptableObject
    {
        // 直接通过资源加载单例
        public static EnumDataBase Instance => Resources.Load<EnumDataBase>($"Enum Data Base");
        
        [DictionaryDrawerSettings(KeyLabel = "分类名称", ValueLabel = "枚举值")]
        public Dictionary<string, HashSet<string>> enumCategories = new Dictionary<string, HashSet<string>>();
        
        public List<string> GetCategoryList(string category)
        {
            if (enumCategories.TryGetValue(category, out var set))
            {
                return set.ToList();
            }
            return new List<string>();
        }
        
        [Button("添加新分类")]
        public void AddNewCategory(string categoryName)
        {
            if (!enumCategories.ContainsKey(categoryName))
            {
                enumCategories.Add(categoryName, new HashSet<string>());
                EditorUtility.SetDirty(this);
            }
        }

        [Button("向分类添加数值")]
        public void AddValueToCategory(string category, string value)
        {
            if (enumCategories.TryGetValue(category, out var set))
            {
                if (set.Add(value))
                {
                    EditorUtility.SetDirty(this);
                }
            }
        }
    }
}