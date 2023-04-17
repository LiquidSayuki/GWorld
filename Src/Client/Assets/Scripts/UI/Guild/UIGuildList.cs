using Assets.Scripts.Managers;
using Services;
using UnityEngine;

internal class UIGuildList:UIWindow
{
    public GameObject itemPrefab;
    public ListView ListMain;
    public Transform itemRoot;
    public UIGuildInfo uiInfo;
    public UIGuildItem selectedItem;

     void Start()
    {
        //this.uiInfo = GameObject.Find("GuildInfo").GetComponent<UIGuildInfo>();
        this.ListMain.onItemSelected += this.OnGuildSelected;
        this.uiInfo.Info = null;

        GuildService.Instance.OnGuildUpdate += RefreshUI;
        GuildService.Instance.SendGuildListRequest();
        //RefreshUI();
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= RefreshUI;
    }

    private void OnGuildSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildItem;
        this.uiInfo.Info = this.selectedItem.Info;
    }

    public void OnClickJoin()
    {
        if(this.selectedItem == null)
        {
            MessageBox.Show("您还未选择您想加入的工会");
            return;
        }
        MessageBox.Show(string.Format("确定要加入[{0}]吗？", selectedItem.Info.guildName), "申请加入工会", MessageBoxType.Confirm, "确定", "取消").OnYes = ()=>
        {
            GuildService.Instance.SendGuildJoinRequest(this.selectedItem.Info.Id);
        };
    }

    private void RefreshUI()
    {
        ClearGuildList();
        InitGuildItems();
    }
    private void ClearGuildList()
    {
        this.ListMain.RemoveAll();
    }
    private void InitGuildItems()
    {
        foreach (var item in GuildManager.Instance.allGuilds)
        {
            GameObject go = Instantiate(itemPrefab, this.ListMain.transform);
            UIGuildItem ui = go.GetComponent<UIGuildItem>();
            ui.SetGuildInfo(item);
            this.ListMain.AddItem(ui);
        }
    }
}

