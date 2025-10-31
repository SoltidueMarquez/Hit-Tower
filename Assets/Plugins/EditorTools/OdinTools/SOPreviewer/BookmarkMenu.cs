using UnityEditor;
using UnityEngine;

namespace Plugins.EditorTools.OdinTools.SOPreviewer
{
    public static class BookmarkMenu
    {
        // 右键菜单：仅当选中对象是 ScriptableObject 时可见
        [MenuItem("Assets/添加书签", true)]
        private static bool ValidateAddBookmark()
        {
            return Selection.activeObject != null
                   && Selection.activeObject is ScriptableObject;
        }

        // 真正的菜单回调
        [MenuItem("Assets/添加书签", false, 2000)]
        private static void AddBookmark()
        {
            var obj = Selection.activeObject;
            if (obj == null) return;

            var assetPath = AssetDatabase.GetAssetPath(obj);
            BookmarkNamePopup.Show(assetPath, obj); // 弹出输入窗口
        }
    }
}