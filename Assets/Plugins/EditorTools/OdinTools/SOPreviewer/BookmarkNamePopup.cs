using UnityEditor;
using UnityEngine;

namespace Plugins.EditorTools.OdinTools.SOPreviewer
{
    /// <summary>
    /// 简单输入框，用来让用户给书签起名。
    /// ShowUtility() 会以模态窗口方式弹出。
    /// </summary>
    public class BookmarkNamePopup : EditorWindow
    {
        private string bookmarkKey = "";
        private string bookmarkName = "";
        private string targetAssetPath; // 被标记的资源路径
        private Object currentTarget;

        /// <summary>静态入口：传入要标记的资源路径并弹窗。</summary>
        public static void Show(string assetPath,Object obj)
        {
            var window = CreateInstance<BookmarkNamePopup>();
            window.titleContent = new GUIContent("创建书签");
            window.targetAssetPath = assetPath;
            window.minSize = new Vector2(260, 160);
            window.position = new Rect(Screen.width / 2f - 130, Screen.height / 2f - 40, 260, 80);
            window.currentTarget = obj;
            window.ShowUtility(); // 模态小窗口
        }

        private void OnGUI()
        {
            GUILayout.Label("书签设置", EditorStyles.boldLabel);

            // 输入书签 Key
            GUILayout.Label("书签 Key：", EditorStyles.label);
            GUI.SetNextControlName("KeyField");
            bookmarkKey = EditorGUILayout.TextField(bookmarkKey);

            // 输入书签名称
            GUILayout.Label("书签名称：", EditorStyles.label);
            GUI.SetNextControlName("NameField");
            bookmarkName = EditorGUILayout.TextField(bookmarkName);

            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OK"))
            {
                if (string.IsNullOrWhiteSpace(bookmarkKey))
                {
                    EditorUtility.DisplayDialog("提示", "书签 Key 不能为空！", "确定");
                }
                else if (string.IsNullOrWhiteSpace(bookmarkName))
                {
                    EditorUtility.DisplayDialog("提示", "书签名称不能为空！", "确定");
                }
                else
                {
                    CreateBookmarkAsset(); // 替换为你自定义的保存逻辑
                    Close();
                }
            }

            if (GUILayout.Button("Cancel"))
            {
                Close();
            }
            GUILayout.EndHorizontal();

            // 自动聚焦第一个输入框
            // EditorGUI.FocusTextInControl("KeyField");
        }

        /// <summary>创建并保存 BookmarkAsset 资源。</summary>
        private void CreateBookmarkAsset()
        {
            if (currentTarget is ScriptableObject so)
            {
                const string folder = "Assets/Bookmarks";
                if (!AssetDatabase.IsValidFolder(folder))
                    AssetDatabase.CreateFolder("Assets", "Bookmarks");

                SoPreviewConfig.Instance.AddMark(bookmarkKey,bookmarkName,so);
                EditorUtility.SetDirty(SoPreviewConfig.Instance);
            }
        }
    }
}