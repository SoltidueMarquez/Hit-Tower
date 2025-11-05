using Archive;
using UI_Framework.Scripts;
using UI_Framework.Scripts.Tools;

namespace UI_Framework.UI.UIArchive
{
    public class UIArchive : UIFormBase
    {
        public UIList archiveList;
        protected override void OnInit()
        {
        }

        protected override void DoWhenOpen()
        {
            archiveList.ClearItems();
            foreach (var player in ArchiveManager.Instance.data.players)
            {
                archiveList.CloneItem<UIArchiveItem>().Init(player);
            }
        }
    }
}