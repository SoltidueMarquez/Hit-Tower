using UI_Framework.Scripts;
using UI_Framework.Scripts.Tools;

namespace UI_Framework.UI.UIShop
{
    public class UIShop : UIFormBase
    {
        public UIList shopList;
        protected override void OnInit()
        {
            Open();
            
            shopList.ClearItems();
            foreach (var shopBuff in GameManager.Instance.shopManager.shopData.shopBuffs)
            {
                shopList.CloneItem<UIShopBuffItem>().Init(shopBuff);
            }
        }
    }
}