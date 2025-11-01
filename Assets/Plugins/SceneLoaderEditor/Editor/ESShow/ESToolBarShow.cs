using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.Scene_Editor
{
    /// <summary>
    /// 静态编辑器工具栏类（InitializeOnLoad特性确保类在Unity启动时自动初始化）
    /// </summary>
    [InitializeOnLoad]// 这个特性值只对静态的类生效，让静态方法在脚本加载时执行一次
    public static class CustomToolbarMenu
    {
        #region 配置选项
        // 是否包含未加入构建设置的场景
        public static bool IncludeNoBuild = false;
        // 是否使用叠加模式打开场景
        public static bool AdditiveModel = false;
        // 是否保持核心场景存在（暂未使用）
        public static bool WithTheAlwaysNoDieCore = false;
        #endregion
        
        // 静态构造函数：Unity加载脚本时自动执行
        static CustomToolbarMenu()
        {
            // 将GUI方法注册到工具栏右侧
            ToolbarExtender.RightToolbarGUI.Add(OnSceneSelectorToolbarGUI);
            ToolbarExtender.RightToolbarGUI.Add(OnSceneSelectorSettingsToolbarGUI);
            // 将GUI方法注册到工具栏左侧
            // ToolbarExtender.LeftToolbarGUI.Add(OnQuickSelectionToolbarGUI);
        }
        
        // 初始化标志位
        public static bool init = true;
        
        /// <summary>
        /// 场景选择器工具栏GUI（创建场景跳转下拉菜单）
        /// </summary>
        static void OnSceneSelectorToolbarGUI()
        {
            // 首次初始化时加载编辑器偏好设置
            if (init)
            {
                IncludeNoBuild = EditorPrefs.GetBool("IncludeNoBuild", false);
                AdditiveModel = EditorPrefs.GetBool("AdditiveModel", false);
                WithTheAlwaysNoDieCore = EditorPrefs.GetBool("WithTheAlwaysNoDieCore", false);
                init = false;
            }

            // 创建下拉菜单按钮
            if (EditorGUILayout.DropdownButton(
                new GUIContent("场景跳转", EditorGUIUtility.IconContent("d__Popup").image),
                FocusType.Passive, EditorStyles.toolbarDropDown))
            {
                // 创建泛型菜单
                var menu = new GenericMenu();

                // 根据设置决定包含的场景范围
                if (IncludeNoBuild)
                {
                    #region 获取项目Assets目录下所有.unity文件
                    // 得到文件路径
                    string assetsPath = Application.dataPath;
                    string[] allFiles = Directory.GetFiles(assetsPath, "*.unity", SearchOption.AllDirectories);
                    // 处理文件路路径
                    foreach (string file in allFiles)
                    {
                        // 转换为Unity可识别的相对路径
                        string relativePath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');
                        // 裁剪获取场景名称
                        string use = relativePath;
                        int indexXIE = relativePath.LastIndexOf('/') + 1;
                        int indexLast = relativePath.LastIndexOf(".unity");
                        if (indexXIE >= 0 && indexLast >= 0)
                        {
                            // 提取场景显示名称
                            string display = relativePath.Substring(indexXIE, indexLast - indexXIE);
                            // 添加菜单项
                            menu.AddItem(new GUIContent($"场景：{display}"), false, () =>
                            {
                                // 保存当前场景
                                Scene activeScene = SceneManager.GetActiveScene();
                                bool res = EditorSceneManager.SaveScene(activeScene);
                                Debug.Log("自动保存场景" + activeScene + (res ? "成功" : "失败"));
                                
                                // 根据设置使用叠加或单场景模式打开
                                if (AdditiveModel) 
                                    EditorSceneManager.OpenScene(use, mode: OpenSceneMode.Additive);
                                else 
                                    EditorSceneManager.OpenScene(use);
                            });
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 仅获取构建设置中的场景
                    var builtScenes = EditorBuildSettings.scenes;
                    foreach (var i in builtScenes)
                    {
                        string use = i.path;
                        int indexXIE = i.path.LastIndexOf('/') + 1;
                        int indexLast = i.path.LastIndexOf(".unity");
                        if (indexXIE >= 0 && indexLast >= 0)
                        {
                            string display = i.path.Substring(indexXIE, indexLast - indexXIE);
                            menu.AddItem(new GUIContent($"场景：{display}"), false, () =>
                            {
                                // 同上保存并打开场景
                                Scene activeScene = SceneManager.GetActiveScene();
                                bool res = EditorSceneManager.SaveScene(activeScene);
                                Debug.Log("自动保存场景" + activeScene + (res ? "成功" : "失败"));
                                if (AdditiveModel) 
                                    EditorSceneManager.OpenScene(use, mode: OpenSceneMode.Additive);
                                else 
                                    EditorSceneManager.OpenScene(use);
                            });
                        }
                    }
                    #endregion
                }
                
                // 添加分隔线
                menu.AddSeparator("");
                // 显示菜单
                menu.ShowAsContext();
            }
        }
        
        /// <summary>
        /// 场景选择器设置工具栏GUI
        /// </summary>
        static void OnSceneSelectorSettingsToolbarGUI()
        {
            // 创建设置下拉菜单按钮
            if (EditorGUILayout.DropdownButton(
                new GUIContent("场景跳转设置", EditorGUIUtility.IconContent("d__Popup").image),
                FocusType.Passive,
                EditorStyles.toolbarDropDown))
            {
                var menu = new GenericMenu();
                bool thisDirty = false;  // 跟踪设置是否变更
                
                // 添加设置选项菜单项
                menu.AddItem(new GUIContent("<包含未构建场景>"), IncludeNoBuild, () =>
                {
                    IncludeNoBuild = !IncludeNoBuild;
                    thisDirty = true;
                });
                
                menu.AddItem(new GUIContent("<使用叠加场景模式>"), AdditiveModel, () =>
                {
                    AdditiveModel = !AdditiveModel;
                    thisDirty = true;
                });
                
                menu.AddItem(new GUIContent("<保持核心存在>"), WithTheAlwaysNoDieCore, () =>
                {
                    WithTheAlwaysNoDieCore = !WithTheAlwaysNoDieCore;
                    thisDirty = true;
                });
                
                // 如果设置变更，保存到编辑器偏好
                if (thisDirty)
                {
                    EditorPrefs.SetBool("IncludeNoBuild", IncludeNoBuild);
                    EditorPrefs.SetBool("AdditiveModel", AdditiveModel);
                    EditorPrefs.SetBool("WithTheAlwaysNoDieCore", WithTheAlwaysNoDieCore);
                }
                
                menu.AddSeparator("");
                menu.ShowAsContext();
            }
        }
        
        // /// <summary>
        // /// 快速选择工具栏GUI（左侧工具栏）
        // /// </summary>
        // static void OnQuickSelectionToolbarGUI()
        // {
        //     // 创建快速定位下拉菜单按钮
        //     if (EditorGUILayout.DropdownButton(
        //         new GUIContent("快速定位操作", EditorGUIUtility.IconContent("d__Popup").image),
        //         FocusType.Passive,
        //         EditorStyles.toolbarDropDown, GUILayout.Width(300)))
        //     {
        //         var menu = new GenericMenu();
        //         
        //         // 添加项目文件夹定位功能
        //         menu.AddItem(new GUIContent("<框架总文件夹>"), false, () =>
        //         {
        //             // 通过名称查找资源
        //             string[] guids = AssetDatabase.FindAssets("ESFramework");
        //             if (guids.Length > 0)
        //             {
        //                 string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        //                 var use = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        //                 Selection.activeObject = use;          // 选中对象
        //                 EditorGUIUtility.PingObject(use);      // 在Project窗口高亮
        //             }
        //         });
        //         
        //         // 其他快速定位选项（实现逻辑类似）
        //         menu.AddItem(new GUIContent("<So数据总文件夹>"), false, () =>
        //         {
        //             string[] guids = AssetDatabase.FindAssets("SingleData");
        //         });
        //         
        //         menu.AddItem(new GUIContent("<编辑器总文件夹>"), false, () =>
        //         {
        //             string[] guids = AssetDatabase.FindAssets("Editor");
        //         });
        //         
        //         menu.AddItem(new GUIContent("<静态策略工具总文件夹>"), false, () =>
        //         {
        //             string[] guids = AssetDatabase.FindAssets("Static_KeyValueMaching_Partial");
        //         });
        //         
        //         menu.AddItem(new GUIContent("<全局数据总文件夹>"), false, () =>
        //         {
        //             string[] guids = AssetDatabase.FindAssets("GlobalData");
        //         });
        //         
        //         menu.AddSeparator("");
        //         
        //         // 场景内对象快速定位
        //         menu.AddItem(new GUIContent("<玩家对象>"), false, () =>
        //         {
        //             // 通过标签查找玩家对象
        //             var player = GameObject.FindGameObjectWithTag("Player");
        //             if (player != null) 
        //             {
        //                 Selection.activeGameObject = player;   // 选中场景中的对象
        //                 EditorGUIUtility.PingObject(player);   // 在Hierarchy窗口高亮
        //             }
        //         });
        //         
        //         menu.AddSeparator("");
        //         menu.ShowAsContext();
        //     }
        // }
    }
}