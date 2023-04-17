using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEquipItem : MonoBehaviour, IPointerClickHandler
{

    public Image icon;
    public Text title;
    public Text level;
    public Text limitClass;
    public Text category;

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

    bool isEquiped = false;

    public int index { get; set; }
    private UICharEquip owner;
    private Item item;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.isEquiped)
        {
            UnEquip();
        }
        else
        {
            if (this.selected)
            {
                DoEquip();
                this.selected = false;
            }
            else
            {
                this.selected = true;
            }
        }
    }

    internal void SetEquipItem(int idx, Item item, UICharEquip owner, bool equiped)
    {
        this.owner = owner;
        this.item = item;
        this.index = idx;
        this.isEquiped = equiped;

        if (this.title != null) this.title.text = this.item.Define.Name;
        if (this.level != null) this.level.text = this.item.Define.Level.ToString();
        if (this.limitClass != null) this.limitClass.text = this.item.Define.LimitClass.ToString();
        if (this.category != null) this.category.text = this.item.Define.Category;
        if (this.icon != null) this.icon.overrideSprite = Resources.Load<Sprite>(this.item.Define.Icon);

    }

    void DoEquip()
    {
        var msg = MessageBox.Show(string.Format(""), "确认", MessageBoxType.Confirm);
        msg.OnYes = () =>
        {
            var oldEquip = EquipManager.Instance.GetEquip(item.EquipInfo.Slot);
            if (oldEquip != null)
            {
                var newMsg = MessageBox.Show(string.Format(""), "确认", MessageBoxType.Confirm);
                newMsg.OnYes = () =>
                {
                    this.owner.DoEquip(this.item);
                };
            }
            else
            {
                this.owner.DoEquip(this.item);
            }
        };
    }
    void UnEquip()
    {
        var msg = MessageBox.Show(string.Format(""), "确认", MessageBoxType.Confirm);
        msg.OnYes = () =>
        {
            this.owner.UnEquip(this.item);
        };
    }
}
