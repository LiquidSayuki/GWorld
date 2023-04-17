using Assets.Scripts.Managers;
using Common.Data;
using Models;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UIShop : UIWindow
    {
        public Text Title;
        public Text Money;
        public GameObject ShopItem;

        ShopDefine shop;

        public Transform[] ItemRoot;

        void Start()
        {
            StartCoroutine(InitItems());
        }

        IEnumerator InitItems()
        {
            foreach (var kv in DataManager.Instance.ShopItems[shop.ID])
            {
                if (kv.Value.Status > 0)
                {
                    GameObject go = Instantiate(ShopItem, ItemRoot[0]);
                    UIShopItem ui = go.GetComponent<UIShopItem>();
                    ui.SetShopItem(kv.Key, kv.Value, this);
                }
            }
            yield return null;
        }

        public void SetShop(ShopDefine shop)
        {
            this.shop = shop;
            this.Title.text = shop.Name;
            this.Money.text = User.Instance.CurrentCharacter.Gold.ToString();
        }

        private UIShopItem selectedItem;
        public void SelectShopItem(UIShopItem item)
        {
            if (selectedItem != null)
            {
                selectedItem.Selected = false;
            }
            selectedItem = item;
        }

        public void OnClickBuy()
        {
            MessageBox.Show("购买", "确定");
            if (this.selectedItem != null)
            {
                ShopManager.Instance.BuyItem(this.shop.ID, this.selectedItem.ShopItemId);
            }

            return;
        }
    }
}
