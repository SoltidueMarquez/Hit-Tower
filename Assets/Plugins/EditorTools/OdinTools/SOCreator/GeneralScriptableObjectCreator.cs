using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.EditorTools.OdinTools;
using Plugins.EditorTools.OdinTools.SOCreator;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace ActFramework_ByHZR.EditorExtension.LessonWork
{
    public class GeneralScriptableObjectCreator : OdinMenuEditorWindow
    {
        private Type SelectedType
        {
            get
            {
                var m = MenuTree.Selection.LastOrDefault(); //因为可以多选，所以返回选中的是一个列表，这里返回的是列表的最后一个Object
                return m?.Value as Type;
            }
        }
        /// <summary>
        /// 选中的 ScriptableObject（等待创建）
        /// </summary>
        private ScriptableObject previewObject;
        
        /// <summary>
        /// 获取继承 ScriptableObject 且不是Editor相关的所有自定义类（也就是自己编写的类）
        /// </summary>
        static HashSet<Type> scriptableObjectTypes = AssemblyUtilities.GetTypes(AssemblyTypeFlags.CustomTypes)
            .Where(t =>
                t.IsClass &&
                typeof(ScriptableObject).IsAssignableFrom(t) &&
                !typeof(EditorWindow).IsAssignableFrom(t) &&
                !typeof(Editor).IsAssignableFrom(t))
            .ToHashSet();
        
        [MenuItem("EditorFramework/GeneralScriptableObjectCreator")]
        public static void OpenWindow()
        {
            WindowExtension.OpenWindow<GeneralScriptableObjectCreator>(new Vector2(800,600),new GUIContent("GeneralScriptableObjectCreator"));
        }
        
        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree(false); //不支持多选
            tree.Config.DrawSearchToolbar = true; //开启搜索状态
            tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle; //菜单设置成树形模式

            //筛选所有非抽象的类 并获取对应的路径
            tree.AddRange(scriptableObjectTypes.Where(x => !x.IsAbstract), GetMenuPathForType).AddThumbnailIcons();
            tree.Selection.SelectionChanged += e =>
            {
                //每当选择发生更改时发生进行回调2次，一次SelectionCleared 一次是ItemAdded
                if (this.previewObject && !AssetDatabase.Contains(this.previewObject))
                {
                    DestroyImmediate(previewObject);
                }

                if (e != SelectionChangedType.ItemAdded)
                {
                    return;
                }

                var t = SelectedType;
                if (t != null && !t.IsAbstract)
                {
                    previewObject = ScriptableObject.CreateInstance(t) as ScriptableObject;
                }
            };
            
            return tree;
        }
        
        protected override IEnumerable<object> GetTargets()
        {
            yield return previewObject;
        }
        
        /// <summary>
        /// 创建 ScriptableObject 时文件存储的目标文件夹
        /// </summary>
        private string targetFolder;
        private Vector2 scroll;
        protected override void DrawEditor(int index)
        {
            //scroll 内容滑动条的XY坐标
            scroll = GUILayout.BeginScrollView(scroll);
            {
                base.DrawEditor(index);
            }
            GUILayout.EndScrollView();

            if (this.previewObject)
            {
                GUILayout.FlexibleSpace(); //插入一个空隙
                SirenixEditorGUI.HorizontalLineSeparator(5); //插入一个水平分割线
                if (GUILayout.Button("Create Asset", GUILayoutOptions.Height(30)))
                {
                    CreateAsset();
                }
            }
        }
        
        private void OnSelectionChange()
        {
            var path = SelectionUtility.GetCurrentPath();
            this.titleContent = new GUIContent(path);
            targetFolder = path.Trim('/');
        }
        
        private void CreateAsset()
        {
            if (previewObject)
            {
                bool isValid = CheckValid(SelectedType);
                if (!isValid)
                {
                    return;
                }
                var dest = targetFolder + "/" + MenuTree.Selection.First().Name.ToLower() + ".asset";
                dest = AssetDatabase.GenerateUniqueAssetPath(dest); //创建唯一路径 重名后缀 +1
                Debug.Log($"要创建的为{previewObject}");
                AssetDatabase.CreateAsset(previewObject, dest);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Selection.activeObject = previewObject;
                //EditorApplication.delayCall += Close; // 如不需要创建后自动关闭可将本行注释
            }
        }
        

        private bool CheckValid(Type type)
        {
            SoCreateLimitAttribute soCreateLimitAttribute = type.GetCustomAttribute<SoCreateLimitAttribute>();
            
            if (!typeof(ScriptableObject).IsAssignableFrom(type))
            {
                Debug.LogWarning($"类型 {type} 不是 ScriptableObject 派生类");
                return false;
            }

            // 使用 type.Name 作为过滤器查找所有 ScriptableObject 资源
            string[] guids = AssetDatabase.FindAssets($"t:{type.Name}");

            int count = 0;
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(path, type);
                count++;
            }

            bool flag = true;
            if (soCreateLimitAttribute != null)
            {
                flag = soCreateLimitAttribute.soCreateCount > count;
            }
            return flag;
        }
        
        private string GetMenuPathForType(Type t)
        {
            if (t != null && scriptableObjectTypes.Contains(t))
            {
                var name = t.Name.Split('`').First().SplitPascalCase(); //主要是为了去除泛型相关 例如：Sirenix.Utilities.GlobalConfig`1[Sirenix.Serialization.GlobalSerializationConfig]
                return GetMenuPathForType(t.BaseType) + "/" + name;
            }
            return "";
        }
    }
}