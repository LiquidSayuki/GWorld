using Assets.Scripts.Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBag : UIWindow 
{
    public Text money;
    public Transform[] pages;
    public GameObject bagItem;
    List<Image> slots;

    void Start()
    {
        if (slots == null)
        {
            slots = new List<Image>();
            for (int page = 0; page < this.pages.Length; page++)
            {
                slots.AddRange(this.pages[page].GetComponentsInChildren<Image>(true));
            }
        }
        StartCoroutine(InitBags());
    }

    IEnumerator InitBags()
    {
        for(int i=0; i < BagManager.Instance.Items.Length;i++)
        {
            var item = BagManager.Instance.Items[i];
            if(item.ItemId > 0)
            {
                //将图标和数量赋予格子
                GameObject go = Instantiate(bagItem, slots[i].transform);
                var ui = go.GetComponent<UIIconItem>();
                var def = ItemManager.Instance.Items[item.ItemId].Define;
                ui.SetMainIcon(def.Icon, item.Count.ToString());
            }
        }
        //将多余格子设置为灰色
        for(int i = BagManager.Instance.Items.Length; i < slots.Count; i++)
        {
            slots[i].color = Color.gray;
        }
        yield return null;
    }

    // 点击整理背包时
    public void OnReset()
    {
        BagManager.Instance.Reset();
    }

    public void SetTitle(string title)
    {
        this.money.text = User.Instance.CurrentCharacter.Id.ToString();
    }
}
