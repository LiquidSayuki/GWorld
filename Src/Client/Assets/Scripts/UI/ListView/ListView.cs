using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ListView : MonoBehaviour
{
    /// <summary>
    /// 每一个需要在ListView中展示的单个物体需要继承的类
    /// </summary>
    public class ListViewItem : MonoBehaviour, IPointerClickHandler
    {
        public ListView owner;

        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                onSelected(selected);
            }
        }
        public virtual void onSelected(bool selected) { }
        public void OnPointerClick(PointerEventData eventData)
        {
            // 点击时如果已经选中就不反应
            // 未选中时，改变自己选中状态
            // 直接改变主人ListView的选中ListViewItem状态
            if (!this.selected)
            {
                this.Selected = true;
            }
            if (owner != null && owner.SelectedItem != this)
            {
                owner.SelectedItem = this;
            }
        }
    }

    // ListView本体从此开始
    List<ListViewItem> items = new List<ListViewItem>();
    public UnityAction<ListViewItem> onItemSelected;
    private ListViewItem selectedItem = null;
    public ListViewItem SelectedItem
    {
        get { return selectedItem; }
        private set
        {
            if (selectedItem != null && selectedItem != value)
            {
                selectedItem.Selected = false;
            }
            selectedItem = value;
            // 每当SelectedItem被修改，都会发送被新的选中item给订阅者
            if (onItemSelected != null)
                onItemSelected.Invoke((ListViewItem)value);
        }
    }

    public void AddItem(ListViewItem item)
    {
        item.owner = this;
        this.items.Add(item);
    }

    public void RemoveAll()
    {
        foreach (var it in items)
        {
            Destroy(it.gameObject);
        }
        items.Clear();
    }
}
