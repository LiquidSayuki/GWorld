using Assets.Scripts.Managers;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuildApplyList : UIWindow
{
    public GameObject itemPrefab;
    public ListView ListMain;
    public Transform itemRoot;

    public UIGuildApplyItem selectedItem;
    //public UIGuildApplyItem uiInfo;

    void Start()
    {
        this.ListMain.onItemSelected += OnGuildApplySelected;
        GuildService.Instance.OnGuildUpdate += UpdateList;
        GuildService.Instance.SendGuildListRequest();
        this.UpdateList();
    }
    private void OnDestroy()
    {
        this.ListMain.onItemSelected = null;
        GuildService.Instance.OnGuildUpdate -= UpdateList;
    }
    private void UpdateList()
    {
        ClearList();
        InitItems();
    }

    private void InitItems()
    {
        foreach (var item in GuildManager.Instance.myGuildInfo.Applies)
        {
            GameObject go = Instantiate(itemPrefab, this.ListMain.transform);
            UIGuildApplyItem ui = go.GetComponent<UIGuildApplyItem>();
            ui.SetGuildInfo(item);
            this.ListMain.AddItem(ui);
        }
    }

    private void ClearList()
    {
        this.ListMain.RemoveAll();
    }

    private void OnGuildApplySelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildApplyItem;
        //this.uiInfo.Info = this.selectedItem.Info;
    }
}
