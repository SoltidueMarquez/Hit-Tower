using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using SerializationUtility = Sirenix.Serialization.SerializationUtility;

namespace Plugins.EditorTools.OdinTools.CustomInspectorWindow
{
    public class OdinCustomInspectorWindow : OdinEditorWindow
    {
        public event Action onUndoPerform;
        private List<string> m_undoList;
        [NonSerialized, OdinSerialize, HideLabel, HideReferenceObjectPicker]
        private object m_inspectTargetObject;

        private Object m_originAsset;
        private EditorWindow m_editorWindow;

        [MenuItem("EditorFramework/Reference Inspector")]
        public static void OpenWindow()
        {
            WindowExtension.OpenWindow<OdinCustomInspectorWindow>(new Vector2(800,600),new GUIContent("Reference Inspector"));
        }

        [Button("清除面板")]
        public void Clear()
        {
            TryToUnInspectorObject(m_inspectTargetObject);
        }

        // private SerializedObjectListener m_serializedObject;

        public static void TryToInspectorObject(object obj, Object originAsset,
            EditorWindow editorWindow = null)
        {
            if (Instance == null)
            {
                OpenWindow();
            }

            Instance.InspectorObject(obj, originAsset, editorWindow);
        }

        public static void TryToUnInspectorObject(object obj)
        {
            if (Instance == null)
            {
                OpenWindow();
            }

            Instance.UnInspectorObject(obj);
        }

        private void UnInspectorObject(object obj)
        {
            if (m_inspectTargetObject == obj)
            {
                m_inspectTargetObject = null;
                m_originAsset = null;
                // m_serializedObject.onChanged -= CheckUpdate;
                // m_serializedObject = null;
                m_undoList.Clear();
                m_undoList = null;
            }
        }

        private void Update()
        {
            if (m_inspectTargetObject != null)
            {
                Repaint();
            }

            if (m_originAsset != null)
            {
                if (m_editorWindow != null)
                    m_editorWindow.Repaint();

                // if (m_serializedObject == null)
                // {
                //     Clear();
                //     return;
                // }
                // m_serializedObject.Update();
            }
        }

        public void InspectorObject(object obj, Object originAsset, EditorWindow editorWindow)
        {
            m_inspectTargetObject = obj;
            m_originAsset = originAsset;
            m_editorWindow = editorWindow;
            // m_serializedObject = new SerializedObjectListener(originAsset);
            // m_serializedObject.onChanged += CheckUpdate;
            m_undoList = new List<string>();
            Repaint();
        }
        //
        // private void CheckUpdate()
        // {
        //     Record();
        //     m_serializedObject.ApplyCurrentModify();
        // }

        public void Record()
        {
            if (m_undoList.Count >= 10) return;
    
            // 使用 Odin 的序列化
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                SerializationUtility.SerializeValue(m_inspectTargetObject, stream, DataFormat.Binary);
                bytes = stream.ToArray();
            }
    
            string base64 = System.Convert.ToBase64String(bytes);
            m_undoList.Add(base64);
        }

        [Button]
        public void Undo()
        {
            if (m_undoList.Count == 0) return;
            string base64Data = m_undoList[^1];
            m_undoList.RemoveAt(m_undoList.Count - 1);
    
            try
            {
                byte[] bytes = System.Convert.FromBase64String(base64Data);
        
                // 使用 Odin 的反序列化
                object deserialized;
                using (var stream = new MemoryStream(bytes))
                {
                    deserialized = SerializationUtility.DeserializeValue<object>(
                        stream, 
                        DataFormat.Binary
                    );
                }
        
                // 复制所有字段值到现有对象
                CopyAllFields(deserialized, m_inspectTargetObject);
                onUndoPerform?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        // 使用反射复制所有字段值
        private void CopyAllFields(object source, object target)
        {
            if (source == null || target == null) return;
            Type type = source.GetType();
            if (type != target.GetType()) return;

            // 获取所有字段（包括私有和继承的）
            FieldInfo[] fields = type.GetFields(
                BindingFlags.Public | 
                BindingFlags.NonPublic | 
                BindingFlags.Instance | 
                BindingFlags.FlattenHierarchy
            );

            foreach (FieldInfo field in fields)
            {
                try
                {
                    object value = field.GetValue(source);
                    field.SetValue(target, value);
                }
                catch
                {
                    // 忽略无法复制的字段
                }
            }
        }

        public void InspectorObject(object obj)
        {
            m_inspectTargetObject = obj;
            Repaint();
        }


        protected override void OnImGUI()
        {
            base.OnImGUI();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(m_originAsset);
            }
        }

        [Button]
        public void Save()
        {
            if (m_originAsset != null)
            {
            }
        }

        private static OdinCustomInspectorWindow m_instance;

        public static OdinCustomInspectorWindow Instance => m_instance;

        protected override void OnEnable()
        {
            m_instance = this;
        }

        protected override void OnDisable()
        {
            m_instance = null;
        }
    }
}