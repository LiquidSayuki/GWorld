using Assets.Scripts.Managers;
using Common.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UIShopItem : MonoBehaviour, ISelectHandler
    {
        public Image icon;
        public Text title;
        public Text price;
        public Text limitClass;
        public Text count;


        public Image background;
        public Sprite normalBg;
        public Sprite selectedBg;
        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                this.background.overrideSprite = selected ? selectedBg : normalBg;
            }
        }

        public int ShopItemId { get; set; }
        private UIShop shop;
        private ItemDefine item;
        private ShopItemDefine ShopItem { get; set; }

        public void SetShopItem(int itemId, ShopItemDefine shopItem, UIShop owner)
        {
            this.shop = owner;
            this.ShopItemId = itemId;
            this.ShopItem = shopItem;
            this.item = DataManager.Instance.Items[this.ShopItem.ItemID];

            this.title.text = this.item.Name;
            this.count.text = shopItem.Count.ToString();
            this.price.text = shopItem.Price.ToString();
            this.icon.overrideSprite = Resloader.Load<Sprite>(item.Icon);
        }

        public void OnSelect(BaseEventData eventData)
        {
            this.Selected = true;
            this.shop.SelectShopItem(this);
        }
    }
}

