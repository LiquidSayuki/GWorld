using SkillBridge.Message;
using System;
using UnityEngine;
using UnityEngine.UI;

internal class UIGuildItem: ListView.ListViewItem
{
    public Text guildId;
    public Text guildName;
    public Text members;
    public Text leaderName;
    public Image Background;
    public Sprite NormalBg;
    public Sprite SelectedBg;

    public NGuildInfo Info;

    public override void onSelected(bool selected)
    {
        this.Background.overrideSprite = selected ? SelectedBg : NormalBg;
    }

    internal void SetGuildInfo(NGuildInfo item)
    {
        this.Info = item;
        if (this.guildId != null) this.guildId.text = this.Info.Id.ToString();
        if (this.guildName != null) this.guildName.text = this.Info.guildName;
        if (this.members != null) this.members.text = this.Info.memberCount.ToString();
        if (this.leaderName != null) this.leaderName.text = this.Info.leaderName;
    }
}

