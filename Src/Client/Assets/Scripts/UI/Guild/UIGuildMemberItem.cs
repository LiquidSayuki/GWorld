using SkillBridge.Message;
using System;
using UnityEngine;
using UnityEngine.UI;

internal class UIGuildMemberItem:ListView.ListViewItem
{
    public Text memberName;
    public Text memberLevel;
    public Text memberClass;
    public Text memberTitle;
    public Text memberJoinDate;
    public Text memberStatus;
    public Image Background;
    public Sprite NormalBg;
    public Sprite SelectedBg;

    public NGuildMemberInfo Info;

    public override void onSelected(bool selected)
    {
        this.Background.overrideSprite = selected ? SelectedBg : NormalBg;
    }

    internal void SetGuildInfo(NGuildMemberInfo item)
    {
        this.Info = item;
        if (this.memberName != null) this.memberName.text = this.Info.Info.Name;
        if (this.memberLevel != null) this.memberLevel.text =this.Info.Info.Level.ToString();
        if (this.memberClass != null) this.memberClass.text = this.Info.Info.Class.ToString();
        if (this.memberTitle != null) 
        {
            switch (this.Info.Title)
            {
                case GuildTitle.None:
                    this.memberTitle.text = "成员";
                    break;
                case GuildTitle.President:
                    this.memberTitle.text = "会长";
                    break;
                case GuildTitle.VicePresident:
                    this.memberTitle.text = "副会长";
                    break;
                case GuildTitle.Senior:
                    this.memberTitle.text = "元老";
                    break;
            }
        }
        if (this.memberJoinDate != null)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime dt = startTime.AddSeconds(this.Info.lastTime);
            string lastDate = dt.ToString("d");
            this.memberJoinDate.text = lastDate;
        }
        
        if (this.memberStatus != null) this.memberStatus.text = this.Info.Status == 1 ? "在线" : "离线";
    }
}

