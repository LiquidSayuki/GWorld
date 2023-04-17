using SkillBridge.Message;
using System;
using UnityEngine;
using UnityEngine.UI;

internal class UIGuildInfo:MonoBehaviour
{
    public Text guildName;
    public Text guildId;
    public Text guildLeader;
    public Text guildNotice;
    public Text memberNumber;

    private NGuildInfo info;
    public NGuildInfo Info
    {
        get { return this.info; }
        set { this.info = value; this.UpdateUI(); }
    }

    private void UpdateUI()
    {
        if(this.info == null)
        {
            this.guildName.text = "无";
            this.guildId.text = "ID:0";
            this.guildLeader.text = "会长:无";
            this.guildNotice.text = " ";
            // this.memberNumber;
        }
        else
        {
            this.guildName.text = this.info.guildName;
            this.guildId.text = "ID: " + this.info.Id;
            this.guildLeader.text = "会长: " + this.info.leaderName;
            this.guildNotice.text = this.info.Notice;
        }
    }
}
