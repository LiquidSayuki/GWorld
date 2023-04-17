using Assets.Scripts.Managers;
using Managers;
using Services;
using SkillBridge.Message;
using System;
using UnityEngine;

internal class UIGuild:UIWindow
{
    public GameObject itemPrefab;
    public ListView ListMain;
    public Transform itemRoot;
    public UIGuildInfo uiInfo;
    public UIGuildMemberItem selectedItem;

    public GameObject panelAdmin;
    public GameObject panelLeader;

    void Start()
    {
        //this.uiInfo = GameObject.Find("GuildInfo").GetComponent<UIGuildInfo>();
        this.ListMain.onItemSelected += this.OnMemberSelected;
        this.uiInfo.Info = GuildManager.Instance.myGuildInfo;
        GuildService.Instance.OnGuildUpdate += RefreshUI;
        RefreshUI();
    }
    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= RefreshUI;
    }

    private void OnMemberSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildMemberItem;
    }

    /// <summary>
    /// 检查工会加入申请
    /// </summary>
    public void OnClickApplyList()
    {
        UIManager.Instance.Show<UIGuildApplyList>();
    }

    /// <summary>
    /// 退出公会
    /// </summary>
    public void OnClickLeave()
    {
        MessageBox.Show(string.Format("确定要退出工会吗？"), "退出工会", MessageBoxType.Confirm).OnYes = () =>
        {
            GuildService.Instance.SendGuildLeave();
        };
    }

    public void OnClickChat()
    {

    }

    /// <summary>
    /// 踢出成员
    /// </summary>
    public void OnClickKick()
    {
        if(selectedItem == null)
        {
            MessageBox.Show("请选择要踢出的成员");
            return;
        }
        if ((int)selectedItem.Info.Title >= (int)GuildManager.Instance.myMemberInfo.Title)
        {
            MessageBox.Show("你的职务不高于对方");
            return;
        }
        MessageBox.Show(string.Format("确定要踢出[{0}]吗？", this.selectedItem.Info.Info.Name), "踢出成员", MessageBoxType.Confirm).OnYes = () =>
        {
            GuildService.Instance.SendAdminCommond(GuildAdminCommand.Kickout,this.selectedItem.Info.Info.Id);
        };
    }

    /// <summary>
    /// 升职
    /// </summary>
    public void OnClickPromote()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要晋升的成员");
            return;
        }
        if(selectedItem.Info.Title == GuildTitle.VicePresident)
        {
            MessageBox.Show("对方已经达到最高头衔");
            return;
        }
        if (selectedItem.Info.Title == GuildTitle.President)
        {
            MessageBox.Show("不要升职你自己");
            return;
        }
        MessageBox.Show(string.Format("确定要晋升[{0}]吗？", this.selectedItem.Info.Info.Name), "晋升成员", MessageBoxType.Confirm).OnYes = () =>
        {
            GuildService.Instance.SendAdminCommond(GuildAdminCommand.Promote, this.selectedItem.Info.Info.Id);
        };
    }

    /// <summary>
    /// 降职
    /// </summary>
    public void OnClickDepose()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要降职的成员");
            return;
        }
        if (selectedItem.Info.Title == GuildTitle.None)
        {
            MessageBox.Show("对方已经没有任何职务");
            return;
        }
        if (selectedItem.Info.Title == GuildTitle.President)
        {
            MessageBox.Show("会长不能被降职");
            return;
        }
        MessageBox.Show(string.Format("确定要降职[{0}]吗？", this.selectedItem.Info.Info.Name), "降职成员", MessageBoxType.Confirm).OnYes = () =>
        {
            GuildService.Instance.SendAdminCommond(GuildAdminCommand.Depost, this.selectedItem.Info.Info.Id);
        };
    }

    /// <summary>
    /// 会长转让
    /// </summary>
    public void OnClickTransfer()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要转让的成员");
            return;
        }
        MessageBox.Show(string.Format("确定要将会长转让给[{0}]吗？", this.selectedItem.Info.Info.Name), "转让会长", MessageBoxType.Confirm).OnYes = () =>
        {
            GuildService.Instance.SendAdminCommond(GuildAdminCommand.Transfer, this.selectedItem.Info.Info.Id);
        };
    }
    /// <summary>
    /// 设置公会Notice
    /// </summary>
    public void OnClickSetNotice()
    {

    }

    private void RefreshUI()
    {
        this.uiInfo.Info = GuildManager.Instance.myGuildInfo;

        ClearGuildMemberList();
        InitMemberItems();

        this.panelAdmin.SetActive(GuildManager.Instance.myMemberInfo.Title > GuildTitle.None);
        this.panelLeader.SetActive(GuildManager.Instance.myMemberInfo.Title ==  GuildTitle.President);
    }
    private void ClearGuildMemberList()
    {
        this.ListMain.RemoveAll();
    }
    private void InitMemberItems()
    {
        foreach (var item in GuildManager.Instance.guildMembers)
        {
            GameObject go = Instantiate(itemPrefab, this.ListMain.transform);
            UIGuildMemberItem ui = go.GetComponent<UIGuildMemberItem>();
            ui.SetGuildInfo(item);
            this.ListMain.AddItem(ui);
        }
    }
}
