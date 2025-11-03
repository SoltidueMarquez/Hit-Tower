using Buff_System;
using TMPro;
using UI_Framework.Scripts.Tools;
using UnityEngine;

namespace UI_Framework.UI.UIBuildings
{
    public class UIBuildingBuffList : MonoBehaviour
    {
        public UIList uiList;
        private BuffHandler m_BuffHandler;
        public void Init(BuffHandler buffHandler)
        {
            m_BuffHandler = buffHandler;
            m_BuffHandler.OnBuffChanged += UpdateBuffInfo;
        }

        private void UpdateBuffInfo()
        {
            if (!gameObject.activeSelf) return;
            uiList.ClearItems();
            foreach (var buffInfo in m_BuffHandler.buffList)
            {
                uiList.CloneItem<TextMeshProUGUI>().text = $"{buffInfo.buffData.buffName}: {buffInfo.curStack}layer";
            }
        }

        public void Open()
        {
            gameObject.SetActive(true);

            UpdateBuffInfo();
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            m_BuffHandler.OnBuffChanged -= UpdateBuffInfo;
        }
    }
}