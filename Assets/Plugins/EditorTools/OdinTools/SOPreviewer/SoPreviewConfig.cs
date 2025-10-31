using System.Collections.Generic;
using Plugins.EditorTools.OdinTools.SOCreator;
using UnityEngine;

namespace Plugins.EditorTools.OdinTools.SOPreviewer
{
    [SoCreateLimit(1), CreateAssetMenu(fileName = "SOPreviewer_Cfg", menuName = "Editor/SOPreviewer")]
    public class SoPreviewConfig : ScriptableObject
    {
        public static SoPreviewConfig Instance => Resources.Load<SoPreviewConfig>("SOPreviewer_Cfg");

        public List<KeyValueData<string, List<TreeDisplayData>>> displayDatas;

        public void AddMark(string key, string name, ScriptableObject so)
        {
            int index = displayDatas.FindIndex((i) => i.key.Equals(key));
            if (index == -1)
            {
                displayDatas.Add(new KeyValueData<string, List<TreeDisplayData>>(key, new List<TreeDisplayData>()));
                index = displayDatas.Count - 1;
            }

            displayDatas[index].value.Add(new TreeDisplayData()
            {
                name = name,
                so = so,
            });
        }
    }

    [System.Serializable]
    public class TreeDisplayData
    {
        public string name;
        public string path;
        public ScriptableObject so;
    }

    [System.Serializable]
    public class KeyValueData<TIkey, T>
    {
        public TIkey key;
        public T value;

        public KeyValueData()
        {
        }

        public KeyValueData(TIkey key, T value)
        {
            this.key = key;
            this.value = value;
        }
    }
}