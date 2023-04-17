using Services;
using SkillBridge.Message;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildApplyItem :ListView.ListViewItem
{
        public Text memberName;
        public Text memberLevel;
        public Text memberClass;

        public Image Background;
        public Sprite NormalBg;
        public Sprite SelectedBg;

        public NGuildApplyInfo Info;

    public override void onSelected(bool selected)
    {
        this.Background.overrideSprite = selected ? SelectedBg : NormalBg;
    }

    internal void SetGuildInfo(NGuildApplyInfo item)
    {
        this.Info = item;
        if(this.memberName != null) this.memberName.text = this.Info.Name;
        if(this.memberLevel != null) this.memberLevel.text = this.Info.Level.ToString();
        if(this.memberClass != null) this.memberClass.text = this.Info.Class.ToString();
    }

    public void OnAccept()
    {
        MessageBox.Show(string.Format("确定要让[{0}]加入工会吗？", this.Info.Name), "审批申请", MessageBoxType.Confirm).OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinApply(true,this.Info);
        };
    }

    public void OnDecline()
    {
        MessageBox.Show(string.Format("确定要拒绝[{0}]加入工会吗？", this.Info.Name), "审批申请", MessageBoxType.Confirm).OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinApply(false, this.Info);
        };
    }
}
