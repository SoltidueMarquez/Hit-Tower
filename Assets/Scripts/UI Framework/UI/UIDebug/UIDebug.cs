using UI_Framework.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace UI_Framework.UI.UIDebug
{
    public class UIDebug : UIFormBase
    {
        public GameObject debugPanel;
        public Button foldBtn;
        protected override void OnInit()
        {
            Open();
            debugPanel.SetActive(false);
            foldBtn.onClick.AddListener(() =>
            {
                debugPanel.SetActive(!debugPanel.activeSelf);
            });
        }
    }
}