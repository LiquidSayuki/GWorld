using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Managers
{
    internal class GuildManager : Singleton<GuildManager>
    {
        public List<NGuildInfo> allGuilds;
        public List<NGuildMemberInfo> guildMembers;
        public NGuildInfo myGuildInfo;
        public NGuildMemberInfo myMemberInfo;

        public bool HasGuild
        {
            get { return this.myGuildInfo != null; }
        }

        public void Init(NGuildInfo guild)
        {
            this.myGuildInfo = guild;
            if(this.myGuildInfo != null)
            {
                this.guildMembers = myGuildInfo.Members;
            }
           

            if(guild == null)
            {
                myGuildInfo = null;
                return;
            }
            foreach(var m in guildMembers)
            {
                if(m.characterId == User.Instance.CurrentCharacter.Id)
                {
                    myMemberInfo = m;
                    break;
                }
            }
        }

        public void ShowGuild()
        {
            if (this.HasGuild) UIManager.Instance.Show<UIGuild>();
            else
            {
                var win = UIManager.Instance.Show<UIGuildPopNoGuild>();
                win.OnClose += PopNoGuild_OnClose;
            }
        }

        private void PopNoGuild_OnClose(UIWindow sender, UIWindow.WindowResult result)
        {
            if(result == UIWindow.WindowResult.Yes) // 创建公会
            {
                UIManager.Instance.Show<UIGuildPopCreate>();
            }else if(result == UIWindow.WindowResult.No) // 加入工会
            {
                UIManager.Instance.Show<UIGuildList>();
            }
        }
    }
}
