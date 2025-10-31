using System.Collections.Generic;
using System.IO;
using Enemy;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// JSON包装类用于序列化 - 需要放在命名空间内
    /// </summary>
    [System.Serializable]
    public class EnemyWaveDataWrapper
    {
        public List<EnemyWaveData> waveDataList;
    }

    #region 工具窗口
    public class EnemyWaveDataEditorWindow : EditorWindow
    {
        public EnemyWaveDatas targetData;
        private Vector2 scrollPosition;
        private string jsonFilePath = "";
        private string importResult = "";

        [MenuItem("Tools/Enemy Wave Data Editor")]
        public static void ShowWindow()
        {
            GetWindow<EnemyWaveDataEditorWindow>("Enemy Wave Editor");
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            
            // 选择目标SO文件
            EditorGUILayout.BeginHorizontal();
            targetData = (EnemyWaveDatas)EditorGUILayout.ObjectField("Target Data", targetData, typeof(EnemyWaveDatas), false);
            if (GUILayout.Button("创建新配置", GUILayout.Width(100)))
            {
                CreateNewEnemyWaveData();
            }
            EditorGUILayout.EndHorizontal();

            if (targetData == null)
            {
                EditorGUILayout.HelpBox("请选择一个EnemyWaveDatas配置文件", MessageType.Info);
                return;
            }

            GUILayout.Space(20);
            EditorGUILayout.LabelField("JSON 操作", EditorStyles.boldLabel);
            
            // JSON文件路径
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("JSON文件路径:", GUILayout.Width(100));
            jsonFilePath = EditorGUILayout.TextField(jsonFilePath);
            if (GUILayout.Button("浏览", GUILayout.Width(50)))
            {
                BrowseForJsonFile();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
            
            // 导入导出按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("导出到JSON", GUILayout.Height(30)))
            {
                ExportToJson();
            }
            if (GUILayout.Button("从JSON导入", GUILayout.Height(30)))
            {
                ImportFromJson();
            }
            EditorGUILayout.EndHorizontal();

            // 显示导入结果
            if (!string.IsNullOrEmpty(importResult))
            {
                EditorGUILayout.HelpBox(importResult, MessageType.Info);
            }

            GUILayout.Space(20);
            EditorGUILayout.LabelField("数据预览", EditorStyles.boldLabel);
            
            // 数据预览
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));
            DrawDataPreview();
            EditorGUILayout.EndScrollView();
        }

        private void CreateNewEnemyWaveData()
        {
            string path = EditorUtility.SaveFilePanel("创建新的敌人波次配置", "Assets", "EnemyWaveData", "asset");
            if (!string.IsNullOrEmpty(path))
            {
                path = FileUtil.GetProjectRelativePath(path);
                var newData = CreateInstance<EnemyWaveDatas>();
                AssetDatabase.CreateAsset(newData, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                targetData = newData;
            }
        }

        private void BrowseForJsonFile()
        {
            string path = EditorUtility.OpenFilePanel("选择JSON文件", "", "json");
            if (!string.IsNullOrEmpty(path))
            {
                jsonFilePath = path;
            }
        }

        private void ExportToJson()
        {
            if (targetData == null) return;

            if (string.IsNullOrEmpty(jsonFilePath))
            {
                jsonFilePath = EditorUtility.SaveFilePanel("导出JSON", "", "EnemyWaveData", "json");
            }

            if (!string.IsNullOrEmpty(jsonFilePath))
            {
                try
                {
                    var wrapper = new EnemyWaveDataWrapper { waveDataList = targetData.waveDataList };
                    string json = JsonUtility.ToJson(wrapper, true);
                    File.WriteAllText(jsonFilePath, json);
                    importResult = $"成功导出到: {jsonFilePath}";
                    Debug.Log($"EnemyWaveData exported to: {jsonFilePath}");
                }
                catch (System.Exception e)
                {
                    importResult = $"导出失败: {e.Message}";
                    Debug.LogError($"Export failed: {e.Message}");
                }
            }
        }

        private void ImportFromJson()
        {
            if (targetData == null || string.IsNullOrEmpty(jsonFilePath)) return;

            if (!File.Exists(jsonFilePath))
            {
                importResult = "JSON文件不存在";
                return;
            }

            try
            {
                string json = File.ReadAllText(jsonFilePath);
                var wrapper = JsonUtility.FromJson<EnemyWaveDataWrapper>(json);
                
                if (wrapper != null && wrapper.waveDataList != null)
                {
                    // 备份原始数据
                    List<EnemyWaveData> originalData = new List<EnemyWaveData>(targetData.waveDataList);
                    
                    try
                    {
                        targetData.waveDataList = wrapper.waveDataList;
                        EditorUtility.SetDirty(targetData);
                        AssetDatabase.SaveAssets();
                        importResult = $"成功导入 {wrapper.waveDataList.Count} 个波次配置";
                        Debug.Log($"EnemyWaveData imported from: {jsonFilePath}");
                    }
                    catch (System.Exception e)
                    {
                        // 恢复备份
                        targetData.waveDataList = originalData;
                        importResult = $"导入失败，已恢复数据: {e.Message}";
                        Debug.LogError($"Import failed: {e.Message}");
                    }
                }
                else
                {
                    importResult = "JSON格式错误";
                }
            }
            catch (System.Exception e)
            {
                importResult = $"导入失败: {e.Message}";
                Debug.LogError($"Import failed: {e.Message}");
            }
        }

        private void DrawDataPreview()
        {
            if (targetData.waveDataList == null || targetData.waveDataList.Count == 0)
            {
                EditorGUILayout.HelpBox("没有数据", MessageType.Info);
                return;
            }

            for (int i = 0; i < targetData.waveDataList.Count; i++)
            {
                var wave = targetData.waveDataList[i];
                EditorGUILayout.BeginVertical("box");
                
                EditorGUILayout.LabelField($"波次 {i + 1}", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"间隔时间: {wave.interval}秒");
                EditorGUILayout.LabelField($"等待时间: {wave.waitTime}秒");
                EditorGUILayout.LabelField($"敌人数量: {wave.singleWaveList?.Count ?? 0}种");

                if (wave.singleWaveList != null && wave.singleWaveList.Count > 0)
                {
                    EditorGUI.indentLevel++;
                    foreach (var single in wave.singleWaveList)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"{single.enemyName} x{single.num}");
                        EditorGUILayout.LabelField($"间隔: {single.singleInterval}秒", GUILayout.Width(80));
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
                GUILayout.Space(5);
            }
        }
    }
    #endregion

    #region 自定义Inspector
    [CustomEditor(typeof(EnemyWaveDatas))]
    public class EnemyWaveDatasEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);
            
            if (GUILayout.Button("打开波次编辑器", GUILayout.Height(30)))
            {
                EnemyWaveDataEditorWindow.ShowWindow();
                var window = EditorWindow.GetWindow<EnemyWaveDataEditorWindow>();
                window.targetData = (EnemyWaveDatas)target;
            }

            GUILayout.Space(5);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("快速导出JSON"))
            {
                QuickExport((EnemyWaveDatas)target);
            }
            if (GUILayout.Button("快速导入JSON"))
            {
                QuickImport((EnemyWaveDatas)target);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void QuickExport(EnemyWaveDatas data)
        {
            string path = EditorUtility.SaveFilePanel("导出JSON", "", "EnemyWaveData", "json");
            if (!string.IsNullOrEmpty(path))
            {
                var wrapper = new EnemyWaveDataWrapper { waveDataList = data.waveDataList };
                string json = JsonUtility.ToJson(wrapper, true);
                File.WriteAllText(path, json);
                EditorUtility.DisplayDialog("导出成功", $"数据已导出到: {path}", "确定");
            }
        }

        private void QuickImport(EnemyWaveDatas data)
        {
            string path = EditorUtility.OpenFilePanel("导入JSON", "", "json");
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    var wrapper = JsonUtility.FromJson<EnemyWaveDataWrapper>(json);
                    data.waveDataList = wrapper.waveDataList;
                    EditorUtility.SetDirty(data);
                    AssetDatabase.SaveAssets();
                    EditorUtility.DisplayDialog("导入成功", "数据导入完成", "确定");
                }
                catch (System.Exception e)
                {
                    EditorUtility.DisplayDialog("导入失败", e.Message, "确定");
                }
            }
        }
    }
    #endregion
}