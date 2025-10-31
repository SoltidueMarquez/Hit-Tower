using EditorTools;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Plugins.EditorTools.StringToEnum.EnumDataBase;

namespace Plugins.EditorTools.Editor
{
    /// <summary>
    /// 自定义属性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(StringToEnumAttribute))]
    public class StringToEnumDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 是否使用了特性的判断
            if (property.propertyType != SerializedPropertyType.String)
            {
                base.OnGUI(position, property, label);
                return;
            }

            // 获取特性实例
            StringToEnumAttribute attr = attribute as StringToEnumAttribute;
            if (attr == null)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }
            
            // 获取分类数据
            var enumList = Instance != null ? Instance.GetCategoryList(attr.Category) : null;
            if (enumList == null || enumList.Count == 0)
            {
                EditorGUI.HelpBox(position, $"分类 '{attr.Category}' 不存在或为空", MessageType.Error);
                return;
            }

            // 属性赋值方法
            void SetValue(string str)
            {
                property.stringValue = str;
                property.serializedObject.ApplyModifiedProperties();
            }

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            string str = property.stringValue;

            // 创建选择窗口
            string labelContent = string.IsNullOrEmpty(str) ? "选择枚举" : str;

            // #region 普通下拉栏
            // var e1Rect = new Rect(position.x, position.y, position.width * 0.5f - 5, position.height);
            // var e2Rect = new Rect(position.x + position.width * 0.5f + 5, position.y, position.width * 0.5f - 5, position.height);
            // if (GUI.Button(e1Rect, labelContent))
            // {
            //     GenericMenu menu = new GenericMenu();// 创建选择窗口
            //     foreach (var enumStr in EnumDataBase.Instance.enumData)// 拿到枚举数据的单例并进行绘制
            //     {
            //         string item = enumStr;
            //         menu.AddItem(new GUIContent($"{item}"), item == str, () => SetValue(item));// 传入赋值方法
            //     }
            //     // 显示出来
            //     menu.ShowAsContext();
            // }
            // #endregion

            #region 带搜索框的下拉栏
            if (EditorGUI.DropdownButton(position, new GUIContent(labelContent), FocusType.Keyboard))
            {
                // 创建搜索窗口并进行初始化
                GeneralSearchWindow searchWindow = ScriptableObject.CreateInstance<GeneralSearchWindow>();
                searchWindow.Init(enumList, (index) =>
                {
                    SetValue(enumList[index]);
                });
                
                // 计算显示位置
                Rect center = position;
                float width = 120;
                float height = 26;
                center.position += Vector2.right * width;
                center.position += Vector2.up * (position.height / 2 + height);
                Vector2 screenPosition = GUIUtility.GUIToScreenPoint(center.position);
                
                // 打开搜索窗口
                SearchWindow.Open(new SearchWindowContext(screenPosition), searchWindow);
            }
            #endregion
        }
    }
}