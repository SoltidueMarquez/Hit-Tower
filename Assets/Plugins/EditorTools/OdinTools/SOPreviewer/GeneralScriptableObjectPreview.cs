using System;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Plugins.EditorTools.OdinTools.SOPreviewer
{
    public class GeneralScriptableObjectPreview : OdinMenuEditorWindow
    {
        [MenuItem("EditorFramework/GeneralScriptableObjectPreview")]
        public static void OpenWindow()
        {
            WindowExtension.OpenWindow<GeneralScriptableObjectPreview>(new Vector2(800,600),new GUIContent("GeneralScriptableObjectPreview"));
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(false);
            tree.AddAssetAtPath("界面拓展显示配置","Assets/Plugins/EditorTools/Resources/SOPreviewer_Cfg.asset");
            tree.Config.DrawSearchToolbar = true;

            foreach (var dataList in SoPreviewConfig.Instance.displayDatas)
            {
                foreach (var data in dataList.value)
                {
                    if (data.so != null)
                    {
                        tree.Add(dataList.key+"/"+data.name, data.so);
                    }
                    else
                    {
                        tree.AddAssetAtPath(dataList.key+"/"+data.name, data.path);
                    }
                }
            }
            return tree;
        }
        

        protected override void OnBeginDrawEditors()
        {
            try
            {
                var selected = MenuTree.Selection.FirstOrDefault();
                var toolbarHeight = MenuTree.Config.SearchToolbarHeight;
                SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
                {
                    if (selected != null)
                    {
                        GUILayout.Label(selected.Name);
                    }

                    if ((SoPreviewConfig)MenuTree.Selection.SelectedValue == SoPreviewConfig.Instance)
                    {
                        if (GUILayout.Button("刷新页面"))
                        {
                            ForceMenuTreeRebuild();
                        }
                    }
                }
                SirenixEditorGUI.EndHorizontalToolbar();
            }
            catch (Exception e)
            {
                // Debug.LogWarning(e.ToString());
            }

        }
    }

}