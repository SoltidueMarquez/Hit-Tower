using Start;
using UI_Framework.Scripts;

namespace UI_Framework.UI.UIStart
{
    public class UIStart : UIFormBase
    {
        protected override void OnInit()
        {
            Open();
        }

        public void OpenArchiveUI()
        {
            UIMgr.Instance.GetFirstUI<UIArchive.UIArchive>().Open();
        }

        public void StartGame() => StartManager.Instance.StartGame();
        public void QuitGame() => StartManager.Instance.QuitGame();
    }
}