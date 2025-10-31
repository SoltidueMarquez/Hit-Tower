using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace EditorTools
{
    /// <summary>
    /// 自己实现一个通用搜索界面
    /// </summary>
    public class GeneralSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private List<string> m_Menus;
        private Action<int> m_Action;   // 点到第i个菜单项时的回调行为

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="menus">外部输入的菜单项列表</param>
        /// <param name="action">执行函数</param>
        public void Init(List<string> menus, Action<int> action)
        {
            m_Menus = menus;
            m_Action = action;
        }

        /// <summary>
        /// 构造一个树形结构的节点创建表
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry> 
                { new SearchTreeGroupEntry(new GUIContent(), 0) };
            
            //通过节点属性设置的路径和名称来构造一个树形结构节点分类
            List<SearchMenuItem> mainMenu = new List<SearchMenuItem>();
            foreach (string type in m_Menus)
            {
                // 获取节点属性的NodePath
                string nodePath = type;
                if (nodePath == null) continue;
                // 将路径中每一项分割
                string[] menus = nodePath.Split('/');
                // 遍历分割的每一项的名称
                List<SearchMenuItem> currentFloor = mainMenu;
                foreach (var currentName in menus)
                {
                    bool exist = false;
                    //当前项不存在 就构造当前项并加入到当前层级中
                    if (!exist)
                    {
                        SearchMenuItem item = new SearchMenuItem()
                        {
                            name = currentName
                        };
                        currentFloor.Add(item);
                    }
                }
            }

            MakeSearchTree(mainMenu, 1, ref searchTreeEntries);
            return searchTreeEntries;
        }

        /// <summary>
        /// 根据构造的节点目录结构构造最终的节点创建目录
        /// </summary>
        /// <param name="floor"></param>
        /// <param name="floorIndex"></param>
        /// <param name="treeEntries"></param>
        private void MakeSearchTree(List<SearchMenuItem> floor, int floorIndex, ref List<SearchTreeEntry> treeEntries)
        {
            foreach (var item in floor)
            {
                // 当前项不是节点
                // 构造节点项 回到顶层 继续构造
                SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(item.name))
                {
                    userData = item.name,
                    level = floorIndex
                };
                treeEntries.Add(entry);
            }
        }

        /// <summary>
        /// 用于鼠标选择一个最终条目时执行
        /// </summary>
        /// <param name="searchTreeEntry"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            int index = m_Menus.FindIndex((i) => i.Equals(searchTreeEntry.userData));
            m_Action?.Invoke(index);
            return true;
        }
    }

    /// <summary>
    /// 构造SearchWindow时 用来存储节点目录的结构
    /// </summary>
    public class SearchMenuItem
    {
        // 目录项的名称
        public string name { get; set; }
    }
}